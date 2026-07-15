namespace OregonTrailDotNet.Bot.Data
{
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

        /// <summary>The shaped training fitness this run scored (the objective the optimizer maximizes), or 0 for legacy rows
        ///     recorded before fitness was tracked.</summary>
        public double Fitness { get; init; }
    }
}
