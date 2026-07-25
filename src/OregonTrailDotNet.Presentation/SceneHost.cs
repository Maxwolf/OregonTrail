namespace OregonTrailDotNet.Presentation
{
    /// <summary>
    ///     Whether the process hosting these scenes has a screen to draw on.
    ///     <para>
    ///         This is a <b>drawing</b> switch and nothing else. Every scene runs the same simulation either way and
    ///         calls <see cref="SceneForm{TData}.Advance" /> the same number of times for the same game; the flag
    ///         only decides whether each step is resampled into an ANSI picture and paced against a wall clock, or
    ///         stepped straight through with a one-line caption. A headless host (the training bot, the test suites)
    ///         therefore plays the identical game a human plays — it simply does not pay for the pixels or wait out
    ///         the animation.
    ///     </para>
    ///     <para>
    ///         Deliberately not a fork of the scene classes and deliberately not consulted anywhere that touches game
    ///         state. If you find yourself reaching for it inside <c>Build</c>, <c>Advance</c> or a damage table, the
    ///         change is wrong: that is the road back to the bot and the player playing two different games.
    ///     </para>
    /// </summary>
    public static class SceneHost
    {
        /// <summary>
        ///     True when frames should be composed and paced for a human. Set once at startup by a host that owns a
        ///     console — the game's <c>Program.Main</c> and the minigame workbench. Off by default, which is what the
        ///     bot and the tests get.
        /// </summary>
        public static bool Graphical { get; set; }
    }
}
