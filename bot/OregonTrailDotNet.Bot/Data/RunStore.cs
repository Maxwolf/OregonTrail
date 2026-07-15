using Microsoft.Data.Sqlite;

namespace OregonTrailDotNet.Bot.Data
{
    /// <summary>Records individual games and summarizes training progress.</summary>
    public sealed class RunStore
    {
        private readonly SqliteConnection _connection;

        public RunStore(SqliteConnection connection) => _connection = connection;

        public long Insert(RunRecord run)
        {
            using var cmd = _connection.CreateCommand();
            cmd.CommandText = """
                INSERT INTO runs (profile_id, iteration_index, generation, candidate_index, timestamp_utc, genome_json,
                                  outcome, final_score, days, miles, survivors, cause_of_death, made_top10, fitness)
                VALUES ($pid, $iter, $gen, $cand, $ts, $genome, $outcome, $score, $days, $miles, $surv, $cause, $top10, $fitness);
                SELECT last_insert_rowid();
                """;
            cmd.Parameters.AddWithValue("$pid", run.ProfileId);
            cmd.Parameters.AddWithValue("$iter", run.IterationIndex);
            cmd.Parameters.AddWithValue("$gen", (object?) run.Generation ?? DBNull.Value);
            cmd.Parameters.AddWithValue("$cand", (object?) run.CandidateIndex ?? DBNull.Value);
            cmd.Parameters.AddWithValue("$ts", string.IsNullOrEmpty(run.TimestampUtc) ? DateTime.UtcNow.ToString("o") : run.TimestampUtc);
            cmd.Parameters.AddWithValue("$genome", run.GenomeJson);
            cmd.Parameters.AddWithValue("$outcome", run.Outcome);
            cmd.Parameters.AddWithValue("$score", run.FinalScore);
            cmd.Parameters.AddWithValue("$days", run.Days);
            cmd.Parameters.AddWithValue("$miles", run.Miles);
            cmd.Parameters.AddWithValue("$surv", run.Survivors);
            cmd.Parameters.AddWithValue("$cause", (object?) run.CauseOfDeath ?? DBNull.Value);
            cmd.Parameters.AddWithValue("$top10", run.MadeTop10 ? 1 : 0);
            cmd.Parameters.AddWithValue("$fitness", run.Fitness);
            return (long) cmd.ExecuteScalar()!;
        }

        public int NextIterationIndex(long profileId)
        {
            using var cmd = _connection.CreateCommand();
            cmd.CommandText = "SELECT COALESCE(MAX(iteration_index), -1) + 1 FROM runs WHERE profile_id = $pid;";
            cmd.Parameters.AddWithValue("$pid", profileId);
            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        public int CountForProfile(long profileId)
        {
            using var cmd = _connection.CreateCommand();
            cmd.CommandText = "SELECT COUNT(*) FROM runs WHERE profile_id = $pid;";
            cmd.Parameters.AddWithValue("$pid", profileId);
            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        public IReadOnlyList<RunRecord> RecentForProfile(long profileId, int limit)
        {
            var list = new List<RunRecord>();
            using var cmd = _connection.CreateCommand();
            cmd.CommandText = "SELECT * FROM runs WHERE profile_id = $pid ORDER BY iteration_index DESC LIMIT $lim;";
            cmd.Parameters.AddWithValue("$pid", profileId);
            cmd.Parameters.AddWithValue("$lim", limit);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
                list.Add(Map(reader));
            return list;
        }

        /// <summary>Mean and best score per completed generation, oldest first — the learning curve.</summary>
        public IReadOnlyList<GenerationStat> GenerationSummary(long profileId)
        {
            var list = new List<GenerationStat>();
            using var cmd = _connection.CreateCommand();
            cmd.CommandText = """
                SELECT generation,
                       AVG(fitness)     AS meanFitness,
                       MAX(fitness)     AS bestFitness,
                       AVG(final_score) AS meanScore,
                       MAX(final_score) AS bestScore,
                       AVG(CASE WHEN outcome = 'Win' THEN 1.0 ELSE 0.0 END) AS winRate,
                       COUNT(*)         AS games
                  FROM runs
                 WHERE profile_id = $pid AND generation IS NOT NULL
                 GROUP BY generation
                 ORDER BY generation;
                """;
            cmd.Parameters.AddWithValue("$pid", profileId);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
                list.Add(new GenerationStat
                {
                    Generation = reader.GetInt32(0),
                    MeanFitness = reader.IsDBNull(1) ? 0 : reader.GetDouble(1),
                    BestFitness = reader.IsDBNull(2) ? 0 : reader.GetDouble(2),
                    MeanScore = reader.IsDBNull(3) ? 0 : reader.GetDouble(3),
                    BestScore = reader.IsDBNull(4) ? 0 : reader.GetInt32(4),
                    WinRate = reader.IsDBNull(5) ? 0 : reader.GetDouble(5),
                    Games = reader.GetInt32(6)
                });
            return list;
        }

        private static RunRecord Map(SqliteDataReader r) => new()
        {
            Id = r.GetInt64(r.GetOrdinal("id")),
            ProfileId = r.GetInt64(r.GetOrdinal("profile_id")),
            IterationIndex = r.GetInt32(r.GetOrdinal("iteration_index")),
            Generation = r.IsDBNull(r.GetOrdinal("generation")) ? null : r.GetInt32(r.GetOrdinal("generation")),
            CandidateIndex = r.IsDBNull(r.GetOrdinal("candidate_index")) ? null : r.GetInt32(r.GetOrdinal("candidate_index")),
            TimestampUtc = r.GetString(r.GetOrdinal("timestamp_utc")),
            GenomeJson = r.GetString(r.GetOrdinal("genome_json")),
            Outcome = r.GetString(r.GetOrdinal("outcome")),
            FinalScore = r.GetInt32(r.GetOrdinal("final_score")),
            Days = r.GetInt32(r.GetOrdinal("days")),
            Miles = r.GetInt32(r.GetOrdinal("miles")),
            Survivors = r.GetInt32(r.GetOrdinal("survivors")),
            CauseOfDeath = r.IsDBNull(r.GetOrdinal("cause_of_death")) ? null : r.GetString(r.GetOrdinal("cause_of_death")),
            MadeTop10 = r.GetInt32(r.GetOrdinal("made_top10")) != 0,
            Fitness = r.IsDBNull(r.GetOrdinal("fitness")) ? 0 : r.GetDouble(r.GetOrdinal("fitness"))
        };
    }
}
