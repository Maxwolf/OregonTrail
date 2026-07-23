// Created by Maxwolf (bigmaxwolf.com)

using System;
using System.IO;
using Microsoft.Data.Sqlite;

namespace OregonTrailDotNet.Persistence
{
    /// <summary>
    ///     The player's persistent game database — a single SQLite file (game.db) that sits next to the executable and holds
    ///     the high-score list and the tombstones left on the trail, so both survive across runs. Mirrors the bot's own
    ///     database design (single long-lived connection, WAL, hand-written stores). The file can simply be deleted by the
    ///     player to wipe it, or reset from the in-game management options.
    /// </summary>
    public sealed class GameDatabase : IDisposable
    {
        /// <summary>Environment variable that overrides the database file location (used by tests to point at a temp file).</summary>
        public const string PathEnvVar = "OREGONTRAIL_GAME_DB";

        private readonly SqliteConnection _connection;

        public GameDatabase(string path = null)
        {
            Path = path ?? ResolveDefaultPath();

            var dir = System.IO.Path.GetDirectoryName(Path);
            if (!string.IsNullOrEmpty(dir))
                Directory.CreateDirectory(dir);

            // Pooling disabled so the file handle and WAL sidecars release cleanly when we dispose.
            _connection = new SqliteConnection(new SqliteConnectionStringBuilder
            {
                DataSource = Path,
                Pooling = false
            }.ToString());
            _connection.Open();

            Execute("PRAGMA journal_mode=WAL;");
            Execute("PRAGMA foreign_keys=ON;");

            GameSchemaMigrator.Migrate(_connection);

            HighScores = new HighScoreStore(_connection);
            Tombstones = new TombstoneStore(_connection);
        }

        public string Path { get; }
        public HighScoreStore HighScores { get; }
        public TombstoneStore Tombstones { get; }

        /// <summary>Opens the default database, returning null (persistence simply off) rather than crashing the game if the
        ///     file can't be opened — e.g. it is locked, read-only, or corrupt.</summary>
        public static GameDatabase TryOpen()
        {
            try
            {
                return new GameDatabase();
            }
            catch
            {
                return null;
            }
        }

        /// <summary>The database file path: an env-var override if set, otherwise game.db next to the executable.</summary>
        public static string ResolveDefaultPath()
        {
            var overridePath = Environment.GetEnvironmentVariable(PathEnvVar);
            if (!string.IsNullOrWhiteSpace(overridePath))
                return overridePath;

            return System.IO.Path.Combine(AppContext.BaseDirectory, "game.db");
        }

        private void Execute(string sql)
        {
            using var cmd = _connection.CreateCommand();
            cmd.CommandText = sql;
            cmd.ExecuteNonQuery();
        }

        public void Dispose()
        {
            _connection.Dispose();
        }
    }
}
