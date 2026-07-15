using System.Reflection;
using OregonTrailDotNet.Bot.Data;
using OregonTrailDotNet.Bot.Game;
using OregonTrailDotNet.Bot.Learning;
using Xunit;

namespace OregonTrailDotNet.Bot.Tests
{
    /// <summary>Exercises the CEM training loop end-to-end against a temp database and a live game.</summary>
    public sealed class TrainingTests : IDisposable
    {
        private readonly string _dbPath = Path.Combine(Path.GetTempPath(), $"bottrain_{Guid.NewGuid():N}.db");

        static TrainingTests() => Assembly.SetEntryAssembly(typeof(GameSimulationApp).Assembly);

        public void Dispose()
        {
            GameSimulationApp.Instance?.Destroy();
            foreach (var suffix in new[] { "", "-wal", "-shm", "-journal" })
                if (File.Exists(_dbPath + suffix))
                    File.Delete(_dbPath + suffix);
        }

        [Fact]
        public void Genome_Json_RoundTrips()
        {
            var genome = StrategyGenome.Default();
            var restored = StrategyGenome.FromJson(genome.ToJson());
            Assert.Equal(genome.Raw, restored.Raw);
            Assert.Equal(genome.FoodTarget, restored.FoodTarget);
        }

        [Fact]
        public void Training_Records_Runs_Persists_State_And_Resumes()
        {
            var config = new TrainingConfig { PopulationSize = 6, GamesPerCandidate = 3, Generations = 2 };
            var expectedGames = config.PopulationSize * config.GamesPerCandidate * config.Generations;

            long profileId;
            var progressCallbacks = 0;

            using (var db = new BotDatabase(_dbPath))
            {
                profileId = db.Profiles.Create("Pathfinder", "cem");
                var profile = db.Profiles.GetById(profileId)!;

                var session = new TrainingSession(db, profile, config);
                session.Run(onGeneration: _ => progressCallbacks++);

                Assert.Equal(config.Generations, progressCallbacks);
                Assert.Equal(expectedGames, db.Runs.CountForProfile(profileId));

                var updated = db.Profiles.GetById(profileId)!;
                Assert.Equal(expectedGames, updated.TotalIterations);
                Assert.Equal(2, updated.Generations);
                Assert.NotNull(updated.LearningState);

                // Two generations of runs should be summarizable for the learning curve.
                var summary = db.Runs.GenerationSummary(profileId);
                Assert.Equal(2, summary.Count);
            }

            // Reopen and resume: the optimizer must pick up the persisted generation count.
            using (var db = new BotDatabase(_dbPath))
            {
                var profile = db.Profiles.GetById(profileId)!;
                var resumed = new TrainingSession(db, profile, config);
                Assert.Equal(2, resumed.Optimizer.Generation);

                resumed.Run();

                var updated = db.Profiles.GetById(profileId)!;
                Assert.Equal(4, updated.Generations);
                Assert.Equal(expectedGames * 2, db.Runs.CountForProfile(profileId));
            }
        }

        [Fact]
        public void Negative_Generations_Trains_Open_Endedly_Until_ShouldStop()
        {
            // A negative generation count is the "train until I press Esc" mode: the loop must not run the count literally
            // (which would be zero iterations) nor loop forever — it runs until shouldStop returns true. A stubbed game runner
            // keeps it fast and deterministic; shouldStop stands in for the Esc/Ctrl+C hook and ends it after 3 generations.
            using var db = new BotDatabase(_dbPath);
            var id = db.Profiles.Create("Endless", "cem");

            var config = new TrainingConfig { PopulationSize = 4, GamesPerCandidate = 1, Generations = -1 };
            var gensSeen = 0;

            new TrainingSession(db, db.Profiles.GetById(id)!, config,
                    playGame: _ => new RunResult { Outcome = GameOutcome.Death, Miles = 100, PartySize = 5, Survivors = 0 })
                .Run(onGeneration: _ => gensSeen++, shouldStop: () => gensSeen >= 3);

            Assert.Equal(3, gensSeen);

            var profile = db.Profiles.GetById(id)!;
            Assert.Equal(3, profile.Generations);
            Assert.Equal(3 * config.PopulationSize * config.GamesPerCandidate, db.Runs.CountForProfile(id));
        }
    }
}
