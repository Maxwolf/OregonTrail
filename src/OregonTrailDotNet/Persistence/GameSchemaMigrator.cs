// Created by Maxwolf (bigmaxwolf.com)

using System;
using Microsoft.Data.Sqlite;

namespace OregonTrailDotNet.Persistence
{
    /// <summary>
    ///     Creates and upgrades the game.db schema. Versioning uses SQLite's built-in <c>PRAGMA user_version</c>: each entry in
    ///     <see cref="Migrations" /> is one schema version, applied in order inside a transaction together with its version
    ///     bump, so a fresh database runs them all and an existing one only runs the entries it hasn't seen. Append a new
    ///     migration string to add tables/columns later; never edit an already-shipped one.
    /// </summary>
    internal static class GameSchemaMigrator
    {
        private static readonly string[] Migrations =
        {
            // v1 — high scores and tombstones. Tombstones are two-per-trail (one per half, keyed by trail_half so a new
            //      death in a half overwrites the old grave) and record the bracketing landmarks the original TOMBS.REC
            //      stored — matching where the party died.
            """
            CREATE TABLE high_scores (
                id      INTEGER PRIMARY KEY AUTOINCREMENT,
                name    TEXT    NOT NULL,
                points  INTEGER NOT NULL
            );
            CREATE INDEX ix_high_scores_points ON high_scores(points DESC);

            CREATE TABLE tombstones (
                trail_half     INTEGER PRIMARY KEY,
                mile_marker    INTEGER NOT NULL,
                player_name    TEXT    NOT NULL,
                epitaph        TEXT    NOT NULL DEFAULT '',
                last_landmark  TEXT    NOT NULL DEFAULT '',
                next_landmark  TEXT    NOT NULL DEFAULT '',
                miles_to_next  INTEGER NOT NULL DEFAULT 0
            );
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
                    // PRAGMA arguments cannot be parameter-bound; safe here since it is an int we control.
                    cmd.CommandText = $"PRAGMA user_version = {version + 1};";
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
