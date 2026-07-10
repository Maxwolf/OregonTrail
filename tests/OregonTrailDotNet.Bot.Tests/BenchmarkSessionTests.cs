using OregonTrailDotNet.Bot.Game;
using OregonTrailDotNet.Bot.Testing;
using Xunit;

namespace OregonTrailDotNet.Bot.Tests
{
    /// <summary>
    ///     Covers the benchmark loop with an injected game-runner and clock: every model's time/games-to-goal is recorded,
    ///     the goal predicate is honored (a first win vs. a score threshold), models that never reach it are marked, and the
    ///     report ranks the achievers fastest-first.
    /// </summary>
    public sealed class BenchmarkSessionTests
    {
        private static RunResult Win(int score) => new() { Outcome = GameOutcome.Win, Score = score };
        private static RunResult Loss() => new() { Outcome = GameOutcome.Death };

        private static bool FirstWin(RunResult r) => r.Outcome == GameOutcome.Win;

        [Fact]
        public void Records_Time_And_Games_To_Goal_For_Every_Model()
        {
            // Each model is rigged to win after a different number of games, so first-win games/times are distinct.
            var winAfter = new Dictionary<string, int>
            {
                ["cem"] = 1, ["genetic"] = 2, ["hillclimb"] = 3, ["random"] = 4, ["neuro"] = 5
            };
            var played = new Dictionary<string, int>();
            var clock = TimeSpan.Zero;

            var session = new BenchmarkSession(0, "a first win", FirstWin,
                playGame: model =>
                {
                    var g = played.TryGetValue(model.Key, out var c) ? c + 1 : 1;
                    played[model.Key] = g;
                    return g >= winAfter[model.Key] ? Win(1000 + g) : Loss();
                },
                elapsed: () =>
                {
                    clock += TimeSpan.FromSeconds(1); // clock advances one second per game
                    return clock;
                });

            var report = session.Run(keepRunning: () => true); // stops itself once every model has reached the goal

            Assert.True(report.AllReached);
            Assert.Equal(1, report.Results.First(r => r.Key == "cem").GamesToGoal);
            Assert.Equal(5, report.Results.First(r => r.Key == "neuro").GamesToGoal);

            // The quicker achiever has the smaller time-to-goal, and the report ranks it above the slower one.
            var cem = report.Results.First(r => r.Key == "cem");
            var neuro = report.Results.First(r => r.Key == "neuro");
            Assert.True(cem.TimeToGoal < neuro.TimeToGoal);

            var text = report.Format();
            Assert.Contains("reach a first win", text);
            Assert.True(text.IndexOf("Cross-Entropy", StringComparison.Ordinal)
                        < text.IndexOf("Neuro-Evolution", StringComparison.Ordinal));
        }

        [Fact]
        public void Marks_Models_That_Never_Reach_The_Goal_When_The_Session_Ends_Early()
        {
            var played = 0;

            var session = new BenchmarkSession(5, "a first win", FirstWin,
                playGame: model => model.Key == "cem" ? Win(2000) : Loss(),
                elapsed: () => TimeSpan.FromSeconds(played));

            // Only CEM ever wins, so the session runs to the (simulated) limit rather than to "all reached".
            var report = session.Run(keepRunning: () => played++ < 20);

            Assert.False(report.AllReached);
            Assert.True(report.Results.First(r => r.Key == "cem").Reached);
            Assert.All(report.Results.Where(r => r.Key != "cem"), r => Assert.False(r.Reached));
            Assert.Contains("not reached", report.Format());
        }

        [Fact]
        public void Meek_Goal_Requires_The_Score_Threshold_Not_Just_A_Win()
        {
            const int meek = 7650;
            var played = 0;

            // The Meek test measures a score threshold: a win that falls short of 7650 does NOT count.
            var session = new BenchmarkSession(5, $"Stephen Meek's {meek}", r => r.Score >= meek,
                playGame: model => model.Key == "cem"
                    ? Win(8000) // clears Meek
                    : Win(5000), // a win, but below Meek
                elapsed: () => TimeSpan.Zero);

            var report = session.Run(keepRunning: () => played++ < 20);

            Assert.False(report.AllReached);
            var cem = report.Results.First(r => r.Key == "cem");
            Assert.True(cem.Reached);
            Assert.Equal(8000, cem.ScoreAtGoal);
            Assert.All(report.Results.Where(r => r.Key != "cem"), r => Assert.False(r.Reached));
            Assert.Contains("Stephen Meek's 7650", report.Format());

            // The highest single-game score seen across every model is tracked for the live display.
            Assert.Equal(8000, report.BestScore);
            Assert.Equal("Cross-Entropy Method", report.BestScoreModel);
            Assert.Contains("Highest score seen: 8000", report.Format());
        }

        [Fact]
        public void Tracks_Total_Score_And_Recent_Scores_Across_All_Games()
        {
            var scores = new Queue<int>(new[] { 100, 0, 250, 4000, 0 });
            var games = 0;

            var session = new BenchmarkSession(5, "a first win", FirstWin,
                playGame: _ => new RunResult { Outcome = GameOutcome.Death, Score = scores.Count > 0 ? scores.Dequeue() : 0 },
                elapsed: () => TimeSpan.Zero);

            var report = session.Run(keepRunning: () => games < 5, onProgress: _ => games++);

            Assert.Equal(5, report.TotalGames);
            Assert.Equal(4350, report.TotalScore); // 100 + 0 + 250 + 4000 + 0
            Assert.Equal(4000, report.BestScore);
            Assert.Equal(new[] { 100, 0, 250, 4000, 0 }, report.RecentScores);
            Assert.Contains("Total score across all games: 4350", report.Format());
        }

        [Fact]
        public void FormatDuration_Shows_Milliseconds_Under_A_Second()
        {
            // Games can reach a goal in well under a second, so those times must not collapse to "0:00".
            Assert.Equal("340ms", BenchmarkReport.FormatDuration(TimeSpan.FromMilliseconds(340)));
            Assert.Equal("0ms", BenchmarkReport.FormatDuration(TimeSpan.Zero));
            Assert.Equal("0:03", BenchmarkReport.FormatDuration(TimeSpan.FromSeconds(3)));
            Assert.Equal("1:05", BenchmarkReport.FormatDuration(TimeSpan.FromSeconds(65)));
        }
    }
}
