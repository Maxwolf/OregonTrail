using Microsoft.Data.Sqlite;

namespace OregonTrailDotNet.Bot.Data
{
    /// <summary>
    ///     Owns the single SQLite connection for a session and exposes the typed stores. The database file lives next to the
    ///     running application (<c>bot.db</c>), overridable via the <c>OREGONTRAIL_BOT_DB</c> environment variable (tests point
    ///     it at a temp file). WAL mode is enabled so a reader can inspect stats while a training batch writes.
    /// </summary>
    public sealed class BotDatabase : IDisposable
    {
        public const string PathEnvVar = "OREGONTRAIL_BOT_DB";

        private readonly SqliteConnection _connection;

        public BotDatabase(string? path = null)
        {
            Path = path ?? ResolveDefaultPath();

            var dir = System.IO.Path.GetDirectoryName(Path);
            if (!string.IsNullOrEmpty(dir))
                Directory.CreateDirectory(dir);

            _connection = new SqliteConnection(new SqliteConnectionStringBuilder
            {
                DataSource = Path,
                // One long-lived connection per session, so pooling buys nothing and only keeps the file handle (and WAL
                // sidecars) locked after Dispose — disable it so the database file releases cleanly.
                Pooling = false
            }.ToString());
            _connection.Open();

            Execute("PRAGMA journal_mode=WAL;");
            Execute("PRAGMA foreign_keys=ON;");

            SchemaMigrator.Migrate(_connection);

            Profiles = new ProfileStore(_connection);
            Runs = new RunStore(_connection);
            Leaderboard = new LeaderboardStore(_connection);
        }

        public string Path { get; }
        public ProfileStore Profiles { get; }
        public RunStore Runs { get; }
        public LeaderboardStore Leaderboard { get; }

        public static string ResolveDefaultPath()
        {
            var overridePath = Environment.GetEnvironmentVariable(PathEnvVar);
            if (!string.IsNullOrWhiteSpace(overridePath))
                return overridePath;

            return System.IO.Path.Combine(AppContext.BaseDirectory, "bot.db");
        }

        private void Execute(string sql)
        {
            using var cmd = _connection.CreateCommand();
            cmd.CommandText = sql;
            cmd.ExecuteNonQuery();
        }

        public void Dispose() => _connection.Dispose();
    }
}
