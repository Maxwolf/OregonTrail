using System.Collections.Generic;
using OregonTrailDotNet.Entity.Location;
using OregonTrailDotNet.Entity.Location.Point;
using OregonTrailDotNet.Entity.Location.Weather;
using OregonTrailDotNet.Window.Travel.RiverCrossing;
using Xunit;

namespace OregonTrailDotNet.Tests.Entity
{
    /// <summary>
    ///     Covers the point-of-interest types that make up the trail. Location construction rolls the
    ///     shared dice for the fresh water warning, so a booted simulation is required.
    /// </summary>
    public class LocationTests : SimulationTestBase
    {
        [Fact]
        public void Settlement_AllowsChattingAndShopping()
        {
            var settlement = new Settlement("Fort Test", ClimateEnum.Moderate);

            Assert.True(settlement.ChattingAllowed);
            Assert.True(settlement.ShoppingAllowed);
        }

        [Fact]
        public void Landmark_ForbidsChattingAndShopping()
        {
            var landmark = new Landmark("Test Rock", ClimateEnum.Dry);

            Assert.False(landmark.ChattingAllowed);
            Assert.False(landmark.ShoppingAllowed);
        }

        [Fact]
        public void TollRoad_ForbidsChattingAndShopping()
        {
            var tollRoad = new TollRoad("Test Toll Road", ClimateEnum.Moderate);

            Assert.False(tollRoad.ChattingAllowed);
            Assert.False(tollRoad.ShoppingAllowed);
        }

        [Fact]
        public void NewLocation_StartsUnreached()
        {
            var location = new Landmark("Test Rock", ClimateEnum.Moderate);

            Assert.Equal(LocationStatusEnum.Unreached, location.Status);
            Assert.False(location.LastLocation);
        }

        [Fact]
        public void RiverCrossing_DefaultsToFloatAndFord()
        {
            var river = new RiverCrossing("Test River", ClimateEnum.Continental);

            Assert.Equal(RiverOptionEnum.FloatAndFord, river.RiverCrossOption);
        }

        [Fact]
        public void RiverCrossing_KeepsExplicitCrossingOption()
        {
            var river = new RiverCrossing("Test River", ClimateEnum.Continental, RiverOptionEnum.FerryOperator);

            Assert.Equal(RiverOptionEnum.FerryOperator, river.RiverCrossOption);
        }

        [Fact]
        public void ForkInRoad_ExposesSkipChoices()
        {
            var choices = new List<Location>
            {
                new Landmark("Left Path", ClimateEnum.Dry),
                new Landmark("Right Path", ClimateEnum.Dry)
            };
            var fork = new ForkInRoad("Test Fork", ClimateEnum.Dry, choices);

            Assert.Equal(2, fork.SkipChoices.Count);
            Assert.Equal("Left Path", fork.SkipChoices[0].Name);
        }

        [Fact]
        public void ForkInRoad_WithoutSkipChoices_ReturnsNullInsteadOfThrowing()
        {
            // The getter documents that it returns null when there are no skip choices; a null skipChoices argument
            // used to leave the backing list null and make the getter throw a NullReferenceException.
            var fork = new ForkInRoad("Dead End", ClimateEnum.Dry, null);

            Assert.Null(fork.SkipChoices);
        }
    }
}
