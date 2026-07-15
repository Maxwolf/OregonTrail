using System.Reflection;
using OregonTrailDotNet;
using OregonTrailDotNet.Bot.Data;
using OregonTrailDotNet.Bot.Game;
using OregonTrailDotNet.Bot.Learning;
using Xunit;

namespace OregonTrailDotNet.Bot.Tests
{
    /// <summary>Trains and resumes each training model against a live game + temp database.</summary>
    public sealed class ModelTrainingTests : IDisposable
    {
        private readonly string _dbPath = Path.Combine(Path.GetTempPath(), $"botmodel_{Guid.NewGuid():N}.db");

        static ModelTrainingTests() => Assembly.SetEntryAssembly(typeof(GameSimulationApp).Assembly);

        public void Dispose()
        {
            GameSimulationApp.Instance?.Destroy();
            foreach (var s in new[] { "", "-wal", "-shm", "-journal" })
                if (File.Exists(_dbPath + s)) File.Delete(_dbPath + s);
        }

        [Theory]
        [InlineData("cem")]
        [InlineData("genetic")]
        [InlineData("hillclimb")]
        [InlineData("random")]
        [InlineData("neuro")]
        [InlineData("naive")]
        public void Model_Trains_Records_And_Resumes(string key)
        {
            var config = new TrainingConfig { PopulationSize = 4, GamesPerCandidate = 2, Generations = 2 };
            var games = config.PopulationSize * config.GamesPerCandidate * config.Generations;

            using var db = new BotDatabase(_dbPath);
            var id = db.Profiles.Create("Runner", key);

            new TrainingSession(db, db.Profiles.GetById(id)!, config).Run();

            var profile = db.Profiles.GetById(id)!;
            Assert.Equal(key, profile.PolicyKind);
            Assert.Equal(games, db.Runs.CountForProfile(id));
            Assert.Equal(games, profile.TotalIterations);
            Assert.Equal(2, profile.Generations);
            Assert.NotNull(profile.LearningState);

            // Resume: a fresh session must pick up the persisted generation count and continue it.
            var resumed = new TrainingSession(db, profile, config);
            Assert.Equal(2, resumed.Optimizer.Generation);
            resumed.Run();
            Assert.Equal(4, db.Profiles.GetById(id)!.Generations);
        }

        [Fact]
        public void NeuralPolicy_Drives_Full_Games_Without_Gaps()
        {
            var model = TrainingModels.ByKey("neuro");
            var policy = model.Decode(model.InitialMean(), "Neuron (bot)");

            for (var i = 0; i < 3; i++)
            {
                using var driver = new GameDriver();
                var recognizer = new ScreenRecognizer(policy);
                driver.Boot();
                var player = new GamePlayer(driver, recognizer, policy);

                var result = player.Run();

                Assert.True(result.Outcome != GameOutcomeEnum.Aborted,
                    $"Run {i} aborted: {result.AbortReason}. Unknown forms: [{string.Join(", ", player.UnknownForms)}]");
                Assert.Empty(player.UnknownForms);
            }
        }
    }
}
