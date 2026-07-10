using System.Reflection;
using System.Text;
using OregonTrailDotNet.Bot.Data;
using OregonTrailDotNet.Bot.Diagnostics;
using OregonTrailDotNet.Bot.Game;
using OregonTrailDotNet.Bot.Learning;
using OregonTrailDotNet.Bot.Ui;

namespace OregonTrailDotNet.Bot
{
    /// <summary>
    ///     Entry point and top-level mode state machine. Alternates between the WolfCurses control panel (interactive menu) and
    ///     game sessions (headless training or a watchable playthrough). Only one WolfCurses app is alive at a time — a control
    ///     panel form records a request and tears itself down, then this loop acts on the request and rebuilds the panel.
    /// </summary>
    internal static class Program
    {
        private static volatile bool _stopRequested;

        public static int Main()
        {
            // WolfCurses' event discovery reflects the ENTRY assembly; pin it to the game once (the bot's own forms are found
            // via the hosting app's own assembly). This never changes for the life of the process.
            Assembly.SetEntryAssembly(typeof(GameSimulationApp).Assembly);

            Console.Title = "Oregon Trail Bot";
            Console.OutputEncoding = Encoding.Unicode;
            TrySetCursorVisible(false);
            Console.CancelKeyPress += (_, e) =>
            {
                _stopRequested = true;
                e.Cancel = true;
            };

            BotContext.DbPath = BotDatabase.ResolveDefaultPath();
            using var db = new BotDatabase(BotContext.DbPath);
            BotContext.Db = db;

            while (true)
            {
                RunControlPanel();

                var request = BotContext.Request;
                BotContext.Request = null;

                if (request == null || request.Kind == BotRequestKind.Quit)
                    break;

                switch (request.Kind)
                {
                    case BotRequestKind.Train:
                        RunTraining(request);
                        break;
                    case BotRequestKind.Watch:
                        RunWatch(request);
                        break;
                }
            }

            BotContext.Db = null;
            TrySetCursorVisible(true);
            SafeClear();
            Console.WriteLine("Thanks for playing. The bot rests by the trail.");
            return 0;
        }

        /// <summary>Runs the WolfCurses control panel until a form tears it down (with a pending request).</summary>
        private static void RunControlPanel()
        {
            SafeClear();
            BotSimulationApp.Create();
            var scene = BotSimulationApp.Instance!.SceneGraph;
            scene.ScreenBufferDirtyEvent += RenderToConsole;

            try
            {
                while (BotSimulationApp.Instance != null)
                {
                    var app = BotSimulationApp.Instance;
                    app.OnTick(true);

                    if (BotSimulationApp.Instance == null)
                        break;

                    if (KeyAvailable())
                    {
                        var key = Console.ReadKey(true);
                        switch (key.Key)
                        {
                            case ConsoleKey.Enter:
                                app.InputManager.SendInputBufferAsCommand();
                                break;
                            case ConsoleKey.Backspace:
                                app.InputManager.RemoveLastCharOfInputBuffer();
                                break;
                            default:
                                app.InputManager.AddCharToInputBuffer(key.KeyChar);
                                break;
                        }
                    }

                    Thread.Sleep(1);
                }
            }
            finally
            {
                scene.ScreenBufferDirtyEvent -= RenderToConsole;
            }
        }

