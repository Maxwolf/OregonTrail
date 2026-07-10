namespace OregonTrailDotNet.Bot.Game
{
    /// <summary>How quickly a watched game plays back, chosen by the viewer before watching begins.</summary>
    public enum WatchSpeed
    {
        Fast,
        Medium,
        Slow
    }

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

        /// <summary>A comfortable default for a human watching over the bot's shoulder (equivalent to <see cref="WatchSpeed.Medium" />).</summary>
        public static WatchOptions Default => ForSpeed(WatchSpeed.Medium);

        /// <summary>Builds pacing for one of the three viewer-selectable speeds.</summary>
        public static WatchOptions ForSpeed(WatchSpeed speed) => speed switch
        {
            WatchSpeed.Fast => new WatchOptions { TickDelayMs = 20, DecisionPauseMs = 350 },
            // Slow is meant to feel like watching a person actually play: travel scrolls gently and every decision holds
            // on screen long enough to read the bot's reasoning before it acts.
            WatchSpeed.Slow => new WatchOptions { TickDelayMs = 450, DecisionPauseMs = 4000 },
            _ => new WatchOptions { TickDelayMs = 60, DecisionPauseMs = 900 }
        };
    }
}
