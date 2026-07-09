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
            var settlement = new Settlement("Fort Test", Climate.Moderate);

            Assert.True(settlement.ChattingAllowed);
            Assert.True(settlement.ShoppingAllowed);
        }

        [Fact]
        public void Landmark_ForbidsChattingAndShopping()
        {
            var landmark = new Landmark("Test Rock", Climate.Dry);

            Assert.False(landmark.ChattingAllowed);
            Assert.False(landmark.ShoppingAllowed);
        }

        [Fact]
        public void TollRoad_ForbidsChattingAndShopping()
        {
            var tollRoad = new TollRoad("Test Toll Road", Climate.Moderate);

            Assert.False(tollRoad.ChattingAllowed);
            Assert.False(tollRoad.ShoppingAllowed);
        }

        [Fact]
        public void NewLocation_StartsUnreached()
        {
            var location = new Landmark("Test Rock", Climate.Moderate);

            Assert.Equal(LocationStatus.Unreached, location.Status);
            Assert.False(location.LastLocation);
        }

        [Fact]
        public void RiverCrossing_DefaultsToFloatAndFord()
        {
            var river = new RiverCrossing("Test River", Climate.Continental);

            Assert.Equal(RiverOption.FloatAndFord, river.RiverCrossOption);
        }

        [Fact]
        public void RiverCrossing_KeepsExplicitCrossingOption()
        {
            var river = new RiverCrossing("Test River", Climate.Continental, RiverOption.FerryOperator);

            Assert.Equal(RiverOption.FerryOperator, river.RiverCrossOption);
        }

        [Fact]
        public void ForkInRoad_ExposesSkipChoices()
        {
            var choices = new List<Location>
            {
                new Landmark("Left Path", Climate.Dry),
                new Landmark("Right Path", Climate.Dry)
            };
            var fork = new ForkInRoad("Test Fork", Climate.Dry, choices);

            Assert.Equal(2, fork.SkipChoices.Count);
            Assert.Equal("Left Path", fork.SkipChoices[0].Name);
        }
    }
}