        private static void RunTraining(BotRequest request)
        {
            SafeClear();
            TrySetCursorVisible(true);
            _stopRequested = false;

            var db = BotContext.Db!;
            var profile = db.Profiles.GetById(request.ProfileId);
            if (profile == null)
            {
                Console.WriteLine("Profile not found.");
                Pause();
                return;
            }

            Console.WriteLine($"Training '{profile.Name}' for {request.Generations} generations.");
            Console.WriteLine("Press Ctrl+C at any time to stop early and return to the menu.");
            Console.WriteLine();

            var config = new TrainingConfig
            {
                PopulationSize = request.PopulationSize,
                GamesPerCandidate = request.GamesPerCandidate,
                Generations = request.Generations
            };

            try
            {
                new TrainingSession(db, profile, config).Run(
                    onGeneration: p => Console.WriteLine(
                        $"gen {p.Generation,3}:  mean fitness {p.MeanFitness,8:F1}   best {p.BestScoreThisGen,6}   " +
                        $"best-ever {p.BestScoreEver,6}   wins {p.WinsThisGen}/{p.GamesThisGen}   (total games {p.TotalIterations})"),
                    shouldStop: () => _stopRequested);

                Console.WriteLine(_stopRequested ? "\nStopped early — progress saved." : "\nTraining complete — progress saved.");
            }
            catch (BotBugException bug)
            {
                Console.WriteLine();
                Console.WriteLine(bug.Report.Format());
            }

            _stopRequested = false;
            TrySetCursorVisible(false);
            Pause();
        }

        private static void RunWatch(BotRequest request)
        {
            var db = BotContext.Db!;
            var profile = db.Profiles.GetById(request.ProfileId);
            if (profile == null)
            {
                SafeClear();
                Console.WriteLine("Profile not found.");
                Pause();
                return;
            }

            SafeClear();
            var result = GamePlayer.PlayOnce(BuildBestPolicy(profile), watch: true, watchDelayMs: 30);

            SafeClear();
            if (result.Bug != null)
            {
                Console.WriteLine(result.Bug.Format());
            }
            else
            {
                Console.WriteLine($"{profile.Name} finished: {result.Outcome}, score {result.Score}, " +
                                  $"{result.Survivors} survivor(s), {result.Miles} miles in {result.Days} days.");
                if (!string.IsNullOrEmpty(result.CauseOfDeath))
                    Console.WriteLine($"Cause: {result.CauseOfDeath}");
                if (result.GameRecordedScore.HasValue)
                    Console.WriteLine($"(recorded to the in-game top ten as '{result.LeaderName}')");
            }

            Pause();
        }

        /// <summary>Best available strategy for a profile: its best-ever genome, else the optimizer mean, else the heuristic.</summary>
        private static IPolicy BuildBestPolicy(ProfileRecord profile)
        {
            var leaderName = $"{profile.Name} (bot)";

            if (!string.IsNullOrEmpty(profile.BestGenomeJson))
                return new GenomePolicy(StrategyGenome.FromJson(profile.BestGenomeJson!), leaderName);

            if (profile.LearningState != null)
            {
                var optimizer = new CemOptimizer();
                optimizer.Load(profile.LearningState);
                return new GenomePolicy(optimizer.MeanGenome(), leaderName);
            }

            return new HeuristicPolicy();
        }

        private static void RenderToConsole(string tuiContent)
        {
            try
            {
                var lines = tuiContent.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
                var height = Math.Max(1, Console.WindowHeight - 1);
                var width = Math.Max(1, Console.WindowWidth);
                for (var index = 0; index < height; index++)
                {
                    Console.SetCursorPosition(0, index);
                    var row = index < lines.Length ? lines[index] : string.Empty;
                    Console.Write(row.PadRight(width));
                }
            }
            catch (IOException)
            {
                // No real console — skip.
            }
        }

        private static bool KeyAvailable()
        {
            try
            {
                return Console.KeyAvailable;
            }
            catch (InvalidOperationException)
            {
                // Input is redirected (no interactive console) — treat as no key.
                return false;
            }
        }

        private static void Pause()
        {
            Console.WriteLine();
            Console.WriteLine("Press any key to return to the menu...");
            try
            {
                Console.ReadKey(true);
            }
            catch (InvalidOperationException)
            {
                // Input redirected — nothing to wait on.
            }
        }

        private static void SafeClear()
        {
            try
            {
                Console.Clear();
            }
            catch (IOException)
            {
                // No real console (redirected) — nothing to clear.
            }
        }

        private static void TrySetCursorVisible(bool visible)
        {
            try
            {
                Console.CursorVisible = visible;
            }
            catch (IOException)
            {
                // No console.
            }
        }
    }
}
