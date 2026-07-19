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
        private readonly Dictionary<string, int> _rates = new();

        /// <summary>
        ///     Frames per second a real-time section runs at, kept <i>per section</i> and remembered across visits.
        ///     <para>
        ///         These were shared at first, on the theory that a rate which feels right in one section feels right
        ///         in the next. It does not: the sections have genuinely different natural cadences. The hunt is a
        ///         walking pace, while the raft moves a whole lane per tick — a seventeenth of the river — so the rate
        ///         that makes the hunt read smoothly puts the raft across the water in under a second. Each section
        ///         declares its own starting rate and <c>-</c>/<c>+</c> tunes that one alone.
        ///     </para>
        /// </summary>
        /// <param name="section">The section's type name.</param>
        /// <param name="fallback">The section's declared starting rate, used the first time it is asked for.</param>
        public int TicksPerSecond(string section, int fallback) =>
            _rates.TryGetValue(section, out var rate) ? rate : fallback;

        /// <summary>Records a section's rate after the speed keys have moved it.</summary>
        /// <param name="section">The section's type name.</param>
        /// <param name="rate">Ticks per second, already clamped.</param>
        public void SetTicksPerSecond(string section, int rate) => _rates[section] = rate;
    }
}
