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
}
