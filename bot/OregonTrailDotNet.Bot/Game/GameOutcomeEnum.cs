namespace OregonTrailDotNet.Bot.Game
{
    /// <summary>
    ///     How a single driven game finished.
    /// </summary>
    public enum GameOutcomeEnum
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
}
