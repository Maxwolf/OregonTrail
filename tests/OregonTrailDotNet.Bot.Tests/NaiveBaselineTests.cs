using System.Reflection;
using OregonTrailDotNet.Bot.Game;
using OregonTrailDotNet.Bot.Learning;
using Xunit;

namespace OregonTrailDotNet.Bot.Tests
{
    /// <summary>
    ///     The naive baseline picks a random choice at every decision, but always from the set the recognizer offers, so it
    ///     must drive complete games without the recognizer meeting an unhandled form or soft-locking on an illegal input -
    ///     even though it usually dies. This is the legality proof for <see cref="RandomPolicy" />: an out-of-set choice would
    ///     surface as an aborted run.
    /// </summary>
    public sealed class NaiveBaselineTests : IDisposable
    {
        static NaiveBaselineTests() => Assembly.SetEntryAssembly(typeof(GameSimulationApp).Assembly);

        public void Dispose() => GameSimulationApp.Instance?.Destroy();

        [Fact]
        public void NaiveBaseline_Drives_Games_Without_Gaps_Or_IllegalChoices()
        {
            for (var seed = 1; seed <= 5; seed++)
            {
                var result = GamePlayer.PlayOnce(new RandomPolicy("Chaos (bot)"), seed: seed);

                Assert.True(result.Outcome != GameOutcomeEnum.Aborted,
                    $"Naive run (seed {seed}) aborted: {result.AbortReason}");
            }
        }

        [Fact]
        public void NaiveBaseline_Is_Registered_As_A_Distinct_Weak_Floor()
        {
            var naive = TrainingModels.ByKey("naive");
            Assert.Equal("naive", naive.Key);
            Assert.IsType<RandomPolicy>(naive.Decode(naive.InitialMean(), "Chaos (bot)"));

            // It must be a different model from the strong expert-seeded "Random Search" control.
            Assert.NotEqual(naive.Key, TrainingModels.ByKey("random").Key);
        }
    }
}
