using WolfCurses.Window;

namespace OregonTrailDotNet.Minigames.Windows
{
    /// <summary>
    ///     Shared state for the workbench window. The sections are deliberately self-contained — a minigame under test
    ///     should not be able to depend on anything the game would have set up — so this only carries what survives
    ///     between visits to a section.
    /// </summary>
    public sealed class MinigameInfo : WindowData
    {
        /// <summary>
        ///     Frames per second the real-time sections run at, remembered across sections so a rate that feels right
        ///     in one is still there in the next. The original's rate is unknowable from the listing (its loop had no
        ///     delay, so it ran at whatever Applesoft managed), which is exactly why this is a live knob.
        /// </summary>
        public int TicksPerSecond { get; set; } = 20;
    }
}
