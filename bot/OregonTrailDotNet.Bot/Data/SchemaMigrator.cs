using Microsoft.Data.Sqlite;

namespace OregonTrailDotNet.Bot.Data
{
    /// <summary>
    ///     Applies ordered schema migrations, tracked by SQLite's <c>PRAGMA user_version</c>. New schema changes are appended
    ///     as additional entries in <see cref="Migrations" />; each is applied exactly once, in a transaction.
    /// </summary>
    internal static class SchemaMigrator
    {
        private static readonly string[] Migrations =
        {
            // v1 — initial schema.
            """
            CREATE TABLE profiles (
                id                INTEGER PRIMARY KEY AUTOINCREMENT,
                name              TEXT    NOT NULL UNIQUE,
                policy_kind       TEXT    NOT NULL,
                created_utc       TEXT    NOT NULL,
                updated_utc       TEXT    NOT NULL,
                total_iterations  INTEGER NOT NULL DEFAULT 0,
                generations       INTEGER NOT NULL DEFAULT 0,
                best_score        INTEGER NOT NULL DEFAULT 0,
                best_genome_json  TEXT,
                learning_state    BLOB
            );

            CREATE TABLE runs (
                id                INTEGER PRIMARY KEY AUTOINCREMENT,
                profile_id        INTEGER NOT NULL REFERENCES profiles(id) ON DELETE CASCADE,
                iteration_index   INTEGER NOT NULL,
                generation        INTEGER,
                candidate_index   INTEGER,
                timestamp_utc     TEXT    NOT NULL,
                genome_json       TEXT    NOT NULL,
                outcome           TEXT    NOT NULL,
                final_score       INTEGER NOT NULL,
                days              INTEGER NOT NULL,
                miles             INTEGER NOT NULL,
                survivors         INTEGER NOT NULL,
                cause_of_death    TEXT,
                made_top10        INTEGER NOT NULL DEFAULT 0
            );
            CREATE INDEX ix_runs_profile ON runs(profile_id, iteration_index);
            CREATE INDEX ix_runs_generation ON runs(profile_id, generation);

            CREATE TABLE leaderboard (
                id                INTEGER PRIMARY KEY AUTOINCREMENT,
                profile_id        INTEGER REFERENCES profiles(id) ON DELETE SET NULL,
                run_id            INTEGER REFERENCES runs(id) ON DELETE SET NULL,
                name              TEXT    NOT NULL,
                score             INTEGER NOT NULL,
                rating            TEXT    NOT NULL,
                timestamp_utc     TEXT    NOT NULL
            );
            CREATE INDEX ix_leaderboard_score ON leaderboard(score DESC);
            """
        };

        public static void Migrate(SqliteConnection connection)
        {
            var current = GetUserVersion(connection);

            for (var version = current; version < Migrations.Length; version++)
            {
                using var tx = connection.BeginTransaction();

                using (var cmd = connection.CreateCommand())
                {
                    cmd.Transaction = tx;
                    cmd.CommandText = Migrations[version];
                    cmd.ExecuteNonQuery();
                }

                using (var cmd = connection.CreateCommand())
                {
                    cmd.Transaction = tx;
                    cmd.CommandText = $"PRAGMA user_version = {version + 1};"; // int, safe to interpolate; pragma can't be bound
                    cmd.ExecuteNonQuery();
                }

                tx.Commit();
            }
        }

        private static int GetUserVersion(SqliteConnection connection)
        {
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "PRAGMA user_version;";
            return Convert.ToInt32(cmd.ExecuteScalar() ?? 0);
        }
    }
}
