using Microsoft.Data.Sqlite;

namespace OregonTrailDotNet.Bot.Data
{
    /// <summary>
    ///     The bot's persistent high-score list. Unlike the game's in-memory top ten (which resets every process), this one
    ///     accumulates across all training sessions and profiles, and is the reference for "would this run make the top 10?".
    /// </summary>
    public sealed class LeaderboardStore
    {
        private readonly SqliteConnection _connection;

        public LeaderboardStore(SqliteConnection connection) => _connection = connection;

        public long Insert(LeaderboardEntry entry)
        {
            using var cmd = _connection.CreateCommand();
            cmd.CommandText = """
                INSERT INTO leaderboard (profile_id, run_id, name, score, rating, timestamp_utc)
                VALUES ($pid, $rid, $name, $score, $rating, $ts);
                SELECT last_insert_rowid();
                """;
            cmd.Parameters.AddWithValue("$pid", (object?) entry.ProfileId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("$rid", (object?) entry.RunId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("$name", entry.Name);
            cmd.Parameters.AddWithValue("$score", entry.Score);
            cmd.Parameters.AddWithValue("$rating", entry.Rating);
            cmd.Parameters.AddWithValue("$ts", string.IsNullOrEmpty(entry.TimestampUtc) ? DateTime.UtcNow.ToString("o") : entry.TimestampUtc);
            return (long) cmd.ExecuteScalar()!;
        }

        public IReadOnlyList<LeaderboardEntry> Top(int n)
        {
            var list = new List<LeaderboardEntry>();
            using var cmd = _connection.CreateCommand();
            cmd.CommandText = "SELECT * FROM leaderboard ORDER BY score DESC, id ASC LIMIT $n;";
            cmd.Parameters.AddWithValue("$n", n);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
                list.Add(Map(reader));
            return list;
        }

        public int Count()
        {
            using var cmd = _connection.CreateCommand();
            cmd.CommandText = "SELECT COUNT(*) FROM leaderboard;";
            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        /// <summary>The score currently sitting in 10th place, or 0 if fewer than 10 entries exist (so anything qualifies).</summary>
        public int TenthPlaceScore()
        {
            using var cmd = _connection.CreateCommand();
            cmd.CommandText = "SELECT score FROM leaderboard ORDER BY score DESC, id ASC LIMIT 1 OFFSET 9;";
            var result = cmd.ExecuteScalar();
            return result == null ? 0 : Convert.ToInt32(result);
        }

        private static LeaderboardEntry Map(SqliteDataReader r) => new()
        {
            Id = r.GetInt64(r.GetOrdinal("id")),
            ProfileId = r.IsDBNull(r.GetOrdinal("profile_id")) ? null : r.GetInt64(r.GetOrdinal("profile_id")),
            RunId = r.IsDBNull(r.GetOrdinal("run_id")) ? null : r.GetInt64(r.GetOrdinal("run_id")),
            Name = r.GetString(r.GetOrdinal("name")),
            Score = r.GetInt32(r.GetOrdinal("score")),
            Rating = r.GetString(r.GetOrdinal("rating")),
            TimestampUtc = r.GetString(r.GetOrdinal("timestamp_utc"))
        };
    }
}
