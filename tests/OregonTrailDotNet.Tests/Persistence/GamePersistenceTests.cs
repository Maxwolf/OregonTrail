using System;
using System.IO;
using System.Linq;
using OregonTrailDotNet.Module.Scoring;
using OregonTrailDotNet.Module.Tombstone;
using OregonTrailDotNet.Persistence;
using Xunit;

namespace OregonTrailDotNet.Tests.Persistence
{
    /// <summary>
    ///     Covers the game.db persistence layer: the high-score and tombstone stores, schema migration idempotency, and the
    ///     scoring/tombstone modules loading and saving through it. Each test uses its own temp database file (passed
    ///     explicitly, so nothing touches the real game.db) and deletes it — and its WAL sidecars — on dispose.
    /// </summary>
    public sealed class GamePersistenceTests : IDisposable
    {
        private readonly string _dbPath = Path.Combine(Path.GetTempPath(), $"gamedb_{Guid.NewGuid():N}.db");

        public void Dispose()
        {
            foreach (var suffix in new[] { "", "-wal", "-shm", "-journal" })
                if (File.Exists(_dbPath + suffix))
                    File.Delete(_dbPath + suffix);
        }

        [Fact]
        public void HighScoreStore_Persists_Ranks_And_Clears()
        {
            using (var db = new GameDatabase(_dbPath))
            {
                db.HighScores.Insert("Alice", 1200);
                db.HighScores.Insert("Bob", 9000);
                db.HighScores.Insert("Cara", 300);
                Assert.Equal(3, db.HighScores.Count());
            }

            // Reopen the same file: the rows survive and come back ranked best-first.
            using (var db = new GameDatabase(_dbPath))
            {
                var all = db.HighScores.All();
                Assert.Equal(new[] { "Bob", "Alice", "Cara" }, all.Select(s => s.Name));
                Assert.Equal(9000, all[0].Points);

                db.HighScores.Clear();
                Assert.Equal(0, db.HighScores.Count());
                Assert.Empty(db.HighScores.All());
            }
        }

        [Fact]
        public void TombstoneStore_Holds_Two_Graves_One_Per_Half_And_Overwrites()
        {
            using (var db = new GameDatabase(_dbPath))
            {
                // First-half grave, then a second death in the same half overwrites it (INSERT OR REPLACE on trail_half).
                db.Tombstones.Insert(0, 500, "Alice", "Watch the river", "Fort Kearney", "Chimney Rock", 40);
                db.Tombstones.Insert(0, 620, "Bob", "Overwrote Alice", "Chimney Rock", "Fort Laramie", 15);

                // A second-half grave lives alongside the first-half one — two graves total.
                db.Tombstones.Insert(1, 1200, "Cara", "", "Fort Hall", "Fort Boise", 5);
            }

            using (var db = new GameDatabase(_dbPath))
            {
                var all = db.Tombstones.All().OrderBy(t => t.TrailHalf).ToList();
                Assert.Equal(2, all.Count);                         // only two graves, one per half of the trail
                Assert.Equal("Bob", all[0].PlayerName);             // second death in half 0 replaced the first
                Assert.Equal("Overwrote Alice", all[0].Epitaph);
                Assert.Equal(620, all[0].MileMarker);
                Assert.Equal("Chimney Rock", all[0].LastLandmark);
                Assert.Equal("Fort Laramie", all[0].NextLandmark);
                Assert.Equal(15, all[0].MilesToNext);
                Assert.Equal("Cara", all[1].PlayerName);            // the half-1 grave is untouched
                Assert.Equal(1200, all[1].MileMarker);

                db.Tombstones.Clear();
                Assert.Empty(db.Tombstones.All());
            }
        }

        [Fact]
        public void Reopening_An_Existing_Database_Does_Not_Re_Run_Migrations()
        {
            // First open creates the schema; second open must be a clean no-op (user_version already current).
            using (var db = new GameDatabase(_dbPath))
                db.HighScores.Insert("Seed", 42);

            using (var db = new GameDatabase(_dbPath))
                Assert.Equal(1, db.HighScores.Count()); // data intact, no CREATE TABLE re-run error
        }

        [Fact]
        public void ScoringModule_Saves_New_Scores_And_Reloads_Them()
        {
            using var db = new GameDatabase(_dbPath);

            var first = new ScoringModule(db.HighScores);
            first.Add(new Highscore("Trailblazer", 8000));

            // A fresh module over the same store rehydrates the earned score, ranked above the seeded defaults.
            var reloaded = new ScoringModule(db.HighScores);
            var top = reloaded.TopTen.ToList();
            Assert.Equal("Trailblazer", top.First().Name);
            Assert.Equal(8000, top.First().Points);

            // Reset wipes earned scores from the database and returns the list to the original defaults.
            reloaded.Reset();
            Assert.Equal(0, db.HighScores.Count());
            Assert.Equal(ScoringModule.DefaultTopTen.Select(s => s.Points),
                reloaded.TopTen.Select(s => s.Points));
        }

        [Fact]
        public void TombstoneModule_Saves_And_Reloads_Graves()
        {
            using var db = new GameDatabase(_dbPath);

            var first = new TombstoneModule(db.Tombstones);
            first.Add(new Tombstone(0, 640, "Ezra", "Ate bad berries", "Fort Kearney", "Chimney Rock", 40));

            // A fresh module over the same store loads the saved grave at its mile marker.
            var reloaded = new TombstoneModule(db.Tombstones);
            Assert.True(reloaded.ContainsTombstone(640));
            reloaded.FindTombstone(640, out var grave);
            Assert.NotNull(grave);
            Assert.Equal("Ezra", grave.PlayerName);
            Assert.Equal("Ate bad berries", grave.Epitaph);

            reloaded.Reset();
            Assert.False(reloaded.ContainsTombstone(640));
            Assert.Empty(db.Tombstones.All());
        }

        [Fact]
        public void Modules_Without_A_Store_Stay_In_Memory()
        {
            // The null-store path (bot/tests default) must not throw and must not create any database file.
            var scoring = new ScoringModule();
            scoring.Add(new Highscore("Ghost", 5000));
            Assert.Contains(scoring.TopTen, s => s.Name == "Ghost");

            var tombstones = new TombstoneModule();
            tombstones.Add(new Tombstone(0, 100, "Ghost", "", "Independence", "Kansas River Crossing", 20));
            Assert.True(tombstones.ContainsTombstone(100));

            Assert.False(File.Exists(_dbPath));
        }
    }
}
