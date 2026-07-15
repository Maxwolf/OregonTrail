using OregonTrailDotNet.Window.Travel.Hunt;

namespace OregonTrailDotNet.Bot.Game
{
    /// <summary>
    ///     Decides when the bot has bagged enough food on a hunt and should stop, the way a person would once the wagon can't
    ///     hold any more rather than firing until the daylight runs out. Kept as a small standalone rule (like the recognizer's
    ///     other universal, non-strategic guards) so it applies to every policy and is trivial to unit-test.
    /// </summary>
    public static class HuntStrategy
    {
        /// <summary>
        ///     Whether the party has bagged enough on the hunt in progress to stop now. Returns false when not hunting / nothing
        ///     bagged yet.
        /// </summary>
        /// <remarks>
        ///     The trigger is the carry cap and nothing short of it. The hunt costs one full day the moment it ends no matter
        ///     when you leave (<c>HuntingResult</c> ticks a day either way), so quitting early never saves time — it only trades
        ///     away food the party could have carried, which for a hungry party means re-hunting sooner. Once the haul reaches
        ///     the cap, though, the wagon is full and the game discards anything more, so every further shot is pure wasted
        ///     ammunition (exactly the "don't kill more than you keep" the game warns about). Stopping there is a strict win:
        ///     same food, fewer bullets spent. A single big animal (buffalo, caribou, bear) fills the cap on its own, so this
        ///     fires often in practice.
        /// </remarks>
        public static bool HasEnoughFood(GameSnapshot state)
        {
            return state.HuntBagged >= HuntManager.MAXFOOD;
        }
    }
}
