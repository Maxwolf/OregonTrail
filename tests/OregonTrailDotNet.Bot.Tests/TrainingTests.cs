using System.Reflection;
using OregonTrailDotNet.Bot.Data;
using OregonTrailDotNet.Bot.Game;
using OregonTrailDotNet.Bot.Learning;
using OregonTrailDotNet.Entity.Person;
using OregonTrailDotNet.Entity.Vehicle;
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
        public void Genome_FromJson_Tolerates_An_OldLength_Vector()
        {
            // A genome serialized under the old 24-gene layout (before pace/ration genes) must load without throwing: the
            // missing genes zero-fill and the new decoders clamp into range. Guards replay of any stale best-genome JSON.
            var oldLayout = new double[24];
            var genome = StrategyGenome.FromJson(System.Text.Json.JsonSerializer.Serialize(oldLayout));

            Assert.Equal(StrategyGenome.Length, genome.Raw.Length);
            Assert.InRange(genome.PaceChoice, 1, 3);
            Assert.InRange(genome.RationChoice, 1, 3);
        }

        [Fact]
        public void Genome_Decodes_Pace_And_Ration_And_WarmStart_Matches_The_Old_Hardcoding()
        {
            // The warm-start prior must still play the previous hardcoded tactics (Grueling pace, Filling rations), so
            // un-hardcoding them changes nothing at generation 0 and only opens the door for the optimizer to explore.
            var expert = StrategyGenome.Default();
            Assert.Equal(3, expert.PaceChoice);                       // Grueling (menu 3)
            Assert.Equal(TravelPace.Grueling, expert.DesiredPace);
            Assert.Equal(1, expert.RationChoice);                     // Filling (menu 1)
            Assert.Equal(RationLevel.Filling, expert.DesiredRation);

            // Out-of-range raw gene values clamp into the legal 1..3 menu range (indices 24/25 = pace/ration).
            var raw = (double[]) StrategyGenome.DefaultMean().Clone();
            raw[24] = 99;
            raw[25] = -99;
            var wild = new StrategyGenome { Raw = raw };
            Assert.Equal(3, wild.PaceChoice);                          // 99 clamps to 3
            Assert.Equal(1, wild.RationChoice);                        // -99 clamps to 1
            Assert.Equal(RationLevel.Filling, wild.DesiredRation);     // menu 1 -> Filling
        }

        [Fact]
        public void Generation_Uses_Shared_Seeds_And_Varies_Across_Generations()
        {
            const int k = 4, gens = 2;
            var seen = new List<int?>();

            using var db = new BotDatabase(_dbPath);
            var profile = db.Profiles.GetById(db.Profiles.Create("Paired", "cem"))!;
            var config = new TrainingConfig { PopulationSize = 5, GamesPerCandidate = k, Generations = gens };

            new TrainingSession(db, profile, config,
                    playGame: (_, seed) =>
                    {
                        seen.Add(seed);
                        return new RunResult { Outcome = GameOutcome.Death, PartySize = 5 };
                    })
                .Run();

            // The optimizer may clamp the population, so derive the actual per-generation candidate count from the calls.
            Assert.True(seen.Count % (k * gens) == 0, $"unexpected call count {seen.Count}");
            var pop = seen.Count / (k * gens);
            Assert.True(pop >= 2);

            // Within a generation every candidate must replay the SAME K-seed set (common random numbers).
            var gen0 = seen.Take(pop * k).ToList();
            var gen1 = seen.Skip(pop * k).Take(pop * k).ToList();
            var gen0Set = gen0.Take(k).ToList();
            var gen1Set = gen1.Take(k).ToList();
            for (var c = 0; c < pop; c++)
            {
                Assert.Equal(gen0Set, gen0.Skip(c * k).Take(k).ToList());
                Assert.Equal(gen1Set, gen1.Skip(c * k).Take(k).ToList());
            }

            // ...but different generations use different seed sets, so candidates aren't overfit to one fixed batch of games.
            Assert.NotEqual(gen0Set, gen1Set);
        }

        [Fact]
        public void SameEvaluationSeed_Reproduces_The_Generation_Seeds()
        {
            using var db = new BotDatabase(_dbPath);
            var config = new TrainingConfig { PopulationSize = 2, GamesPerCandidate = 3, Generations = 2, EvaluationSeed = 999 };

            List<int?> RunOnce(string name)
            {
                var seeds = new List<int?>();
                var profile = db.Profiles.GetById(db.Profiles.Create(name, "cem"))!;
                new TrainingSession(db, profile, config,
                        playGame: (_, seed) =>
                        {
                            seeds.Add(seed);
                            return new RunResult { Outcome = GameOutcome.Death, PartySize = 5 };
                        })
                    .Run();
                return seeds;
            }

            Assert.Equal(RunOnce("A"), RunOnce("B"));
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
                    playGame: (_, _) => new RunResult { Outcome = GameOutcome.Death, Miles = 100, PartySize = 5, Survivors = 0 })
                .Run(onGeneration: _ => gensSeen++, shouldStop: () => gensSeen >= 3);

            Assert.Equal(3, gensSeen);

            var profile = db.Profiles.GetById(id)!;
            Assert.Equal(3, profile.Generations);
            Assert.Equal(3 * config.PopulationSize * config.GamesPerCandidate, db.Runs.CountForProfile(id));
        }
    }
}
