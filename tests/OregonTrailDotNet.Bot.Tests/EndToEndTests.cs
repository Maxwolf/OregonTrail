using OregonTrailDotNet.Bot.Data;
using OregonTrailDotNet.Bot.Game;
using OregonTrailDotNet.Bot.Learning;
using Xunit;

namespace OregonTrailDotNet.Bot.Tests
{
    /// <summary>
    ///     Exercises the whole pipeline the way <c>Program</c> does: create a profile, train it, confirm qualifying finishes
    ///     land on the leaderboard tagged with the plain profile name, the best genome/score persist, and that best strategy
    ///     can be replayed (what "Watch a game" does).
    /// </summary>
    public sealed class EndToEndTests : IDisposable
    {
        private readonly string _dbPath = Path.Combine(Path.GetTempPath(), $"bote2e_{Guid.NewGuid():N}.db");


        public void Dispose()
        {
            GameSimulationApp.Instance?.Destroy();
            foreach (var s in new[] { "", "-wal", "-shm", "-journal" })
                if (File.Exists(_dbPath + s)) File.Delete(_dbPath + s);
        }

        // A guaranteed qualifying finish: a full, healthy five-person Farmer party arriving in Oregon. Used to drive the
        // record/leaderboard/persistence pipeline deterministically, since a real driven game reaches Oregon only rarely
        // (most parties die on the trail) — so asserting on a random training finish is inherently flaky.
        private static RunResult WinningRun() => new()
        {
            Outcome = GameOutcomeEnum.Win,
            Score = 5000,
            Days = 150,
            Miles = 2000,
            Survivors = 5,
            PartySize = 5,
            PartyHealthValue = 500,
            LeaderName = "Odyssey 1"
        };

        [Fact]
        public void Training_Drives_Real_Games_And_Persists_Learning_State()
        {
            using var db = new BotDatabase(_dbPath);
            var id = db.Profiles.Create("Odyssey", "cem");

            var config = new TrainingConfig { PopulationSize = 6, GamesPerCandidate = 2, Generations = 2 };
            var expectedGames = config.PopulationSize * config.GamesPerCandidate * config.Generations;

            // No injected runner: this drives real games end-to-end through the recognizer/policy stack.
            new TrainingSession(db, db.Profiles.GetById(id)!, config).Run();

            // Every game is recorded and the optimizer's learning state advances and persists — all independent of whether any
            // single random game happened to finish, so these hold on every run.
            var profile = db.Profiles.GetById(id)!;
            Assert.Equal(expectedGames, db.Runs.CountForProfile(id));
            Assert.Equal(expectedGames, profile.TotalIterations);
            Assert.Equal(config.Generations, profile.Generations);
            Assert.NotNull(profile.LearningState);

            // The persisted best genome is now the optimizer's ROBUST champion (its best vector by shaped fitness), saved after
            // every generation independent of whether any single random game happened to score - so it is always available to
            // replay once training has run, even when no game finished with points. (Best RAW score is tracked separately.)
            Assert.False(string.IsNullOrEmpty(profile.BestGenomeJson));
        }

        [Fact]
        public void Qualifying_Finish_Lands_On_Leaderboard_And_Best_Genome_Replays()
        {
            using var db = new BotDatabase(_dbPath);
            var id = db.Profiles.Create("Odyssey", "cem");

            // Drive the pipeline with guaranteed finishes so the leaderboard/best-genome path is exercised deterministically.
            // The genomes recorded are still the optimizer's real candidate vectors — only the game outcome is controlled.
            new TrainingSession(db, db.Profiles.GetById(id)!,
                new TrainingConfig { PopulationSize = 8, GamesPerCandidate = 2, Generations = 2 },
                playGame: (_, _) => WinningRun()).Run();

            // Finishing games populate the bot's own leaderboard, which uses the plain profile name (no "(bot)").
            var top = db.Leaderboard.Top(10);
            Assert.NotEmpty(top);
            Assert.All(top, e => Assert.Equal("Odyssey", e.Name));
            Assert.All(top, e => Assert.DoesNotContain("(bot)", e.Name));
            Assert.True(top[0].Score >= top[^1].Score); // ordered best-first

            // Best score and genome were persisted for the profile.
            var profile = db.Profiles.GetById(id)!;
            Assert.Equal(5000, profile.BestScore);
            Assert.False(string.IsNullOrEmpty(profile.BestGenomeJson));

            // Replaying the best genome (what "Watch a game" does) produces a valid, non-crashing run (a rare soft-lock is
            // tolerated the same way training tolerates it — it is not a crash/bug).
            var policy = new GenomePolicy(StrategyGenome.FromJson(profile.BestGenomeJson!), $"{profile.Name} (bot)");
            var result = GamePlayer.PlayOnce(policy);
            Assert.True(result.Bug is null || result.Bug.Category == Diagnostics.BugCategoryEnum.SoftLock);

            // The party is named "<profile> 1..4", so the leader (crew #1, whose name brands the in-game high-score list)
            // is "<profile> 1" — the "(bot)" tag passed to the policy is stripped for the shared party base name.
            if (!string.IsNullOrEmpty(result.LeaderName))
                Assert.Equal("Odyssey 1", result.LeaderName);
        }
    }
}
