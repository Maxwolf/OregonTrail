// Created by Maxwolf (bigmaxwolf.com)

namespace OregonTrailDotNet.Entity.Item
{
    /// <summary>
    ///     What each good is worth at Matt's General Store in Independence, before the trail marks it up. These are the
    ///     1985 original's own prices, read out of its VAR.BIN price table (see legacy/source/VAR.BIN.txt).
    ///     They are kept here rather than written into the item definitions because two different parts of the game need
    ///     them and need them differently: the store quotes them marked up by how far west you are, while emigrants on the
    ///     trail barter against the plain unmarked value. Every one of them works out to the same twenty dollars a lot -
    ///     an ox, two sets of clothing, two spare parts, two hundred bullets, a hundred pounds of food - which is the
    ///     exchange rate the whole trading economy turns on.
    /// </summary>
    internal static class ItemPrices
    {
        /// <summary>
        ///     One ox. A yoke is two of them, which is the forty dollars every party is made to spend before leaving.
        /// </summary>
        internal const float Ox = 20f;

        /// <summary>
        ///     One set of clothing.
        /// </summary>
        internal const float ClothingSet = 10f;

        /// <summary>
        ///     One bullet. Sold by the box of twenty for two dollars, so a bullet is a tenth of a dollar.
        /// </summary>
        internal const float Bullet = 0.10f;

        /// <summary>
        ///     One spare wagon wheel, axle or tongue - they all cost the same.
        /// </summary>
        internal const float WagonPart = 10f;

        /// <summary>
        ///     One pound of food.
        /// </summary>
        internal const float FoodPound = 0.20f;

        /// <summary>
        ///     One medical kit. Ours, not MECC's - the original had no such line item.
        /// </summary>
        internal const float MedicineKit = 15f;
    }
}
