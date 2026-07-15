namespace OregonTrailDotNet.Bot.Data
{
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
}
