namespace OregonTrailDotNet.Bot
{
    /// <summary>
    ///     Process-wide state that must survive the teardown/recreate cycle of the control-panel window stack (whose
    ///     <c>WindowData</c> is discarded each time). Holds the database location, the active profile, and the pending
    ///     transition request. Data-access stores are attached here by <see cref="Program" /> at startup.
    /// </summary>
    public static class BotContext
    {
        /// <summary>
        ///     Absolute path to the SQLite database. Defaults to <c>bot.db</c> next to the running executable and can be
        ///     overridden with the <c>OREGONTRAIL_BOT_DB</c> environment variable (used by tests to point at a temp file).
        /// </summary>
        public static string DbPath { get; set; } = string.Empty;

        /// <summary>
        ///     Currently selected profile id, or -1 if none is selected.
        /// </summary>
        public static long ActiveProfileId { get; set; } = -1;

        /// <summary>
        ///     Currently selected profile name, shown in UI headers.
        /// </summary>
        public static string ActiveProfileName { get; set; } = string.Empty;

        /// <summary>
        ///     Pending transition the control panel wants <see cref="Program" /> to perform, or null.
        /// </summary>
        public static BotRequest? Request { get; set; }

        /// <summary>
        ///     The open database for the session, set by <see cref="Program" /> at startup so control-panel forms can read
        ///     profiles/leaderboard/stats. Only one app (control panel or game) runs at a time, so sharing is safe.
        /// </summary>
        public static Data.BotDatabase? Db { get; set; }
    }
}
