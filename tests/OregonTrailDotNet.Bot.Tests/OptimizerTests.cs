using OregonTrailDotNet.Bot.Learning;
using Xunit;

namespace OregonTrailDotNet.Bot.Tests
{
    /// <summary>Pure tests of the training-model registry and each model's optimizer (no game, no database).</summary>
    public sealed class OptimizerTests
    {
        [Fact]
        public void Registry_Has_All_Five_Models_And_Resolves_Keys()
        {
            var keys = TrainingModels.All.Select(m => m.Key).ToList();

            Assert.Equal(5, TrainingModels.All.Count);
            foreach (var expected in new[] { "cem", "genetic", "hillclimb", "random", "neuro" })
                Assert.Contains(expected, keys);

            Assert.Equal("cem", TrainingModels.Default.Key);
            foreach (var m in TrainingModels.All)
                Assert.Equal(m.Key, TrainingModels.ByKey(m.Key).Key);

            // Unknown/legacy keys fall back to CEM.
            Assert.Equal("cem", TrainingModels.ByKey("nonsense").Key);
            Assert.Equal("cem", TrainingModels.ByKey(null).Key);
        }

        [Fact]
        public void Every_Model_Optimizer_Samples_Updates_And_RoundTrips()
        {
            foreach (var model in TrainingModels.All)
            {
                var optimizer = model.CreateOptimizer(8);

                var population = optimizer.Sample();
                Assert.Equal(8, population.Count);
                Assert.All(population, v => Assert.Equal(model.VectorLength, v.Length));

                // Score them (fitness = index) and fold back in.
                var scored = population.Select((v, i) => (v, (double) (i * 10))).ToList();
                optimizer.Update(scored);

                Assert.Equal(1, optimizer.Generation);
                Assert.NotNull(optimizer.BestVector);
                Assert.Equal(model.VectorLength, optimizer.MeanVector().Length);

                // Serialize/Load into a fresh optimizer of the same model preserves progress (resume).
                var restored = model.CreateOptimizer(8);
                restored.Load(optimizer.Serialize());
                Assert.Equal(optimizer.Generation, restored.Generation);
                Assert.Equal(optimizer.BestFitness, restored.BestFitness);
            }
        }
    }
}
