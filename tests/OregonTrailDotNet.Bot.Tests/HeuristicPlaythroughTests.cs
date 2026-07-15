using System.Reflection;
using OregonTrailDotNet;
using OregonTrailDotNet.Bot.Game;
using OregonTrailDotNet.Bot.Learning;
using Xunit;

namespace OregonTrailDotNet.Bot.Tests
{
    /// <summary>
    ///     Exercises the whole game-driving stack: the heuristic policy must drive a complete game from the main menu to a
    ///     terminal state, hitting every screen type on the fixed trail (store, rivers, forks, events, rest, game over) without
    ///     the recognizer meeting a screen it doesn't understand and without soft-locking. Random events make each run
    ///     different, so we play several.
    /// </summary>
    public sealed class HeuristicPlaythroughTests : IDisposable
    {
        static HeuristicPlaythroughTests()
        {
            Assembly.SetEntryAssembly(typeof(GameSimulationApp).Assembly);
        }

        public void Dispose()
        {
            GameSimulationApp.Instance?.Destroy();
        }

        [Fact]
        public void Heuristic_Drives_Full_Games_Without_Gaps_Or_SoftLocks()
        {
            var policy = new HeuristicPolicy();

            for (var i = 0; i < 4; i++)
            {
                using var driver = new GameDriver();
                var recognizer = new ScreenRecognizer(policy);
                driver.Boot();
                var player = new GamePlayer(driver, recognizer, policy);

                var result = player.Run();

                Assert.True(
                    result.Outcome != GameOutcomeEnum.Aborted,
                    $"Run {i} aborted: {result.AbortReason}. Unknown forms: [{string.Join(", ", player.UnknownForms)}]. " +
                    $"Last screen:\n{driver.LastScreen}");

                Assert.Empty(player.UnknownForms);

                // A finished game (win or out-of-time) should score, and the bot's recompute must match what the game
                // actually recorded to its top-ten when the score lands there.
                if (result.Outcome is GameOutcomeEnum.Win or GameOutcomeEnum.Timeout)
                {
                    Assert.True(result.Score > 0, $"Run {i} finished ({result.Outcome}) but scored {result.Score}.");
                    if (result.GameRecordedScore.HasValue)
                        Assert.Equal(result.Score, result.GameRecordedScore.Value);
                }
            }
        }
    }
}
