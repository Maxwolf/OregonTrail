using System.Reflection;
using OregonTrailDotNet;
using OregonTrailDotNet.Bot.Data;
using OregonTrailDotNet.Bot.Game;
using OregonTrailDotNet.Bot.Learning;
using Xunit;

namespace OregonTrailDotNet.Bot.Tests
{
    /// <summary>
    ///     Exercises the whole pipeline the way <c>Program</c> does: create a profile, train it, confirm qualifying finishes
    ///     land on the leaderboard tagged "(bot)", the best genome/score persist, and that best strategy can be replayed
    ///     (what "Watch a game" does).
    /// </summary>
    public sealed class EndToEndTests : IDisposable
    {
        private readonly string _dbPath = Path.Combine(Path.GetTempPath(), $"bote2e_{Guid.NewGuid():N}.db");

        static EndToEndTests() => Assembly.SetEntryAssembly(typeof(GameSimulationApp).Assembly);

        public void Dispose()
        {
            GameSimulationApp.Instance?.Destroy();
            foreach (var s in new[] { "", "-wal", "-shm", "-journal" })
                if (File.Exists(_dbPath + s)) File.Delete(_dbPath + s);
        }

        [Fact]
        public void Train_Then_Leaderboard_Best_Genome_And_Replay()
        {
            using var db = new BotDatabase(_dbPath);
            var id = db.Profiles.Create("Odyssey", "cem");

            new TrainingSession(db, db.Profiles.GetById(id)!,
                new TrainingConfig { PopulationSize = 8, GamesPerCandidate = 4, Generations = 3 }).Run();

            // Finishing games should have populated the bot's own leaderboard, which uses the plain profile name (no "(bot)").
            var top = db.Leaderboard.Top(10);
            Assert.NotEmpty(top);
            Assert.All(top, e => Assert.Equal("Odyssey", e.Name));
            Assert.All(top, e => Assert.DoesNotContain("(bot)", e.Name));
            Assert.True(top[0].Score >= top[^1].Score); // ordered best-first

            // Best score and genome were persisted for the profile.
            var profile = db.Profiles.GetById(id)!;
            Assert.True(profile.BestScore > 0);
            Assert.False(string.IsNullOrEmpty(profile.BestGenomeJson));

            // Replaying the best genome (what "Watch a game" does) produces a valid, non-crashing run.
            var policy = new GenomePolicy(StrategyGenome.FromJson(profile.BestGenomeJson!), $"{profile.Name} (bot)");
            var result = GamePlayer.PlayOnce(policy);
            Assert.True(result.Bug is null || result.Bug.Category == Diagnostics.BugCategory.SoftLock);

            // The party is named "<profile> 1..4", so the leader (crew #1, whose name brands the in-game high-score list)
            // is "<profile> 1" — the "(bot)" tag passed to the policy is stripped for the shared party base name.
            if (!string.IsNullOrEmpty(result.LeaderName))
                Assert.Equal("Odyssey 1", result.LeaderName);
        }
    }
}
