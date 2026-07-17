using System.Collections.Generic;
using System.Linq;
using OregonTrailDotNet.Entity;
using OregonTrailDotNet.Entity.Item;
using OregonTrailDotNet.Window.Travel.Trade;
using Xunit;

namespace OregonTrailDotNet.Tests.Window
{
    /// <summary>
    ///     Covers what an emigrant will swap for what. The exchange rate is the whole of the trading economy: every good is
    ///     priced against every other, and because food is the one thing a party can manufacture out of a day and a bullet,
    ///     that rate is what lets hunting be turned into oxen, clothing and spare parts. The randomizer is not seedable, so
    ///     these sample many offers and assert the bounds the value model guarantees rather than exact rolls.
    /// </summary>
    public class TradeOfferTests : SimulationTestBase
    {
        /// <summary>
        ///     What a single unit of each good is worth, which is what every offer is built from.
        /// </summary>
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

        private static IEnumerable<TradeOffer> Offers(int count)
        {
            for (var i = 0; i < count; i++)
            {
                var offer = TradeOffer.Generate();
                if (offer != null)
                    yield return offer;
            }
        }

        [Fact]
        public void EveryLotIsWorthTwentyDollars()
        {
            // The rate the whole economy turns on, straight out of the original's price table: an ox, two sets of
            // clothing, two spare parts, two hundred bullets and a hundred pounds of food are all the same money.
            Assert.Equal(20f, ItemPrices.Ox);
            Assert.Equal(20f, 2*ItemPrices.ClothingSet);
            Assert.Equal(20f, 2*ItemPrices.WagonPart);
            Assert.Equal(20f, 200*ItemPrices.Bullet, 3);
            Assert.Equal(20f, 100*ItemPrices.FoodPound, 3);
        }

        [Fact]
        public void AnOfferNeverFavoursThePlayer_AndIsNeverWorseThanRoughlyHalf()
        {
            // The emigrant asks for between the honest rate and two and a fifth times it. So the party always sheds a
            // little value on a swap and sometimes a lot — trading gets you what you need, never ahead. Quantities are
            // whole numbers, so allow a unit of rounding slack in each direction.
            foreach (var offer in Offers(3000))
            {
                var given = offer.WantedItem.Quantity*UnitValue(offer.WantedItem.Category);
                var got = offer.OfferedItem.Quantity*UnitValue(offer.OfferedItem.Category);

                var wantUnit = UnitValue(offer.WantedItem.Category);
                var giveUnit = UnitValue(offer.OfferedItem.Category);
                var slack = wantUnit + giveUnit;

                Assert.True(got <= given + slack,
                    $"Offer favoured the player: gave {given:C} of {offer.WantedItem.Category}, " +
                    $"got {got:C} of {offer.OfferedItem.Category}.");
                Assert.True(got >= given/2.2 - slack,
                    $"Offer was worse than the haggling floor: gave {given:C}, got {got:C}.");
            }
        }

        [Fact]
        public void TheTwoGoodsAreAlwaysDifferent_AndCashIsNeverBartered()
        {
            foreach (var offer in Offers(1000))
            {
                Assert.NotEqual(offer.WantedItem.Category, offer.OfferedItem.Category);

                // Money can be spent but never traded for; it is the one resource that only ever runs down.
                Assert.NotEqual(EntitiesEnum.Cash, offer.WantedItem.Category);
                Assert.NotEqual(EntitiesEnum.Cash, offer.OfferedItem.Category);
                Assert.NotEqual(EntitiesEnum.Vehicle, offer.OfferedItem.Category);
                Assert.NotEqual(EntitiesEnum.Person, offer.OfferedItem.Category);

                Assert.True(offer.WantedItem.Quantity > 0);
                Assert.True(offer.OfferedItem.Quantity > 0);
            }
        }

        [Fact]
        public void FoodBuysBulletsAtRoughlyTheHistoricRate_WhichIsWhatMakesHuntingPay()
        {
            // A hundred pounds of food is twenty dollars, and so is two hundred bullets. A party that can hunt can turn a
            // day and a bullet into food and food back into more bullets than it started with — that loop is the reason
            // trading matters at all, and it only exists because the rate ignores how easy food is to come by.
            var swaps = Offers(6000)
                .Where(o => (o.WantedItem.Category == EntitiesEnum.Food) &&
                            (o.OfferedItem.Category == EntitiesEnum.Ammo))
                .ToList();

            Assert.NotEmpty(swaps);

            foreach (var swap in swaps)
            {
                // Bullets and food are bartered fifty at a time; nobody haggles over a single pound.
                Assert.True(swap.WantedItem.Quantity%50 == 0, $"Food lot was {swap.WantedItem.Quantity}.");
                Assert.True(swap.OfferedItem.Quantity%50 == 0, $"Ammo lot was {swap.OfferedItem.Quantity}.");
            }
        }

        [Fact]
        public void AnEmigrantNeverAsksForMoreSparePartsThanAWagonCouldHold()
        {
            // A spare part is worth half an ox, so at the worst of the haggling the value model asks four of them for one
            // — and a wagon may only carry three. The original shipped exactly that and never checked, which is why
            // "wants 4 wagon wheels" is a thing players remember; an offer nobody could accept just wasted their day.
            // The item's own ceiling clamps the demand back to three here, so the offer stays one the party can actually
            // take. This is a deliberate (small) departure from the original rather than an oversight.
            var partDemands = Offers(6000)
                .Where(o => (o.WantedItem.Category == EntitiesEnum.Wheel) ||
                            (o.WantedItem.Category == EntitiesEnum.Axle) ||
                            (o.WantedItem.Category == EntitiesEnum.Tongue))
                .Select(o => o.WantedItem.Quantity)
                .ToList();

            Assert.NotEmpty(partDemands);
            Assert.All(partDemands, quantity => Assert.InRange(quantity, 1, 3));

            // The haggling still bites: somebody does ask the full three parts for the one ox.
            Assert.Contains(3, partDemands);
        }
    }
}
