using OregonTrailDotNet.Bot.Diagnostics;
using OregonTrailDotNet.Bot.Learning;
using OregonTrailDotNet.Entity;
using OregonTrailDotNet.Entity.Person;

namespace OregonTrailDotNet.Bot.Game
{
    /// <summary>
    ///     Plays one full game end-to-end by repeatedly inspecting the current screen and either letting a self-advancing
    ///     screen tick forward or asking the recognizer/policy for the next input. Detects the terminal state, recomputes the
    ///     final score, and guards against soft-locks with fingerprint/tick caps.
    /// </summary>
    public sealed class GamePlayer
    {
        // Store forms keep their per-visit purchase session; anything else resets it.
        private static readonly HashSet<string> StoreForms = new() { "Store", "StorePurchase" };

        // The whole river-crossing subsystem; while inside it the per-crossing bounce counter must persist.
        private static readonly HashSet<string> RiverForms = new()
        {
            "RiverCross", "RiverCrossHelp", "UseFerryConfirm", "FerryNoMonies", "IndianGuidePrompt",
            "UseIndianConfirm", "CrossingTick", "CrossingResult", "FordRiverHelp", "CaulkRiverHelp", "FerryHelp"
        };

        // A fork and its toll-road prompt; the bounce counter persists across the pair.
        private static readonly HashSet<string> ForkForms = new() { "LocationFork", "TollRoadQuestion" };

        private readonly GameDriver _driver;
        private readonly ScreenRecognizer _recognizer;
        private readonly IPolicy _policy;

        public GamePlayer(GameDriver driver, ScreenRecognizer recognizer, IPolicy policy)
        {
            _driver = driver;
            _recognizer = recognizer;
            _policy = policy;
        }

        public IReadOnlyCollection<string> UnknownForms => _recognizer.UnknownForms;

        /// <summary>Convenience: boot a fresh game, play it with the given policy, and tear it down. Pass
        ///     <paramref name="watch" /> to render and pace it for a human; leave it null for headless training speed.</summary>
        public static RunResult PlayOnce(IPolicy policy, WatchOptions? watch = null, Func<bool>? shouldAbort = null,
            int? seed = null)
        {
            using var driver = new GameDriver(watch);
            var recognizer = new ScreenRecognizer(policy);
            try
            {
                driver.Boot(seed);
                var player = new GamePlayer(driver, recognizer, policy);
                return player.Run(shouldAbort: shouldAbort);
            }
            catch (Exception ex)
            {
                // Any unhandled exception from the game is treated as a crash bug: capture full context and stop.
                return new RunResult
                {
                    Outcome = GameOutcomeEnum.Aborted,
                    Score = 0,
                    Days = GameSimulationApp.Instance?.Time?.TotalDays ?? 0,
                    Miles = GameSimulationApp.Instance?.Vehicle?.Odometer ?? 0,
                    Survivors = GameSimulationApp.Instance?.Vehicle?.PassengerLivingCount ?? 0,
                    PartySize = GameSimulationApp.Instance?.Vehicle?.Passengers.Count ?? 0,
                    PartyHealthValue = GameSimulationApp.Instance?.Vehicle != null
                        ? (int) GameSimulationApp.Instance.Vehicle.PassengerHealthStatus
                        : 0,
                    Profession = policy.Profession,
                    StartMonth = policy.StartMonth,
                    AbortReason = "crash: " + ex.Message,
                    Bug = BugReport.Capture(BugCategoryEnum.Crash, driver, "unhandled exception while driving the game", ex)
                };
            }
        }

