using Microsoft.Data.Sqlite;

namespace OregonTrailDotNet.Bot.Data
{
    /// <summary>CRUD for bot profiles (the save files).</summary>
    public sealed class ProfileStore
    {
        private readonly SqliteConnection _connection;

        public ProfileStore(SqliteConnection connection) => _connection = connection;

        public long Create(string name, string policyKind)
        {
            var now = Now();
            using var cmd = _connection.CreateCommand();
            cmd.CommandText = """
                INSERT INTO profiles (name, policy_kind, created_utc, updated_utc)
                VALUES ($name, $kind, $now, $now);
                SELECT last_insert_rowid();
                """;
            cmd.Parameters.AddWithValue("$name", name);
            cmd.Parameters.AddWithValue("$kind", policyKind);
            cmd.Parameters.AddWithValue("$now", now);
            return (long) cmd.ExecuteScalar()!;
        }

        public bool NameExists(string name)
        {
            using var cmd = _connection.CreateCommand();
            cmd.CommandText = "SELECT 1 FROM profiles WHERE name = $name LIMIT 1;";
            cmd.Parameters.AddWithValue("$name", name);
            return cmd.ExecuteScalar() != null;
        }

        public ProfileRecord? GetById(long id) => QuerySingle("SELECT * FROM profiles WHERE id = $v;", "$v", id);
        public ProfileRecord? GetByName(string name) => QuerySingle("SELECT * FROM profiles WHERE name = $v;", "$v", name);

        public IReadOnlyList<ProfileRecord> All()
        {
            var list = new List<ProfileRecord>();
            using var cmd = _connection.CreateCommand();
            cmd.CommandText = "SELECT * FROM profiles ORDER BY name;";
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
                list.Add(Map(reader));
            return list;
        }

        /// <summary>Persists the policy's serialized learning state plus the headline progress figures.</summary>
        public void SaveLearningState(long id, byte[] learningState, int generations, int bestScore, string? bestGenomeJson)
        {
            using var cmd = _connection.CreateCommand();
            cmd.CommandText = """
                UPDATE profiles
                   SET learning_state = $state, generations = $gens, best_score = $best,
                       best_genome_json = $genome, updated_utc = $now
                 WHERE id = $id;
                """;
            cmd.Parameters.AddWithValue("$state", (object?) learningState ?? DBNull.Value);
            cmd.Parameters.AddWithValue("$gens", generations);
            cmd.Parameters.AddWithValue("$best", bestScore);
            cmd.Parameters.AddWithValue("$genome", (object?) bestGenomeJson ?? DBNull.Value);
            cmd.Parameters.AddWithValue("$now", Now());
            cmd.Parameters.AddWithValue("$id", id);
            cmd.ExecuteNonQuery();
        }

        public void AddIterations(long id, int count)
        {
            using var cmd = _connection.CreateCommand();
            cmd.CommandText = """
                UPDATE profiles SET total_iterations = total_iterations + $n, updated_utc = $now WHERE id = $id;
                """;
            cmd.Parameters.AddWithValue("$n", count);
            cmd.Parameters.AddWithValue("$now", Now());
            cmd.Parameters.AddWithValue("$id", id);
            cmd.ExecuteNonQuery();
        }

        public void Delete(long id)
        {
            using var cmd = _connection.CreateCommand();
            cmd.CommandText = "DELETE FROM profiles WHERE id = $id;";
            cmd.Parameters.AddWithValue("$id", id);
            cmd.ExecuteNonQuery();
        }

        /// <summary>Removes every profile (and, via ON DELETE CASCADE, all of their runs).</summary>
        public void DeleteAll()
        {
            using var cmd = _connection.CreateCommand();
            cmd.CommandText = "DELETE FROM profiles;";
            cmd.ExecuteNonQuery();
        }

        private ProfileRecord? QuerySingle(string sql, string paramName, object value)
        {
            using var cmd = _connection.CreateCommand();
            cmd.CommandText = sql;
            cmd.Parameters.AddWithValue(paramName, value);
            using var reader = cmd.ExecuteReader();
            return reader.Read() ? Map(reader) : null;
        }

        private static ProfileRecord Map(SqliteDataReader r) => new()
        {
            Id = r.GetInt64(r.GetOrdinal("id")),
            Name = r.GetString(r.GetOrdinal("name")),
            PolicyKind = r.GetString(r.GetOrdinal("policy_kind")),
            CreatedUtc = r.GetString(r.GetOrdinal("created_utc")),
            UpdatedUtc = r.GetString(r.GetOrdinal("updated_utc")),
            TotalIterations = r.GetInt32(r.GetOrdinal("total_iterations")),
            Generations = r.GetInt32(r.GetOrdinal("generations")),
            BestScore = r.GetInt32(r.GetOrdinal("best_score")),
            BestGenomeJson = r.IsDBNull(r.GetOrdinal("best_genome_json")) ? null : r.GetString(r.GetOrdinal("best_genome_json")),
            LearningState = r.IsDBNull(r.GetOrdinal("learning_state")) ? null : (byte[]) r["learning_state"]
        };

        private static string Now() => DateTime.UtcNow.ToString("o");
    }
}
