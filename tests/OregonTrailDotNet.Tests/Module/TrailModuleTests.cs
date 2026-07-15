using System;
using System.Linq;
using OregonTrailDotNet.Entity.Location;
using OregonTrailDotNet.Entity.Location.Point;
using OregonTrailDotNet.Entity.Location.Weather;
using OregonTrailDotNet.Module.Trail;
using Xunit;
using TrailEntity = OregonTrailDotNet.Module.Trail.Trail;

namespace OregonTrailDotNet.Tests.Module
{
    /// <summary>
    ///     Covers the registry-defined Oregon Trail and the module that walks the vehicle down it.
    ///     Trail construction rolls random segment lengths, so a booted simulation is required.
    /// </summary>
    public class TrailModuleTests : SimulationTestBase
    {
        [Fact]
        public void Journey_StartsAtIndependence()
        {
            Assert.Equal(0, Game.Trail.LocationIndex);
            Assert.True(Game.Trail.IsFirstLocation);
            Assert.Equal("Independence", Game.Trail.CurrentLocation.Name);
            Assert.IsType<Settlement>(Game.Trail.CurrentLocation);
        }

        [Fact]
        public void OregonTrail_EndsAtOregonCity()
        {
            var lastLocation = Game.Trail.Locations.Last();

            Assert.Equal("Oregon City", lastLocation.Name);
            Assert.True(lastLocation.LastLocation);

            // Only the final location may carry the last location flag.
            Assert.Single(Game.Trail.Locations, location => location.LastLocation);
        }

        [Fact]
        public void OregonTrail_HasFifteenTopLevelLocations()
        {
            Assert.Equal(15, Game.Trail.Locations.Count);
        }

        [Fact]
        public void NextLocation_FromStart_IsKansasRiverCrossing()
        {
            Assert.Equal("Kansas River Crossing", Game.Trail.NextLocation.Name);
        }

        [Fact]
        public void SegmentDistances_StayWithinRegistryBounds()
        {
            // TrailRegistry builds the Oregon Trail with segments between 32 and 164 miles.
            foreach (var location in Game.Trail.Locations)
                Assert.InRange(location.TotalDistance, 32, 164);
        }

        [Fact]
        public void TrailLength_CoversAtLeastTheTopLevelSegments()
        {
            // Total length also includes fork skip choices, so it can only be larger.
            var topLevelSum = Game.Trail.Locations.Sum(location => location.TotalDistance);

            Assert.True(Game.Trail.Length >= topLevelSum);
        }

        [Fact]
        public void ArriveAtNextLocation_OnFirstTurn_SetsUpStartingLocation()
        {
            Game.Trail.ArriveAtNextLocation();

            Assert.Equal(0, Game.Trail.LocationIndex);
            Assert.Equal(LocationStatusEnum.Arrived, Game.Trail.CurrentLocation.Status);
            Assert.Equal(Game.Trail.CurrentLocation.TotalDistance, Game.Trail.DistanceToNextLocation);
        }

        [Fact]
        public void ArriveAtNextLocation_AfterFirstTurn_AdvancesDownTheTrail()
        {
            Game.Trail.ArriveAtNextLocation();
            Game.TakeTurn(false);

            Game.Trail.ArriveAtNextLocation();

            Assert.Equal(1, Game.Trail.LocationIndex);
            Assert.False(Game.Trail.IsFirstLocation);
            Assert.Equal("Kansas River Crossing", Game.Trail.CurrentLocation.Name);
            Assert.Equal(LocationStatusEnum.Arrived, Game.Trail.CurrentLocation.Status);
        }

        [Fact]
        public void ArriveAtNextLocation_AtFinalLocation_IsNoOpInsteadOfThrowing()
        {
            // Get past the first-turn setup so the end-of-trail advance guard is active.
            Game.Trail.ArriveAtNextLocation();
            Game.TakeTurn(false);

            // Jump straight to the final location on the trail.
            var lastIndex = Game.Trail.Locations.Count - 1;
            typeof(TrailModule).GetProperty(nameof(TrailModule.LocationIndex)).SetValue(Game.Trail, lastIndex);
            Assert.True(Game.Trail.CurrentLocation.LastLocation);

            // Arriving again while already parked at the last location must be a harmless no-op. The old
            // '> Locations.Count' guard could never fire, so the index ran to Locations.Count and threw.
            var ex = Record.Exception(() => Game.Trail.ArriveAtNextLocation());

            Assert.Null(ex);
            Assert.Equal(lastIndex, Game.Trail.LocationIndex);
        }

        [Fact]
        public void InsertLocation_AddsLocationAfterCurrentOne()
        {
            var detour = new Landmark("Test Detour", ClimateEnum.Moderate);
            Game.Trail.InsertLocation(detour);

            Assert.Same(detour, Game.Trail.NextLocation);
            Assert.Equal(16, Game.Trail.Locations.Count);

            // Inserting must not steal the last location flag from Oregon City.
            Assert.True(Game.Trail.Locations.Last().LastLocation);
            Assert.False(detour.LastLocation);
        }

        [Fact]
        public void WinTrail_IsTwoLocationDebugTrail()
        {
            var winTrail = TrailRegistry.WinTrail;

            Assert.Equal(2, winTrail.Locations.Count);
            Assert.True(winTrail.Locations.Last().LastLocation);
        }

        [Fact]
        public void Trail_RejectsNullLocationList()
        {
            Assert.Throws<ArgumentNullException>(() => new TrailEntity(null, 32, 164));
        }

        [Fact]
        public void Trail_RejectsSingleLocationList()
        {
            var locations = new Location[] {new Landmark("Lonely Rock", ClimateEnum.Dry)};

            Assert.Throws<ArgumentException>(() => new TrailEntity(locations, 32, 164));
        }
    }
}
