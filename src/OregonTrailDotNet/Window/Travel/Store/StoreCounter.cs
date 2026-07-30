// Created by Maxwolf (bigmaxwolf.com)

using OregonTrailDotNet.Entity;
using OregonTrailDotNet.Entity.Item;
using OregonTrailDotNet.Entity.Location;

namespace OregonTrailDotNet.Window.Travel.Store
{
    /// <summary>
    ///     Which counter the party is standing at, and how that counter sells its goods.
    ///     <para>
    ///         The original had <b>two</b> stores, not one shop with two skins, and they do not sell the same way.
    ///         Matt's, at Independence, is the outfitter: he talks you through what a family needs, keeps a running
    ///         tab, and sells oxen the way a teamster does - by the yoke, two at a time, forty dollars. The forts out
    ///         on the trail are a counter and a price list: no advice, no tab, and oxen sold <b>singly</b> at twenty,
    ///         because a party that has lost one animal needs to replace one animal.
    ///     </para>
    ///     <para>
    ///         The quantity fields differ too, and in the original those field widths <i>were</i> the per-purchase
    ///         limit. Matt's are tight (one character for oxen, two for ammunition); the forts give three, and four
    ///         for food. Byte offsets for all of it are in the notes on
    ///         <see cref="SimItem.MaxSaleDigits" />.
    ///     </para>
    ///     <para>
    ///         The item definitions describe the <i>goods</i>; this describes the <i>counter</i>. Everything that needs
    ///         to know how the shop in front of the player behaves asks here, so the two stores can differ in one place
    ///         rather than in a condition sprinkled through every store form.
    ///     </para>
    /// </summary>
    internal static class StoreCounter
    {
        /// <summary>Field width every fort counter uses, and the four the original gave food alone.</summary>
        private const int FortSaleDigits = 3;

        /// <summary>How wide the fort's food field is - a wagon takes two thousand pounds, so three would not reach.</summary>
        private const int FortFoodSaleDigits = 4;

        /// <summary>
        ///     TRUE while the party is still outfitting at Matt's in Independence, before the journey has begun.
        ///     <para>
        ///         The location index alone is not enough and this is the bug that keeps being rewritten: the party
        ///         comes back through Independence's own index the moment it departs, so the status has to be checked
        ///         too. Every store form reads this one property rather than rebuilding the condition.
        ///     </para>
        /// </summary>
        internal static bool AtMatts =>
            GameSimulationApp.Instance.Trail.IsFirstLocation &&
            GameSimulationApp.Instance.Trail.CurrentLocation?.Status == LocationStatusEnum.Unreached;

        /// <summary>
        ///     How many units of this good the counter in front of the player sells at a time. The item's own lot is
        ///     Matt's; a fort breaks the yoke apart and sells oxen one by one.
        /// </summary>
        /// <param name="item">The good being bought.</param>
        internal static int LotSize(SimItem item)
        {
            if (item == null)
                return 1;

            return !AtMatts && item.Category == EntitiesEnum.Animal ? 1 : item.LotSize;
        }

        /// <summary>
        ///     How many units of a good this counter sells at a time, by category. The same answer
        ///     <see cref="LotSize(SimItem)" /> gives, for callers holding only the category - the headless bot, which
        ///     has to answer the quantity prompt in whatever unit the counter in front of it is using.
        /// </summary>
        /// <param name="category">The good's category.</param>
        internal static int LotSize(EntitiesEnum category)
        {
            switch (category)
            {
                case EntitiesEnum.Animal:
                    return LotSize(Parts.Oxen);
                case EntitiesEnum.Ammo:
                    return LotSize(Resources.Bullets);
                default:
                    return 1;
            }
        }

        /// <summary>Singular name of one lot at this counter - "yoke" at Matt's, "ox" at a fort.</summary>
        /// <param name="item">The good being bought.</param>
        internal static string LotUnit(SimItem item)
        {
            return LotSize(item) == item.LotSize ? item.LotUnit : item.DelineatingUnit;
        }

        /// <summary>Plural name of a lot at this counter - "yoke" at Matt's, "oxen" at a fort.</summary>
        /// <param name="item">The good being bought.</param>
        internal static string LotPluralForm(SimItem item)
        {
            return LotSize(item) == item.LotSize ? item.LotPluralForm : item.PluralForm;
        }

        /// <summary>Renders a count of this counter's lots with the right noun: "3 yoke", "1 ox", "12 boxes".</summary>
        /// <param name="item">The good being bought.</param>
        /// <param name="lots">How many lots.</param>
        internal static string ToLotString(SimItem item, int lots)
        {
            return $"{lots:N0} {(lots == 1 ? LotUnit(item) : LotPluralForm(item))}";
        }

        /// <summary>
        ///     The most lots this counter's quantity field can physically accept. Matt's uses each item's own declared
        ///     width; the forts use a flat three characters, four for food.
        /// </summary>
        /// <param name="item">The good being bought.</param>
        internal static int MaxLots(SimItem item)
        {
            if (item == null)
                return int.MaxValue;

            if (AtMatts)
                return item.MaxSaleLots;

            var digits = item.Category == EntitiesEnum.Food ? FortFoodSaleDigits : FortSaleDigits;
            return SimItem.LotsForDigits(digits);
        }
    }
}
