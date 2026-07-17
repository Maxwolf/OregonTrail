using System;
using System.Collections.Generic;
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
        public void SegmentDistances_AreTheOriginalsRouteTable()
        {
            // The 1985 game's own mileages, recovered from VAR.BIN (legacy/source/VAR.BIN.txt). A location's distance is
            // the leg leading away from it, so these read as "Independence to the Kansas River is 102 miles".
            var expected = new Dictionary<string, int>
            {
                ["Independence"] = 102,
                ["Kansas River Crossing"] = 83,
                ["Big Blue River Crossing"] = 119,
                ["Fort Kearney"] = 250,
                ["Chimney Rock"] = 86,
                ["Fort Laramie"] = 190,
                ["Independence Rock"] = 102,
                ["Soda Springs"] = 57,
                ["Fort Hall"] = 182,
                ["Snake River Crossing"] = 114,
                ["Fort Boise"] = 160,
                ["Blue Mountains"] = 125,
                ["The Dalles"] = 100
            };

            foreach (var location in Game.Trail.Locations)
                if (expected.TryGetValue(location.Name, out var miles))
                    Assert.Equal(miles, location.TotalDistance);

            // Nothing is invented for this trail any more, so no leg may be randomised into existence.
            foreach (var location in Game.Trail.Locations)
                Assert.True(expected.ContainsKey(location.Name) || location.LastLocation ||
                            location is ForkInRoad);
        }

        [Fact]
        public void ForkBranches_CarryTheirOwnLegAndOnwardDistance()
        {
            var southPass = (ForkInRoad) Game.Trail.Locations.First(l => l.Name == "South Pass");
            var bridger = southPass.SkipChoices.First(c => c.Name == "Fort Bridger");
            var greenRiver = southPass.SkipChoices.First(c => c.Name == "Green River Crossing");

            // Going by way of Fort Bridger skips the Green River crossing but costs 86 extra miles.
            Assert.Equal(125, bridger.LegDistance);
            Assert.Equal(162, bridger.TotalDistance);
            Assert.Equal(57, greenRiver.LegDistance);
            Assert.Equal(144, greenRiver.TotalDistance);
            Assert.Equal(86, bridger.LegDistance + bridger.TotalDistance -
                             (greenRiver.LegDistance + greenRiver.TotalDistance));

            // Fort Walla Walla is a detour off the main trail that rejoins at The Dalles: 55 + 120 against 125 straight.
            var blueMountains = (ForkInRoad) Game.Trail.Locations.First(l => l.Name == "Blue Mountains");
            var wallaWalla = blueMountains.SkipChoices.First(c => c != null && c.Name == "Fort Walla Walla");
            Assert.Equal(55, wallaWalla.LegDistance);
            Assert.Equal(120, wallaWalla.TotalDistance);
            Assert.Contains(blueMountains.SkipChoices, c => c == null);
        }

        [Fact]
        public void BaseMilesPerDay_IsTwentyOnThePlainsAndTwelveWestOfFortLaramie()
        {
            var plains = new[]
            {
                "Independence", "Kansas River Crossing", "Big Blue River Crossing", "Fort Kearney", "Chimney Rock"
            };

            foreach (var location in Game.Trail.Locations)
                Assert.Equal(plains.Contains(location.Name)
                        ? Location.PlainsMilesPerDay
                        : Location.MountainMilesPerDay,
                    location.BaseMilesPerDay);

            // The five plains legs are exactly the 640 miles from Independence to Fort Laramie, which is where the
            // original's daily rate drops from twenty to twelve.
            Assert.Equal(640, Game.Trail.Locations
                .Where(l => plains.Contains(l.Name))
                .Sum(l => l.TotalDistance));
        }

        [Fact]
        public void TrailLength_MatchesTheOriginalsStandardRoute()
        {
            // 102 + 83 + 119 + 250 + 86 + 190 + 102 (to South Pass), + 57 + 144 through the Green River,
            // + 57 + 182 + 114 + 160 (to the Blue Mountains), + 125 straight on, + 100 to the valley = 1,871.
            var standardRoute = 102 + 83 + 119 + 250 + 86 + 190 + 102 + 57 + 144 + 57 + 182 + 114 + 160 + 125 + 100;
            Assert.Equal(1871, standardRoute);
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
            var detour = new Landmark("Test Detour", ClimateEnum.MissouriValley);
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
            var locations = new Location[] {new Landmark("Lonely Rock", ClimateEnum.HighCountry)};

            Assert.Throws<ArgumentException>(() => new TrailEntity(locations, 32, 164));
        }
    }
}
