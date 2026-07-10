using OregonTrailDotNet.Bot.Data;
using Xunit;

namespace OregonTrailDotNet.Bot.Tests
{
    /// <summary>Round-trips the SQLite schema and stores against a throwaway database file.</summary>
    public sealed class DatabaseTests : IDisposable
    {
        private readonly string _dbPath = Path.Combine(Path.GetTempPath(), $"bottest_{Guid.NewGuid():N}.db");

        public void Dispose()
        {
            foreach (var suffix in new[] { "", "-wal", "-shm", "-journal" })
            {
                var p = _dbPath + suffix;
                if (File.Exists(p))
                    File.Delete(p);
            }
        }

        [Fact]
        public void Migrator_Creates_Schema_And_Is_Idempotent()
        {
            using (new BotDatabase(_dbPath)) { }
            // Opening again must not re-run migrations or throw.
            using var db = new BotDatabase(_dbPath);
            Assert.Empty(db.Profiles.All());
        }

        [Fact]
        public void Profile_Run_Leaderboard_RoundTrip()
        {
            using var db = new BotDatabase(_dbPath);

            var id = db.Profiles.Create("Explorer", "cem");
            Assert.True(id > 0);
            Assert.True(db.Profiles.NameExists("Explorer"));
            Assert.Equal(0, db.Runs.NextIterationIndex(id));

            var runId = db.Runs.Insert(new RunRecord
            {
                ProfileId = id,
                IterationIndex = db.Runs.NextIterationIndex(id),
                Generation = 0,
                CandidateIndex = 2,
                GenomeJson = "{\"profession\":3}",
                Outcome = "Win",
                FinalScore = 1843,
                Days = 120,
                Miles = 2100,
                Survivors = 4,
                MadeTop10 = true
            });
            db.Profiles.AddIterations(id, 1);

            Assert.Equal(1, db.Runs.NextIterationIndex(id));
            Assert.Equal(1, db.Runs.CountForProfile(id));
            Assert.Equal(1843, db.Runs.RecentForProfile(id, 10).Single().FinalScore);

            db.Profiles.SaveLearningState(id, new byte[] { 1, 2, 3, 4 }, generations: 1, bestScore: 1843, bestGenomeJson: "{}");

            var profile = db.Profiles.GetById(id)!;
            Assert.Equal(1, profile.TotalIterations);
            Assert.Equal(1, profile.Generations);
            Assert.Equal(1843, profile.BestScore);
            Assert.Equal(new byte[] { 1, 2, 3, 4 }, profile.LearningState);

            db.Leaderboard.Insert(new LeaderboardEntry
            {
                ProfileId = id, RunId = runId, Name = "Trailblazer (bot)", Score = 1843, Rating = "Adventurer"
            });

            Assert.Equal(1, db.Leaderboard.Count());
            Assert.Equal("Trailblazer (bot)", db.Leaderboard.Top(10).Single().Name);
            Assert.Equal(0, db.Leaderboard.TenthPlaceScore()); // fewer than 10 entries -> anything qualifies
        }

        [Fact]
        public void Deleting_A_Profile_Cascades_To_Its_Runs()
        {
            using var db = new BotDatabase(_dbPath);
            var id = db.Profiles.Create("Doomed", "heuristic");
            db.Runs.Insert(new RunRecord { ProfileId = id, IterationIndex = 0, GenomeJson = "{}", Outcome = "Death" });

            db.Profiles.Delete(id);

            Assert.Empty(db.Profiles.All());
            Assert.Equal(0, db.Runs.CountForProfile(id));
        }

        [Fact]
        public void Delete_And_Clear_Helpers_Wipe_The_Right_Rows()
        {
            using var db = new BotDatabase(_dbPath);
            var a = db.Profiles.Create("A", "cem");
            var b = db.Profiles.Create("B", "cem");
            db.Runs.Insert(new RunRecord { ProfileId = a, IterationIndex = 0, GenomeJson = "{}", Outcome = "Win" });
            db.Runs.Insert(new RunRecord { ProfileId = b, IterationIndex = 0, GenomeJson = "{}", Outcome = "Win" });
            db.Leaderboard.Insert(new LeaderboardEntry { ProfileId = a, Name = "A", Score = 100, Rating = "Greenhorn" });
            db.Leaderboard.Insert(new LeaderboardEntry { ProfileId = b, Name = "B", Score = 200, Rating = "Greenhorn" });

            // DeleteForProfile removes only A's leaderboard entry.
            db.Leaderboard.DeleteForProfile(a);
            Assert.Equal(1, db.Leaderboard.Count());

            // Deleting profile A cascades its runs but leaves B's.
            db.Profiles.Delete(a);
            Assert.Equal(0, db.Runs.CountForProfile(a));
            Assert.Equal(1, db.Runs.CountForProfile(b));

            // Clear + DeleteAll wipe everything that's left.
            db.Leaderboard.Clear();
            db.Profiles.DeleteAll();
            Assert.Empty(db.Profiles.All());
            Assert.Equal(0, db.Leaderboard.Count());
            Assert.Equal(0, db.Runs.CountForProfile(b));
        }
    }
}
