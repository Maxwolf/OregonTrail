namespace OregonTrailDotNet.Bot
{
    /// <summary>
    ///     What a control-panel form is asking <see cref="Program" /> to do after it tears the UI down.
    /// </summary>
    public enum BotRequestKind
    {
        None,
        Train,
        Watch,
        AutoTest,
        Benchmark,
        Quit
    }

    /// <summary>
    ///     A transition request handed from the WolfCurses control-panel UI to the top-level <see cref="Program" /> state
    ///     machine. Because only one WolfCurses <c>SimulationApp</c> can own the console at a time, a form cannot launch a game
    ///     session itself; instead it records the request here and destroys the control-panel app, and <see cref="Program" />
    ///     acts on it.
    /// </summary>
    public sealed class BotRequest
    {
        public BotRequestKind Kind { get; init; }
        public long ProfileId { get; init; }
        public int PopulationSize { get; init; }
        public int GamesPerCandidate { get; init; }
        public int Generations { get; init; }

        /// <summary>Watch mode: playback speed the viewer chose on the watch-config screen.</summary>
        public Game.WatchSpeed WatchSpeed { get; init; } = Game.WatchSpeed.Medium;

        /// <summary>Watch mode: keep replaying games until the viewer presses Esc.</summary>
        public bool LoopUntilEscape { get; init; }

        /// <summary>Automated testing: session length in minutes (0 = run until Esc).</summary>
        public int AutoTestMinutes { get; init; } = 5;

        /// <summary>Automated testing: stop the session as soon as a problem is found (vs. keep going and log them all).</summary>
        public bool AutoTestStopOnProblem { get; init; } = true;

        /// <summary>Benchmark: time limit in minutes (0 = run until every model wins or Esc).</summary>
        public int BenchmarkMinutes { get; init; } = 5;
    }

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
