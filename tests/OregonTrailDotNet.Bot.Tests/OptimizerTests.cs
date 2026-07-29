using System.Text;
using System.Text.Json.Nodes;
using OregonTrailDotNet.Bot.Learning;
using Xunit;

namespace OregonTrailDotNet.Bot.Tests
{
    /// <summary>Pure tests of the training-model registry and each model's optimizer (no game, no database).</summary>
    public sealed class OptimizerTests
    {
        [Fact]
        public void Registry_Has_All_Models_And_Resolves_Keys()
        {
            var keys = TrainingModels.All.Select(m => m.Key).ToList();

            Assert.Equal(6, TrainingModels.All.Count);
            foreach (var expected in new[] { "cem", "genetic", "hillclimb", "random", "neuro", "naive" })
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

        [Fact]
        public void Every_Model_Optimizer_Serializes_BeforeItsFirstUpdate()
        {
            foreach (var model in TrainingModels.All)
            {
                // A fresh optimizer has no champion yet, and TrainingSession persists in exactly that state: an Esc/Ctrl+C
                // stop or a bug-halt during the FIRST generation calls PersistOptimizer() before any Update() has run. When
                // BestFitness started at NegativeInfinity that threw ("cannot be written as valid JSON"), so the abort lost
                // the whole run's learning state and a bug-halt reported the JSON error instead of the bug it caught.
                var optimizer = model.CreateOptimizer(8);
                Assert.Null(optimizer.BestVector);

                var blob = optimizer.Serialize();

                var restored = model.CreateOptimizer(8);
                restored.Load(blob);
                Assert.Equal(0, restored.Generation);
                Assert.Null(restored.BestVector);

                // ...and the resumed optimizer still adopts the first champion it sees, even a negative-fitness one (every
                // party dead short of Oregon scores below zero), so nothing is frozen out by the sentinel.
                var scored = restored.Sample().Select((v, i) => (v, -1000.0 + i)).ToList();
                restored.Update(scored);
                Assert.NotNull(restored.BestVector);
                Assert.Equal(-1000.0 + (scored.Count - 1), restored.BestFitness);
            }
        }

        [Fact]
        public void Load_DropsTheChampion_FromABlobSavedUnderAnOlderFitnessShaping()
        {
            foreach (var model in TrainingModels.All)
            {
                var optimizer = model.CreateOptimizer(8);
                var scored = optimizer.Sample().Select((v, i) => (v, (double) (i * 10))).ToList();
                optimizer.Update(scored);
                Assert.NotNull(optimizer.BestVector);

                // Blobs written before fitness versioning carry no FitnessVersion property (reads back as 0).
                var node = JsonNode.Parse(optimizer.Serialize())!.AsObject();
                node.Remove("FitnessVersion");
                var preVersioningBlob = Encoding.UTF8.GetBytes(node.ToJsonString());

                var restored = model.CreateOptimizer(8);
                restored.Load(preVersioningBlob);

                // The search state survives the upgrade, but the champion must not: its BestFitness was measured on the
                // old fitness scale, and comparing it against new-scale fitness would freeze the profile's "best" genome
                // on a policy the reshape was designed to displace.
                Assert.Equal(optimizer.Generation, restored.Generation);
                Assert.Null(restored.BestVector);
                Assert.Equal(double.MinValue, restored.BestFitness);
            }
        }
    }
}
