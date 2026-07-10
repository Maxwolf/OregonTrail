namespace OregonTrailDotNet.Bot.Diagnostics
{
    /// <summary>
    ///     Thrown to abort a training/watch batch the moment a <see cref="BugReport" /> is produced, so the bot stops and the
    ///     report is surfaced to the developer instead of the bot continuing over a broken game.
    /// </summary>
    public sealed class BotBugException : Exception
    {
        public BotBugException(BugReport report) : base($"Bot stopped: {report.Category} — {report.Detail}")
        {
            Report = report;
        }

        public BugReport Report { get; }
    }
}
