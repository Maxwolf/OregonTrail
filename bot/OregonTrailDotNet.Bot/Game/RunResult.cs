namespace OregonTrailDotNet.Bot.Game
{
    /// <summary>
    ///     How a single driven game finished.
    /// </summary>
    public enum GameOutcome
    {
        /// <summary>Reached Oregon City.</summary>
        Win,

        /// <summary>Whole party died — the game records no score.</summary>
        Death,

        /// <summary>Ran out of time (>= 246 days) while still alive; the game tallies partial-progress points.</summary>
        Timeout,

        /// <summary>The bot aborted the run (soft-lock, tick cap, or a detected bug).</summary>
        Aborted
    }

    /// <summary>
    ///     Immutable summary of one game the bot played. Descriptive only — the trainer derives a fitness value from these
    ///     facts (see the CEM policy's shaping) and persists them to the <c>runs</c> table.
    /// </summary>
    public sealed class RunResult
    {
        public GameOutcome Outcome { get; init; }

        /// <summary>Final score as the game would record it (0 for death/abort).</summary>
        public int Score { get; init; }

        /// <summary>The score the game actually recorded to its top-ten during finalization, if it landed there. Used to
        ///     cross-check <see cref="Score" /> against the game's own tabulation; null when not recorded/found.</summary>
        public int? GameRecordedScore { get; init; }

        public int Days { get; init; }
        public int Miles { get; init; }
        public int Survivors { get; init; }
        public string CauseOfDeath { get; init; } = "";
        public string LeaderName { get; init; } = "";
        public int Profession { get; init; }
        public int StartMonth { get; init; }

        /// <summary>JSON of the genome that produced this run (filled in by the trainer).</summary>
        public string GenomeJson { get; set; } = "";

        public int Generation { get; set; }
        public int CandidateIndex { get; set; }

        /// <summary>True if this run's score beat the current #10 of the in-game top ten.</summary>
        public bool MadeTop10 { get; set; }

        /// <summary>If <see cref="Outcome" /> is <see cref="GameOutcome.Aborted" />, why.</summary>
        public string? AbortReason { get; init; }

        /// <summary>Set when the run was stopped by a detected crash/bug; the trainer surfaces this and halts.</summary>
        public Diagnostics.BugReport? Bug { get; set; }

        public bool IsFinished => Outcome is GameOutcome.Win or GameOutcome.Timeout;
    }
}
