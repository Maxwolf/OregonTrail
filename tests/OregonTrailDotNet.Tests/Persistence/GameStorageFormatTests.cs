using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Data.Sqlite;
using OregonTrailDotNet.Persistence;
using Xunit;

namespace OregonTrailDotNet.Tests.Persistence
{
    /// <summary>
    ///     Freezes the on-disk wire format of game.db. The persisted contract is the SQLite column names and the primitive
    ///     values that round-trip through the stores — NOT any C# type, enum, field, or file name. That decoupling is why the
    ///     repo-wide *Enum / naming rename never touches storage: nothing serialized is derived from a renamed symbol. This
    ///     test pins that contract so a future change that DID alter a stored column name (or a stored value) fails loudly,
    ///     since it would silently break compatibility with existing players' game.db files.
    /// </summary>
    public sealed class GameStorageFormatTests : IDisposable
    {
        private readonly string _dbPath = Path.Combine(Path.GetTempPath(), $"gamefmt_{Guid.NewGuid():N}.db");

        public void Dispose()
        {
            foreach (var suffix in new[] { "", "-wal", "-shm", "-journal" })
                if (File.Exists(_dbPath + suffix))
                    File.Delete(_dbPath + suffix);
        }

        private List<string> ColumnNames(string table)
        {
            using var connection = new SqliteConnection(
                new SqliteConnectionStringBuilder { DataSource = _dbPath, Pooling = false }.ToString());
            connection.Open();
            using var cmd = connection.CreateCommand();
            cmd.CommandText = $"PRAGMA table_info({table});";
            var names = new List<string>();
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
                names.Add(reader.GetString(1)); // column 1 of PRAGMA table_info is the column name
            return names;
        }

        [Fact]
        public void HighScores_Table_Keeps_Its_Frozen_Columns()
        {
            using var db = new GameDatabase(_dbPath);
            Assert.Equal(new[] { "id", "name", "points" }, ColumnNames("high_scores"));
        }

        [Fact]
        public void Tombstones_Table_Keeps_Its_Frozen_Columns()
        {
            using var db = new GameDatabase(_dbPath);
            Assert.Equal(
                new[]
                {
                    "trail_half", "mile_marker", "player_name", "epitaph", "last_landmark", "next_landmark",
                    "miles_to_next"
                },
                ColumnNames("tombstones"));
        }

        [Fact]
        public void Stored_Values_Round_Trip_Unchanged_Across_A_Reopen()
        {
            using (var db = new GameDatabase(_dbPath))
            {
                db.HighScores.Insert("Stephen Meek", 7650);
                db.Tombstones.Insert(0, 815, "Ezra", "Here lies Ezra", "Fort Hall", "Fort Boise", 5);
            }

            using (var db = new GameDatabase(_dbPath))
            {
                var score = Assert.Single(db.HighScores.All());
                Assert.Equal(("Stephen Meek", 7650), (score.Name, score.Points));

                var grave = Assert.Single(db.Tombstones.All());
                Assert.Equal(
                    (0, 815, "Ezra", "Here lies Ezra", "Fort Hall", "Fort Boise", 5),
                    (grave.TrailHalf, grave.MileMarker, grave.PlayerName, grave.Epitaph, grave.LastLandmark,
                        grave.NextLandmark, grave.MilesToNext));
            }
        }
    }
}
