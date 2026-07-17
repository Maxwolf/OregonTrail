using OregonTrailDotNet.Bot.Game;
using OregonTrailDotNet.Window.Travel;

namespace OregonTrailDotNet.Bot.Learning
{
    /// <summary>
    ///     The endgame score grind shared by the learned policies: at the last stops before Oregon City, keep browsing
    ///     emigrant trades so hunted food converts into clothes and bullets — the same strategy that pushes the 1985 game
    ///     toward its 13,860 ceiling (clothes cap 255 sets, bullets cap 65,535). The genome decides how many browses to
    ///     spend and when the party's health calls it off; this class owns the per-game counter, mirroring SupplyPlanner.
    ///     One instance per policy per game.
    /// </summary>
    internal sealed class EndgameGrinder
    {
        private int _attempts;

        public bool WantTrade(GameSnapshot state, int grindTrades, double grindHealthFloor,
            IReadOnlyCollection<TravelCommandsEnum> available)
        {
            // Zero budget is the off switch (and what legacy genomes decode to); spent budget ends the grind.
            if (grindTrades <= 0 || _attempts >= grindTrades)
                return false;

            if (!available.Contains(TravelCommandsEnum.AttemptToTrade))
                return false;

            // Only grind once the journey is effectively over — the final stops before Oregon City.
            if (!state.NearTrailEnd)
                return false;

            // Every browse ticks the party (illness risk); five heads at Good health outscore any pile of goods.
            if ((int) state.LowestHealth <= grindHealthFloor)
                return false;

            _attempts++;
            return true;
        }
    }
}
