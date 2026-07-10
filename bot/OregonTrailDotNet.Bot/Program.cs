using System.Reflection;
using System.Text;
using System.Text.Json;
using OregonTrailDotNet.Bot.Data;
using OregonTrailDotNet.Bot.Diagnostics;
using OregonTrailDotNet.Bot.Game;
using OregonTrailDotNet.Bot.Learning;
using OregonTrailDotNet.Bot.Testing;
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
                    case BotRequestKind.AutoTest:
                        RunAutoTest(request);
                        break;
                    case BotRequestKind.Benchmark:
                        RunBenchmark(request);
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

            var options = WatchOptions.ForSpeed(request.WatchSpeed);

            while (true)
            {
                DrainKeys(); // discard any keystrokes queued before this game starts
                SafeClear();

                // Esc stops the watch: mid-game it aborts the run, and the flag lets us skip the result screen.
                var stoppedByViewer = false;
                var result = GamePlayer.PlayOnce(BuildBestPolicy(profile), options, shouldAbort: () =>
                {
                    if (!EscPressed())
                        return false;
                    stoppedByViewer = true;
                    return true;
                });

                SafeClear();

                if (stoppedByViewer)
                {
                    Console.WriteLine("Stopped watching.");
                    Pause();
                    return;
                }

                // A crash or detected bug is a developer-stop condition — always surface it and end the session.
                if (result.Bug != null)
                {
                    Console.WriteLine(result.Bug.Format());
                    Pause();
                    return;
                }

                PrintWatchResult(profile, result);

                if (!request.LoopUntilEscape)
                {
                    Pause();
                    return;
                }

                Console.WriteLine();
                Console.WriteLine("Starting another game… press Esc to stop looping.");
                if (WaitOrEsc(2500))
                {
                    Console.WriteLine("Stopped watching.");
                    Pause();
                    return;
                }
            }
        }

        private static void PrintWatchResult(ProfileRecord profile, RunResult result)
        {
            Console.WriteLine($"{profile.Name} finished: {result.Outcome}, score {result.Score}, " +
                              $"{result.Survivors} survivor(s), {result.Miles} miles in {result.Days} days.");
            if (!string.IsNullOrEmpty(result.CauseOfDeath))
                Console.WriteLine($"Cause: {result.CauseOfDeath}");
            if (result.GameRecordedScore.HasValue)
                Console.WriteLine($"(recorded to the in-game top ten as '{result.LeaderName}')");
        }

        /// <summary>Drains pending keystrokes and reports whether Esc was among them (no-op when input is redirected).</summary>
        private static bool EscPressed()
        {
            var esc = false;
            while (KeyAvailable())
                if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                    esc = true;
            return esc;
        }

        private static void DrainKeys()
        {
            while (KeyAvailable())
                Console.ReadKey(true);
        }

        /// <summary>Waits up to <paramref name="milliseconds" />, returning true early if the viewer pressed Esc.</summary>
        private static bool WaitOrEsc(int milliseconds)
        {
            var waited = 0;
            while (waited < milliseconds)
            {
                if (EscPressed())
                    return true;
                Thread.Sleep(50);
                waited += 50;
            }

            return false;
        }

        private static void RunAutoTest(BotRequest request)
        {
            SafeClear();
            TrySetCursorVisible(false);
            DrainKeys();

            var minutes = Math.Max(0, request.AutoTestMinutes);
            var infinite = minutes <= 0;
            var startedAt = DateTime.UtcNow;
            var deadline = infinite ? DateTime.MaxValue : startedAt.AddMinutes(minutes);
            var stoppedByEsc = false;
            var lastRender = DateTime.MinValue;

            var session = new AutoTestSession(minutes, request.AutoTestStopOnProblem);

            var report = session.Run(
                keepRunning: () =>
                {
                    if (EscPressed())
                    {
                        stoppedByEsc = true;
                        return false;
                    }

                    return infinite || DateTime.UtcNow < deadline;
                },
                onProgress: r =>
                {
                    // Throttle the dashboard so redrawing doesn't steal time from actually running games.
                    var now = DateTime.UtcNow;
                    if ((now - lastRender).TotalMilliseconds < 120)
                        return;
                    lastRender = now;
                    RenderAutoTestDashboard(r, startedAt, deadline, infinite);
                });

            if (!report.StoppedOnProblem)
                report.EndReason = stoppedByEsc ? "Stopped by Esc." :
                    infinite ? "Session stopped." : "Reached the configured time limit.";

            var savedPath = SaveAutoTestReport(report);

            SafeClear();
            TrySetCursorVisible(true);
            Console.WriteLine(report.Format());
            Console.WriteLine();
            Console.WriteLine(savedPath != null
                ? $"Report saved to: {savedPath}"
                : "(Could not save the report to disk — it is shown above.)");
            Pause();
            TrySetCursorVisible(false);
        }

        private static void RenderAutoTestDashboard(AutoTestReport report, DateTime startedAt, DateTime deadline, bool infinite)
        {
            try
            {
                var width = Math.Max(1, Console.WindowWidth);
                var elapsed = DateTime.UtcNow - startedAt;
                var limit = infinite ? "no limit" : $"{(int) (deadline - startedAt).TotalMinutes}:{(deadline - startedAt).Seconds:00}";

                var lines = new List<string>
                {
                    "AUTOMATED TESTING — one bot of every model, hunting game bugs",
                    $"elapsed {(int) elapsed.TotalMinutes}:{elapsed.Seconds:00}   limit {limit}   (press Esc to stop)",
                    "",
                    $"{"Model",-24}{"Games",7}{"Wins",6}{"Deaths",8}{"Timeouts",10}{"Problems",10}"
                };
                foreach (var m in report.Models)
                    lines.Add($"{Truncate(m.DisplayName, 23),-24}{m.Games,7}{m.Wins,6}{m.Deaths,8}{m.Timeouts,10}{m.Problems,10}");
                lines.Add("");
                lines.Add($"Total games: {report.TotalGames}     Problems found: {report.Problems.Count}");
                lines.Add($"On problem: {(report.StopOnProblem ? "stop the session" : "keep going and log it")}");

                var rows = Math.Max(lines.Count, Math.Max(1, Console.WindowHeight - 1));
                for (var i = 0; i < rows; i++)
                {
                    Console.SetCursorPosition(0, i);
                    var row = i < lines.Count ? lines[i] : string.Empty;
                    if (row.Length > width) row = row[..width];
                    Console.Write(row.PadRight(width));
                }
            }
            catch (IOException)
            {
                // No real console (redirected) — skip rendering.
            }
            catch (ArgumentOutOfRangeException)
            {
                // Console resized very small — skip this frame.
            }
        }

        private static string? SaveAutoTestReport(AutoTestReport report)
        {
            try
            {
                var dir = Path.Combine(AppContext.BaseDirectory, "test-reports");
                Directory.CreateDirectory(dir);
                var file = Path.Combine(dir, $"automated-test-{DateTime.UtcNow:yyyyMMdd-HHmmss}.txt");
                File.WriteAllText(file, report.Format());
                return file;
            }
            catch (Exception)
            {
                // Disk not writable (e.g. read-only deployment) — the report is still shown on screen.
                return null;
            }
        }

        private static void RunBenchmark(BotRequest request)
        {
            SafeClear();
            TrySetCursorVisible(false);
            DrainKeys();

            var minutes = Math.Max(0, request.BenchmarkMinutes);
            var infinite = minutes <= 0;
            var startedAt = DateTime.UtcNow;
            var deadline = infinite ? DateTime.MaxValue : startedAt.AddMinutes(minutes);
            var stoppedByEsc = false;
            var lastRender = DateTime.MinValue;

            var session = new BenchmarkSession(minutes);

            var report = session.Run(
                keepRunning: () =>
                {
                    if (EscPressed())
                    {
                        stoppedByEsc = true;
                        return false;
                    }

                    return infinite || DateTime.UtcNow < deadline;
                },
                onProgress: r =>
                {
                    var now = DateTime.UtcNow;
                    if ((now - lastRender).TotalMilliseconds < 120)
                        return;
                    lastRender = now;
                    RenderBenchmarkDashboard(r, startedAt, deadline, infinite);
                });

            report.EndReason = report.AllWon ? "Every model scored a win." :
                stoppedByEsc ? "Stopped by Esc." : "Reached the configured time limit.";

            var savedPath = SaveBenchmarkReport(report);

            SafeClear();
            TrySetCursorVisible(true);
            Console.WriteLine(report.Format());
            Console.WriteLine();
            Console.WriteLine(savedPath != null
                ? $"Report saved to: {savedPath}"
                : "(Could not save the report to disk — it is shown above.)");
            Pause();
            TrySetCursorVisible(false);
        }

        private static void RenderBenchmarkDashboard(BenchmarkReport report, DateTime startedAt, DateTime deadline, bool infinite)
        {
            try
            {
                var width = Math.Max(1, Console.WindowWidth);
                var elapsed = DateTime.UtcNow - startedAt;
                var limit = infinite ? "until all win" : $"{(int) (deadline - startedAt).TotalMinutes}:{(deadline - startedAt).Seconds:00}";

                var lines = new List<string>
                {
                    "BENCHMARK — how long each model takes to score its first win",
                    $"elapsed {(int) elapsed.TotalMinutes}:{elapsed.Seconds:00}   limit {limit}   (press Esc to stop)",
                    "",
                    $"{"Model",-24}{"Games",8}{"First win",16}"
                };
                foreach (var r in report.Results)
                {
                    var status = r.Won
                        ? $"{(int) r.TimeToFirstWin.TotalMinutes}:{r.TimeToFirstWin.Seconds:00} (g{r.GamesToFirstWin})"
                        : "— not yet —";
                    lines.Add($"{Truncate(r.DisplayName, 23),-24}{r.Games,8}{status,16}");
                }
                lines.Add("");
                lines.Add($"Total games: {report.TotalGames}     Won: {report.Results.Count(r => r.Won)}/{report.Results.Count}");

                var rows = Math.Max(lines.Count, Math.Max(1, Console.WindowHeight - 1));
                for (var i = 0; i < rows; i++)
                {
                    Console.SetCursorPosition(0, i);
                    var row = i < lines.Count ? lines[i] : string.Empty;
                    if (row.Length > width) row = row[..width];
                    Console.Write(row.PadRight(width));
                }
            }
            catch (IOException)
            {
                // No real console (redirected) — skip rendering.
            }
            catch (ArgumentOutOfRangeException)
            {
                // Console resized very small — skip this frame.
            }
        }

        private static string? SaveBenchmarkReport(BenchmarkReport report)
        {
            try
            {
                var dir = Path.Combine(AppContext.BaseDirectory, "test-reports");
                Directory.CreateDirectory(dir);
                var file = Path.Combine(dir, $"benchmark-{DateTime.UtcNow:yyyyMMdd-HHmmss}.txt");
                File.WriteAllText(file, report.Format());
                return file;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static string Truncate(string text, int max) => text.Length <= max ? text : text[..max];

        /// <summary>Best available strategy for a profile, decoded through the profile's own training model: its best-ever
        ///     vector, else the optimizer's current mean, else the hand-tuned heuristic.</summary>
        private static IPolicy BuildBestPolicy(ProfileRecord profile)
        {
            var leaderName = $"{profile.Name} (bot)";
            var model = TrainingModels.ByKey(profile.PolicyKind);

            if (!string.IsNullOrEmpty(profile.BestGenomeJson))
            {
                var vector = JsonSerializer.Deserialize<double[]>(profile.BestGenomeJson!);
                if (vector is { Length: > 0 })
                    return model.Decode(vector, leaderName);
            }

            if (profile.LearningState != null)
            {
                var optimizer = model.CreateOptimizer(1);
                optimizer.Load(profile.LearningState);
                return model.Decode(optimizer.MeanVector(), leaderName);
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
                    if (row.Length > width) row = row[..width]; // never let a long line wrap and shove content off-screen
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