        /// <summary>
        ///     Drives the booted game to completion. Caps guard against infinite loops; the soft-lock detector trips when the
        ///     screen fingerprint stops changing for too long (a screen we don't know how to answer). The command/tick caps
        ///     are sized so a full endgame trade grind (up to 400 browses plus the hunts that feed it) finishes with room to
        ///     spare; a genuinely stuck game still trips the stall limit long before these.
        /// </summary>
        public RunResult Run(int maxCommands = 9000, int maxTicks = 135000, int stallLimit = 400, Func<bool>? shouldAbort = null)
        {
            var lastFingerprint = string.Empty;
            var stall = 0;
            var strandedHits = 0;

            while (true)
            {
                // A watching human can bail out (e.g. Esc) between steps; stop the game cleanly when they ask.
                if (shouldAbort != null && shouldAbort())
                    return Aborted("viewer stopped the watch");

                if (!_driver.Alive)
                    return Aborted("game instance was destroyed unexpectedly");

                if (_driver.WindowName == "GameOver")
                    return Terminal();

                // A wagon with no oxen (or an unrepairable broken part) can never move again, and the game will not end a
                // stranded-but-alive party on its own — it would just bounce between the travel menu and this screen forever.
                // Recognize the dead-end and finish the run as a failed journey.
                if (_driver.FormName == "UnableToContinue" && ++strandedHits > 3)
                    return Stranded();

                if (!StoreForms.Contains(_driver.FormName))
                    _recognizer.ResetStoreSession();
                if (!RiverForms.Contains(_driver.FormName))
                    _recognizer.ResetRiverSession();
                if (!ForkForms.Contains(_driver.FormName))
                    _recognizer.ResetForkSession();

                // Fuzzing the real game surfaces bugs: check public state invariants every step and stop if one breaks.
                var violation = CheckInvariants();
                if (violation != null)
                    return BugResult(BugCategoryEnum.InvariantViolation, violation);

                var state = GameSnapshot.Capture(GameSimulationApp.Instance!);

                // A rescued wagon (emigrant trade or fort store) can strand again later, so the dead-end detector counts
                // only CONSECUTIVE stranding sightings: reset once the wagon can roll again. A hopeless stranding still
                // terminates — nothing resets the counter while the team stays empty or the part stays broken.
                if (strandedHits > 0 && state.Oxen > 0 && state.BrokenPart == null)
                    strandedHits = 0;

                if (_driver.FormIsNull)
                {
                    var input = _driver.WindowName switch
                    {
                        "MainMenu" => "1", // Travel the trail
                        "Travel" => _recognizer.TravelChoice(_driver, state),
                        _ => "1"
                    };
                    NarrateDecision(input, state);
                    _driver.Send(input);
                }
                else if (!_driver.InputFillsBuffer)
                {
                    // Prompt or self-advancing screen: let it tick; if it made no progress, press ENTER to dismiss/advance it.
                    if (_driver.Watching)
                        _driver.StatusLine = WatchNarration.PromptStatus(_driver.FormName);

                    var before = Fingerprint();
                    _driver.Tick();

                    if (!_driver.Alive)
                        return Aborted("game instance was destroyed unexpectedly");
                    if (_driver.WindowName == "GameOver")
                        return Terminal();

                    if (Fingerprint() == before)
                        _driver.Send("");
                }
                else
                {
                    // A form the recognizer has no handler for is a gap a developer must close — stop and report it exactly.
                    var unknownBefore = _recognizer.UnknownForms.Count;
                    var input = _recognizer.TypedInput(_driver, state);
                    if (_recognizer.UnknownForms.Count > unknownBefore)
                        return BugResult(BugCategoryEnum.RecognizerGap, $"no input handler for form '{_driver.FormName}'");

                    NarrateDecision(input, state);
                    _driver.Send(input);
                }

                var fingerprint = Fingerprint();
                if (fingerprint == lastFingerprint)
                {
                    if (++stall > stallLimit)
                        return BugResult(SoftLockCategory(),
                            $"soft-lock: no progress for {stall} steps on {_driver.WindowName}/{_driver.FormName}");
                }
                else
                {
                    stall = 0;
                    lastFingerprint = fingerprint;
                }

                if (_driver.CommandCount > maxCommands)
                    return BugResult(SoftLockCategory(), $"command cap ({maxCommands}) exceeded on {_driver.WindowName}/{_driver.FormName}");
                if (_driver.TickCount > maxTicks)
                    return BugResult(SoftLockCategory(), $"tick cap ({maxTicks}) exceeded on {_driver.WindowName}/{_driver.FormName}");
            }
        }

