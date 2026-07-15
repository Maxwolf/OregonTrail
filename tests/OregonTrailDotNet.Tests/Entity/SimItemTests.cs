using System;
using OregonTrailDotNet.Entity;
using OregonTrailDotNet.Entity.Item;
using Xunit;

namespace OregonTrailDotNet.Tests.Entity
{
    /// <summary>
    ///     Covers the quantity clamping, scoring, and value math on the base item every commodity in
    ///     the simulation is built from. Pure logic, no game simulation required.
    /// </summary>
    public class SimItemTests
    {
        private static SimItem MakeItem(
            string name = "Test Item",
            int maxQuantity = 100,
            float cost = 2f,
            int weight = 5,
            int minimumQuantity = 1,
            int startingQuantity = 0,
            int pointsAwarded = 3,
            int pointsPerAmount = 10)
        {
            return new SimItem(EntitiesEnum.Food, name, "things", "thing", maxQuantity, cost, weight,
                minimumQuantity, startingQuantity, pointsAwarded, pointsPerAmount);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Ctor_RejectsNonPositiveMinimumQuantity(int minimum)
        {
            Assert.Throws<ArgumentException>(() => MakeItem(minimumQuantity: minimum));
        }

        [Fact]
        public void Ctor_StartsAtStartingQuantity()
        {
            var item = MakeItem(startingQuantity: 7);

            Assert.Equal(7, item.Quantity);
        }

        [Fact]
        public void CopyCtor_ClampsQuantityAboveMaximumToMaximum()
        {
            var item = new SimItem(MakeItem(maxQuantity: 100), 500);

            Assert.Equal(100, item.Quantity);
        }

        [Fact]
        public void CopyCtor_ClampsQuantityBelowMinimumToMinimum()
        {
            var item = new SimItem(MakeItem(minimumQuantity: 5), 0);

            Assert.Equal(5, item.Quantity);
        }

        [Fact]
        public void AddQuantity_AccumulatesOntoExistingQuantity()
        {
            var item = MakeItem();
            item.AddQuantity(10);
            item.AddQuantity(20);

            Assert.Equal(30, item.Quantity);
        }

        [Fact]
        public void AddQuantity_CapsAtMaximumQuantity()
        {
            var item = MakeItem(maxQuantity: 100);
            item.AddQuantity(500);

            Assert.Equal(100, item.Quantity);
        }

        [Fact]
        public void ReduceQuantity_SubtractsFromQuantity()
        {
            var item = MakeItem();
            item.AddQuantity(50);
            item.ReduceQuantity(20);

            Assert.Equal(30, item.Quantity);
        }

        [Fact]
        public void ReduceQuantity_FloorsAtZero()
        {
            var item = MakeItem();
            item.AddQuantity(10);
            item.ReduceQuantity(500);

            Assert.Equal(0, item.Quantity);
        }

        [Fact]
        public void Reset_RestoresStartingQuantity()
        {
            var item = MakeItem(startingQuantity: 7);
            item.AddQuantity(50);
            item.Reset();

            Assert.Equal(7, item.Quantity);
        }

        [Fact]
        public void Points_ZeroQuantity_AwardsNothing()
        {
            var item = MakeItem();

            Assert.Equal(0, item.Points);
        }

        [Fact]
        public void Points_QuantityBelowPerAmountThreshold_AwardsNothing()
        {
            var item = MakeItem(pointsAwarded: 3, pointsPerAmount: 10);
            item.AddQuantity(9);

            Assert.Equal(0, item.Points);
        }

        [Fact]
        public void Points_UsesIntegerDivisionOfQuantityByPerAmount()
        {
            var item = MakeItem(pointsAwarded: 3, pointsPerAmount: 10);
            item.AddQuantity(25);

            // 25 / 10 = 2 whole blocks, times 3 points each.
            Assert.Equal(6, item.Points);
        }

        [Fact]
        public void TotalWeight_MultipliesUnitWeightByQuantity()
        {
            var item = MakeItem(weight: 5);
            item.AddQuantity(30);

            Assert.Equal(150, item.TotalWeight);
        }

        [Fact]
        public void TotalValue_MultipliesCostByQuantity()
        {
            var item = MakeItem(cost: 2f);
            item.AddQuantity(30);

            Assert.Equal(60f, item.TotalValue);
        }

        [Fact]
        public void ToString_Default_ShowsCostPerDelineatingUnit()
        {
            var item = MakeItem(cost: 2f);

            Assert.Equal($"{2f:F2} per thing", item.ToString());
        }

        [Fact]
        public void ToString_StoreMode_ShowsTotalValueAsCurrency()
        {
            var item = MakeItem(cost: 2f);
            item.AddQuantity(30);

            Assert.Equal((2f*30).ToString("C2"), item.ToString(true));
        }

        [Fact]
        public void Equals_MatchesOnName()
        {
            Assert.True(MakeItem().Equals(MakeItem()));
            Assert.False(MakeItem().Equals(MakeItem(name: "Other Item")));
        }

        [Theory]
        [InlineData(1, "1 ox")]
        [InlineData(2, "2 oxen")]
        [InlineData(6, "6 oxen")]
        public void ToQuantityString_Oxen_ReadAsCountableAnimalWithSingularUnit(int quantity, string expected)
        {
            // Name is already the plural noun ("Oxen"), so the singular falls back to the delineating unit ("ox").
            Assert.Equal(expected, Parts.Oxen.ToQuantityString(quantity));
        }

        [Theory]
        [InlineData(1, "1 wheel")]
        [InlineData(3, "3 wheels")]
        public void ToQuantityString_VehicleParts_ReadAsCountableWithoutRedundantNameOfName(int quantity,
            string expected)
        {
            // Regression for "3 wheels of vehicle wheel": countable parts must never repeat the item noun.
            Assert.Equal(expected, Parts.Wheel.ToQuantityString(quantity));
            Assert.DoesNotContain("of vehicle", Parts.Wheel.ToQuantityString(quantity));
        }

        [Fact]
        public void ToQuantityString_AxleAndTongue_ArePluralizedCleanly()
        {
            Assert.Equal("3 axles", Parts.Axle.ToQuantityString(3));
            Assert.Equal("3 tongues", Parts.Tongue.ToQuantityString(3));
        }

        [Theory]
        [InlineData(1, "1 pound of food")]
        [InlineData(50, "50 pounds of food")]
        [InlineData(2000, "2,000 pounds of food")]
        public void ToQuantityString_Food_MeasuredInPoundsOfFood(int quantity, string expected)
        {
            // Bulk goods keep the "unit of name" phrasing that reads naturally ("50 pounds of food").
            Assert.Equal(expected, Resources.Food.ToQuantityString(quantity));
        }

        [Theory]
        [InlineData(1, "1 set of clothing")]
        [InlineData(3, "3 sets of clothing")]
        public void ToQuantityString_Clothing_MeasuredInSetsOfClothing(int quantity, string expected)
        {
            Assert.Equal(expected, Resources.Clothing.ToQuantityString(quantity));
        }

        [Fact]
        public void ToQuantityString_AmmunitionAndMedicine_MeasuredInTheirUnits()
        {
            Assert.Equal("5 boxes of ammunition", Resources.Bullets.ToQuantityString(5));
            Assert.Equal("3 kits of medicine", Resources.Medicine.ToQuantityString(3));
        }
    }
}
