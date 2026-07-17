using System.Reflection;
using OregonTrailDotNet.Bot.Game;
using OregonTrailDotNet.Bot.Learning;
using Xunit;

namespace OregonTrailDotNet.Bot.Tests
{
    /// <summary>
    ///     Exercises the endgame trade grind through the whole real game stack: the default (warm-start) genome now
    ///     budgets 250 endgame trade browses, so a full playthrough must survive that loop — no soft-lock, no unknown
    ///     screens, no blowing the GamePlayer command/tick caps — whatever the run's outcome. Random events make each
    ///     run different, so we play a few.
    /// </summary>
    public sealed class GenomeGrindPlaythroughTests : IDisposable
    {
        static GenomeGrindPlaythroughTests()
        {
            Assembly.SetEntryAssembly(typeof(GameSimulationApp).Assembly);
        }

        public void Dispose() => GameSimulationApp.Instance?.Destroy();

        [Fact]
        public void DefaultGenome_WithEndgameGrind_PlaysCompleteGamesWithoutBugs()
        {
            for (var i = 0; i < 3; i++)
            {
                var policy = new GenomePolicy(StrategyGenome.Default(), "Grinder");
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
                Assert.Null(result.Bug);

                if (result.Outcome == GameOutcomeEnum.Win)
                {
                    Assert.True(result.Score > 0, $"Run {i} won but scored {result.Score}.");
                    if (result.GameRecordedScore.HasValue)
                        Assert.Equal(result.Score, result.GameRecordedScore.Value);
                }
            }
        }
    }
}
