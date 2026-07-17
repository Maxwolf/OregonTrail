using OregonTrailDotNet.Bot.Game;
using OregonTrailDotNet.Entity;

namespace OregonTrailDotNet.Bot.Learning
{
    /// <summary>
    ///     Shared mid-trail supply tactics: when to walk back into a fort store to restock, and when to accept an emigrant
    ///     trade. Every policy (genome, neural, heuristic) applies the same rules — only the thresholds differ — so the
    ///     capability lives here rather than being re-implemented three ways. These are the two levers that counter the
    ///     game's dominant failure: bleeding oxen until the wagon strands.
    /// </summary>
    internal static class SupplyTactics
    {
        /// <summary>How many rescue-trade attempts a stranded party makes before giving up. Browsing costs no game DAYS,
        ///     but every Trading screen runs a skip-day party tick (twice per browse) that can sicken the very party being
        ///     rescued — so the budget is modest, and it must also stay well under GamePlayer's per-run command cap.</summary>
        public const int MaxDesperateTradeAttempts = 15;

        /// <summary>Store base price ($/unit) of each barterable item — the common yardstick for valuing a trade offer.</summary>
        public static double UnitValue(EntitiesEnum item) => item switch
        {
            EntitiesEnum.Animal => 20,
            EntitiesEnum.Food => 0.10,
            EntitiesEnum.Clothes => 10,
            EntitiesEnum.Ammo => 2,
            EntitiesEnum.Medicine => 15,
            EntitiesEnum.Wheel => 10,
            EntitiesEnum.Axle => 10,
            EntitiesEnum.Tongue => 10,
            _ => 0
        };

        /// <summary>The wagon cannot roll — no oxen, or a broken part with no spare on hand — and only a trade (or, at a
        ///     fort, the store) can fix it. This is the state 98% of failed games died in before these tactics existed.</summary>
        public static bool IsDesperate(GameSnapshot state) =>
            state.Oxen <= 0 || (state.BrokenPart is { } part && state.OwnedOf(part) <= 0);

        /// <summary>Whether a store visit is worth making: something tracked is below its target and there is money to
        ///     spend. The once-per-location guard lives in the policy (it owns the per-game state).</summary>
        public static bool ShouldRestock(GameSnapshot state, Func<EntitiesEnum, int> target)
        {
            if (state.Cash < 5)
                return false;

            foreach (var item in new[]
                     {
                         EntitiesEnum.Animal, EntitiesEnum.Food, EntitiesEnum.Clothes, EntitiesEnum.Medicine,
                         EntitiesEnum.Ammo, EntitiesEnum.Wheel, EntitiesEnum.Axle, EntitiesEnum.Tongue
                     })
                if (target(item) > state.OwnedOf(item))
                    return true;

            return false;
        }

        /// <summary>
        ///     Whether to accept the trade currently on the table. A stranded party pays whatever it takes for the item that
        ///     un-strands the wagon; otherwise the offer must clear <paramref name="margin" /> $ of value at store prices,
        ///     and never strip the party of the team, clothing, or larder it needs to survive.
        /// </summary>
        public static bool AcceptTrade(GameSnapshot state, double margin)
        {
            if (state.Trade is not { CanPay: true } trade)
                return false;

            // Desperation: the wagon is (or is about to be) immovable — take any payable offer of the fixing item. The
            // one exception: never fix a broken part by handing over the whole team, which just re-strands the wagon.
            if (state.Oxen <= 0)
                return trade.Offered == EntitiesEnum.Animal;
            if (state.BrokenPart is { } part && state.OwnedOf(part) <= 0)
                return trade.Offered == part &&
                       (trade.Wanted != EntitiesEnum.Animal || state.Oxen - trade.WantedQuantity >= 1);

            // Survival guards: below 6 oxen the wagon slows (mileage scales min(1, oxen/6)), clothing under 2 sets per
            // living member risks the hail-freeze, and the larder floor mirrors the genome's 300 lb food minimum.
            if (trade.Wanted == EntitiesEnum.Animal && state.Oxen - trade.WantedQuantity < 6)
                return false;
            if (trade.Wanted == EntitiesEnum.Clothes && state.Clothing - trade.WantedQuantity < 2 * state.LivingCount)
                return false;
            if (trade.Wanted == EntitiesEnum.Food && state.Food - trade.WantedQuantity < 300)
                return false;

            return UnitValue(trade.Offered) * trade.OfferedQuantity -
                UnitValue(trade.Wanted) * trade.WantedQuantity >= margin;
        }
    }
}
