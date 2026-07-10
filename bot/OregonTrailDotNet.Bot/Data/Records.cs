namespace OregonTrailDotNet.Bot.Data
{
    /// <summary>A saved bot profile: identity, learning progress, and the serialized learning state that lets it improve
    ///     across sessions. The profile IS the save file (persisted in the shared SQLite database).</summary>
    public sealed class ProfileRecord
    {
        public long Id { get; init; }
        public string Name { get; init; } = "";
        public string PolicyKind { get; init; } = "";
        public string CreatedUtc { get; init; } = "";
        public string UpdatedUtc { get; init; } = "";
        public int TotalIterations { get; init; }
        public int Generations { get; init; }
        public int BestScore { get; init; }
        public string? BestGenomeJson { get; init; }
        public byte[]? LearningState { get; init; }
    }

    /// <summary>One recorded game played by a profile.</summary>
    public sealed class RunRecord
    {
        public long Id { get; init; }
        public long ProfileId { get; init; }
        public int IterationIndex { get; init; }
        public int? Generation { get; init; }
        public int? CandidateIndex { get; init; }
        public string TimestampUtc { get; init; } = "";
        public string GenomeJson { get; init; } = "";
        public string Outcome { get; init; } = "";
        public int FinalScore { get; init; }
        public int Days { get; init; }
        public int Miles { get; init; }
        public int Survivors { get; init; }
        public string? CauseOfDeath { get; init; }
        public bool MadeTop10 { get; init; }
    }

    /// <summary>An entry on the bot's persistent high-score leaderboard (name already carries the "(bot)" suffix).</summary>
    public sealed class LeaderboardEntry
    {
        public long Id { get; init; }
        public long? ProfileId { get; init; }
        public long? RunId { get; init; }
        public string Name { get; init; } = "";
        public int Score { get; init; }
        public string Rating { get; init; } = "";
        public string TimestampUtc { get; init; } = "";
    }

    /// <summary>Per-generation training progress used to render the learning curve.</summary>
    public sealed class GenerationStat
    {
        public int Generation { get; init; }
        public double MeanScore { get; init; }
        public int BestScore { get; init; }
        public int Games { get; init; }
    }
}