        private string Fingerprint()
        {
            var game = GameSimulationApp.Instance;
            if (game?.Vehicle == null || game.Time == null)
                return $"{_driver.WindowName}|{_driver.FormName}";

            return $"{_driver.WindowName}|{_driver.FormName}|{game.Vehicle.Odometer}|{game.Time.TotalDays}";
        }

        private RunResult Terminal()
        {
            var game = GameSimulationApp.Instance!;
            var vehicle = game.Vehicle;
            var leader = vehicle.PassengerLeader;

            var win = game.Trail.CurrentLocation?.LastLocation == true;
            var dead = vehicle.PassengersDead;
            var outcome = win ? GameOutcomeEnum.Win : dead ? GameOutcomeEnum.Death : GameOutcomeEnum.Timeout;

            var score = outcome == GameOutcomeEnum.Death ? 0 : ScoreCalculator.Compute(vehicle);
            var cause = dead ? (leader?.Cause ?? CauseOfDeathEnum.Unknown).ToString() : "";
            var leaderName = leader?.Name ?? "";
            var days = game.Time.TotalDays;
            var miles = vehicle.Odometer;
            var survivors = vehicle.PassengerLivingCount;
            var profession = leader != null ? (int) leader.Profession : _policy.Profession;

            // For a finished game, let the game tabulate and record the score to its own top-ten (this is what stamps the
            // "(bot)" leader name onto the high-score list), then read that recorded value back for cross-checking. Death
            // records nothing, so we leave those end screens alone (avoids wandering into the epitaph flow).
            int? recorded = null;
            if (outcome != GameOutcomeEnum.Death)
            {
                var guard = 0;
                while (_driver.Alive && _driver.WindowName == "GameOver" && guard++ < 25)
                    _driver.Send("");
                recorded = FindRecordedScore(leaderName);
            }

            return new RunResult
            {
                Outcome = outcome,
                Score = score,
                GameRecordedScore = recorded,
                Days = days,
                Miles = miles,
                Survivors = survivors,
                PartySize = vehicle.Passengers.Count,
                PartyHealthValue = (int) vehicle.PassengerHealthStatus,
                CauseOfDeath = cause,
                LeaderName = leaderName,
                Profession = profession,
                StartMonth = _policy.StartMonth
            };
        }

        private RunResult Stranded()
        {
            var game = GameSimulationApp.Instance!;
            var vehicle = game.Vehicle;
            var leader = vehicle.PassengerLeader;

            // The game's UnableToContinue screen covers BOTH immovable-wagon causes; report the one that actually hit so
            // the training data doesn't lump broken-part strandings in with oxen loss.
            var cause = vehicle.Inventory[EntitiesEnum.Animal].Quantity <= 0
                ? "Stranded (no oxen to pull the wagon)"
                : vehicle.BrokenPart != null
                    ? $"Stranded (broken {vehicle.BrokenPart.Name.ToLowerInvariant()}, no spare)"
                    : "Stranded (wagon unable to continue)";

            // A stranded journey scores nothing and never finishes; model it as a failed run so the optimizer avoids it.
            // PartySize/PartyHealthValue must be filled in here: the fitness survival term reads them, and stranding is the
            // dominant failed outcome — leaving them zero silently erased the survival gradient for almost every death.
            return new RunResult
            {
                Outcome = GameOutcomeEnum.Death,
                Score = 0,
                Days = game.Time.TotalDays,
                Miles = vehicle.Odometer,
                Survivors = vehicle.PassengerLivingCount,
                PartySize = vehicle.Passengers.Count,
                PartyHealthValue = (int) vehicle.PassengerHealthStatus,
                CauseOfDeath = cause,
                LeaderName = leader?.Name ?? "",
                Profession = leader != null ? (int) leader.Profession : _policy.Profession,
                StartMonth = _policy.StartMonth
            };
        }

