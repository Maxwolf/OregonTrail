// Created by Maxwolf (bigmaxwolf.com)
// Timestamp 01/03/2016@1:50 AM

using OregonTrailDotNet.Entity;
using OregonTrailDotNet.Entity.Item;
using OregonTrailDotNet.Entity.Vehicle;

namespace OregonTrailDotNet.Window.Travel.Trade
{
    /// <summary>
    ///     A swap an emigrant on the trail proposes: so much of one thing for so much of another. The two goods are picked
    ///     at random, but what they are worth against each other is not - an ox, two sets of clothing, two spare parts, two
    ///     hundred bullets and a hundred pounds of food are all the same twenty dollars, and the offer is built from that.
    ///     What makes trading worth doing is that the exchange rate takes no account of how hard a thing is to come by: food
    ///     is the one good that can be made out of nothing but a day and a bullet, so a party willing to hunt can turn it
    ///     into anything else on the list. What stops it being free money is that the emigrant always asks a little over the
    ///     odds, so every swap sheds some value even while it gets the party what it actually needs.
    /// </summary>
    public sealed class TradeOffer
    {
        /// <summary>
        ///     The goods an emigrant will barter, in the order the original kept them. Cash is not among them: money can be
        ///     spent but never traded for, which is what makes it the one thing on the wagon that can only ever run down.
        /// </summary>
        private static readonly EntitiesEnum[] Tradeable =
        {
            EntitiesEnum.Animal, EntitiesEnum.Clothes, EntitiesEnum.Ammo,
            EntitiesEnum.Wheel, EntitiesEnum.Axle, EntitiesEnum.Tongue, EntitiesEnum.Food
        };

        /// <summary>
        ///     Bullets and food are counted in units too small to swap one at a time, so they change hands fifty at a time.
        /// </summary>
        private const int SmallGoodsLot = 50;

        /// <summary>
        ///     Initializes a new instance of the <see cref="TradeOffer" /> class.
        /// </summary>
        /// <param name="wanted">What the emigrant wants, and how much of it.</param>
        /// <param name="offered">What they will hand over for it.</param>
        private TradeOffer(SimItem wanted, SimItem offered)
        {
            WantedItem = wanted;
            OfferedItem = offered;
        }

        /// <summary>
        ///     Wanted item from the players vehicle inventory in order to get the offered item.
        /// </summary>
        public SimItem WantedItem { get; }

        /// <summary>
        ///     Offers up an item in exchange for the traders wanted item.
        /// </summary>
        public SimItem OfferedItem { get; }

        /// <summary>
        ///     Builds the offer an emigrant would put to the party today, or NULL if what they came up with is not worth
        ///     putting to anybody.
        /// </summary>
        /// <returns>The proposed swap, or NULL for no offer.</returns>
        public static TradeOffer Generate()
        {
            var random = GameSimulationApp.Instance.Random;

            // One good they want, and a different one they are parting with.
            var wantIndex = random.Next(Tradeable.Length);
            var giveIndex = random.Next(Tradeable.Length);
            if (giveIndex == wantIndex)
                giveIndex++;
            if (giveIndex >= Tradeable.Length)
                giveIndex = 0;

            var wanted = Tradeable[wantIndex];
            var offered = Tradeable[giveIndex];

            // How many of the thing they are giving is one of the thing they want worth? At honest value that would simply
            // be the ratio of the two prices; the emigrant instead asks for up to two and a fifth times that, and never
            // less than the honest rate. So a trade is always a shade worse than fair and sometimes a good deal worse.
            var haggle = 1 + 1.2*random.NextDouble();
            var ratio = UnitValue(wanted)/(UnitValue(offered)*haggle);

            // Whichever good is worth less changes hands in numbers; the dearer one goes one at a time.
            int wantedQuantity;
            int offeredQuantity;
            if (ratio < 1)
            {
                wantedQuantity = (int) (1/ratio);
                offeredQuantity = 1;
            }
            else
            {
                wantedQuantity = 1;
                offeredQuantity = (int) ratio;
            }

            // Nobody haggles over single bullets and single pounds of food.
            if (IsSmallGoodsPair(wanted, offered))
            {
                wantedQuantity *= SmallGoodsLot;
                offeredQuantity *= SmallGoodsLot;
            }

            if ((wantedQuantity <= 0) || (offeredQuantity <= 0))
                return null;

            return new TradeOffer(
                new SimItem(Vehicle.DefaultInventory[wanted], wantedQuantity),
                new SimItem(Vehicle.DefaultInventory[offered], offeredQuantity));
        }

        /// <summary>
        ///     What one of a good is worth on the trail. This is the plain Independence price rather than what a fort would
        ///     charge: emigrants barter against what a thing is worth, not against how far from a store they are standing.
        /// </summary>
        /// <param name="category">Good being valued.</param>
        /// <returns>Value in dollars of a single ox, set, bullet, part or pound.</returns>
        private static float UnitValue(EntitiesEnum category)
        {
            switch (category)
            {
                case EntitiesEnum.Animal:
                    return ItemPrices.Ox;
                case EntitiesEnum.Clothes:
                    return ItemPrices.ClothingSet;
                case EntitiesEnum.Ammo:
                    return ItemPrices.Bullet;
                case EntitiesEnum.Food:
                    return ItemPrices.FoodPound;
                default:
                    return ItemPrices.WagonPart;
            }
        }

        /// <summary>
        ///     Whether this swap is bullets against food, the one pairing where both sides are counted in units too small to
        ///     trade singly.
        /// </summary>
        /// <param name="wanted">Good the emigrant wants.</param>
        /// <param name="offered">Good the emigrant is giving.</param>
        /// <returns>TRUE when the two goods are ammunition and food in either order.</returns>
        private static bool IsSmallGoodsPair(EntitiesEnum wanted, EntitiesEnum offered)
        {
            return ((wanted == EntitiesEnum.Ammo) && (offered == EntitiesEnum.Food)) ||
                   ((wanted == EntitiesEnum.Food) && (offered == EntitiesEnum.Ammo));
        }
    }
}
