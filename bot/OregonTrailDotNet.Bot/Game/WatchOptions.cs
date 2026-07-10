namespace OregonTrailDotNet.Bot.Game
{
    /// <summary>
    ///     Pacing for "watch a game" mode, where a human is watching the bot play and wants to follow along. When this is null
    ///     the game runs flat-out headless (training), with no rendering, delays, or narration.
    /// </summary>
    public sealed class WatchOptions
    {
        /// <summary>Delay after every logic tick, so travel/animation screens advance at a watchable speed.</summary>
        public int TickDelayMs { get; init; } = 60;

        /// <summary>Extra pause at each real decision, so the human can read what the bot chose and why.</summary>
        public int DecisionPauseMs { get; init; } = 900;

        /// <summary>Whether to show the bot's "thinking" status line under the game screen.</summary>
        public bool Narrate { get; init; } = true;

        /// <summary>A comfortable default for a human watching over the bot's shoulder.</summary>
        public static WatchOptions Default => new();
    }
}