        // In watch mode, show what the bot is about to do (and why) and pause so a viewer can follow. No-op when headless.
        private void NarrateDecision(string input, GameSnapshot state)
        {
            if (!_driver.Watching)
                return;

            if (_driver.Watch!.Narrate)
                _driver.StatusLine = "  » " + WatchNarration.Describe(_driver.WindowName, _driver.FormName, input, state);

            _driver.Repaint();

            if (_driver.Watch!.DecisionPauseMs > 0)
                Thread.Sleep(_driver.Watch!.DecisionPauseMs);
        }

        private string? CheckInvariants()
        {
            var vehicle = GameSimulationApp.Instance?.Vehicle;
            if (vehicle == null)
                return null;

            if (vehicle.Balance < 0)
                return $"vehicle balance went negative ({vehicle.Balance:C0})";
            if (vehicle.Passengers.Count > GameSimulationApp.MAXPLAYERS)
                return $"party has {vehicle.Passengers.Count} members but the limit is {GameSimulationApp.MAXPLAYERS}";
            if (vehicle.PassengerLivingCount > vehicle.Passengers.Count)
                return $"living passenger count ({vehicle.PassengerLivingCount}) exceeds party size ({vehicle.Passengers.Count})";
            foreach (var entry in vehicle.Inventory)
                if (entry.Value.Quantity < 0)
                    return $"inventory '{entry.Key}' went negative ({entry.Value.Quantity})";

            return null;
        }

        private BugCategoryEnum SoftLockCategory() =>
            _recognizer.UnknownForms.Contains(_driver.FormName) ? BugCategoryEnum.RecognizerGap : BugCategoryEnum.SoftLock;

        private RunResult BugResult(BugCategoryEnum category, string detail)
        {
            var game = GameSimulationApp.Instance;
            return new RunResult
            {
                Outcome = GameOutcomeEnum.Aborted,
                Score = 0,
                Days = game?.Time?.TotalDays ?? 0,
                Miles = game?.Vehicle?.Odometer ?? 0,
                Survivors = game?.Vehicle?.PassengerLivingCount ?? 0,
                PartySize = game?.Vehicle?.Passengers.Count ?? 0,
                PartyHealthValue = game?.Vehicle != null ? (int) game.Vehicle.PassengerHealthStatus : 0,
                LeaderName = game?.Vehicle?.PassengerLeader?.Name ?? "",
                Profession = _policy.Profession,
                StartMonth = _policy.StartMonth,
                AbortReason = detail,
                Bug = BugReport.Capture(category, _driver, detail)
            };
        }

        private static int? FindRecordedScore(string leaderName)
        {
            var scoring = GameSimulationApp.Instance?.Scoring;
            if (scoring == null || string.IsNullOrEmpty(leaderName))
                return null;

            foreach (var highscore in scoring.TopTen)
                if (highscore.Name == leaderName)
                    return highscore.Points;

            return null;
        }

        private RunResult Aborted(string reason)
        {
            var game = GameSimulationApp.Instance;
            return new RunResult
            {
                Outcome = GameOutcomeEnum.Aborted,
                Score = 0,
                Days = game?.Time?.TotalDays ?? 0,
                Miles = game?.Vehicle?.Odometer ?? 0,
                Survivors = game?.Vehicle?.PassengerLivingCount ?? 0,
                PartySize = game?.Vehicle?.Passengers.Count ?? 0,
                PartyHealthValue = game?.Vehicle != null ? (int) game.Vehicle.PassengerHealthStatus : 0,
                LeaderName = game?.Vehicle?.PassengerLeader?.Name ?? "",
                Profession = _policy.Profession,
                StartMonth = _policy.StartMonth,
                AbortReason = reason
            };
        }
    }
}
