using OregonTrailDotNet.Bot.Game;
using OregonTrailDotNet.Bot.Learning;
using OregonTrailDotNet.Bot.Testing;
using Xunit;

namespace OregonTrailDotNet.Bot.Tests
{
    /// <summary>
    ///     Covers the benchmark loop with an injected game-runner and clock: every model's time/games-to-first-win is
    ///     recorded, models that never win are marked, and the report ranks the winners fastest-first.
    /// </summary>
    public sealed class BenchmarkSessionTests
    {
        private static RunResult Win(int score) => new() { Outcome = GameOutcome.Win, Score = score };
        private static RunResult Loss() => new() { Outcome = GameOutcome.Death };

        [Fact]
        public void Records_Time_And_Games_To_First_Win_For_Every_Model()
        {
            // Each model is rigged to win after a different number of games, so first-win games/times are distinct.
            var winAfter = new Dictionary<string, int>
            {
                ["cem"] = 1, ["genetic"] = 2, ["hillclimb"] = 3, ["random"] = 4, ["neuro"] = 5
            };
            var played = new Dictionary<string, int>();
            var clock = TimeSpan.Zero;

            var session = new BenchmarkSession(0,
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

            var report = session.Run(keepRunning: () => true); // stops itself once every model has won

            Assert.True(report.AllWon);
            Assert.Equal(1, report.Results.First(r => r.Key == "cem").GamesToFirstWin);
            Assert.Equal(5, report.Results.First(r => r.Key == "neuro").GamesToFirstWin);

            // The quicker winner has the smaller time-to-first-win, and the report ranks it above the slower one.
            var cem = report.Results.First(r => r.Key == "cem");
            var neuro = report.Results.First(r => r.Key == "neuro");
            Assert.True(cem.TimeToFirstWin < neuro.TimeToFirstWin);

            var text = report.Format();
            Assert.Contains("TIME TO FIRST WIN", text);
            Assert.True(text.IndexOf("Cross-Entropy", StringComparison.Ordinal)
                        < text.IndexOf("Neuro-Evolution", StringComparison.Ordinal));
        }

        [Fact]
        public void Marks_Models_That_Never_Win_When_The_Session_Ends_Early()
        {
            var played = 0;

            var session = new BenchmarkSession(5,
                playGame: model => model.Key == "cem" ? Win(2000) : Loss(),
                elapsed: () => TimeSpan.FromSeconds(played));

            // Only CEM ever wins, so the session runs to the (simulated) limit rather than to "all won".
            var report = session.Run(keepRunning: () => played++ < 20);

            Assert.False(report.AllWon);
            Assert.True(report.Results.First(r => r.Key == "cem").Won);
            Assert.All(report.Results.Where(r => r.Key != "cem"), r => Assert.False(r.Won));
            Assert.Contains("no win", report.Format());
        }
    }
}
