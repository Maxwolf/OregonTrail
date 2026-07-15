namespace OregonTrailDotNet.Bot.Game
{
    /// <summary>
    ///     Immutable summary of one game the bot played. Descriptive only — the trainer derives a fitness value from these
    ///     facts (see the CEM policy's shaping) and persists them to the <c>runs</c> table.
    /// </summary>
    public sealed class RunResult
    {
        public GameOutcomeEnum Outcome { get; init; }

        /// <summary>Final score as the game would record it (0 for death/abort).</summary>
        public int Score { get; init; }

        /// <summary>The score the game actually recorded to its top-ten during finalization, if it landed there. Used to
        ///     cross-check <see cref="Score" /> against the game's own tabulation; null when not recorded/found.</summary>
        public int? GameRecordedScore { get; init; }

        public int Days { get; init; }
        public int Miles { get; init; }
        public int Survivors { get; init; }

        /// <summary>Total party size the run started with (living + dead). Used with <see cref="Survivors" /> to count deaths,
        ///     so the trainer can penalise losing members and reward bringing the whole party through.</summary>
        public int PartySize { get; init; }

        /// <summary>Average health of the living party at the end of the run, as the raw <c>HealthStatus</c> value (Good=500 ..
        ///     VeryPoor=200, 0 if nobody is left alive). The trainer weights survivors by this so "arrive healthy" beats "arrive
        ///     barely alive", giving a survival gradient even for runs that never finish.</summary>
        public int PartyHealthValue { get; init; }
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

        /// <summary>If <see cref="Outcome" /> is <see cref="GameOutcomeEnum.Aborted" />, why.</summary>
        public string? AbortReason { get; init; }

        /// <summary>Set when the run was stopped by a detected crash/bug; the trainer surfaces this and halts.</summary>
        public Diagnostics.BugReport? Bug { get; set; }

        public bool IsFinished => Outcome is GameOutcomeEnum.Win or GameOutcomeEnum.Timeout;
    }
}
