using System.Linq;
using System.Reflection;
using OregonTrailDotNet.Entity;
using OregonTrailDotNet.Entity.Item;
using OregonTrailDotNet.Entity.Location;
using OregonTrailDotNet.Entity.Location.Point;
using OregonTrailDotNet.Entity.Location.Weather;
using OregonTrailDotNet.Entity.Person;
using OregonTrailDotNet.Event;
using OregonTrailDotNet.Event.Prefab;
using OregonTrailDotNet.Event.Vehicle;
using OregonTrailDotNet.Event.Weather;
using OregonTrailDotNet.Module.Director;
using OregonTrailDotNet.Module.Time;
using OregonTrailDotNet.Window.Travel.Hunt;
using WolfCurses.Utility;
using Xunit;
using VehicleEntity = OregonTrailDotNet.Entity.Vehicle.Vehicle;

namespace OregonTrailDotNet.Tests
{
    /// <summary>
    ///     Covers the simulation features that were added to bring the game in line with the behaviors listed in the README's
    ///     "Simulation Features" section: the day cap, fort pricing, mountain hazards, hunting/trade tallies, medical supplies,
    ///     weighted events, and death/win screen data. The randomizer is not seedable so these assert deterministic values and
    ///     invariant ranges rather than exact random outcomes.
    /// </summary>
    public class ReadmeFeaturesTests : SimulationTestBase
    {
        // ---- Travel #3 / Winning #1: journey day tracking and the 20+ week cap ----

        [Fact]
        public void TimeModule_ExposesTotalDaysAndMaxTravelDays()
        {
            Assert.Equal(246, TimeModule.MaxTravelDays);
            Assert.True(Game.Time.TotalDays >= 0);
        }

        // ---- Mountains #2/#3: high-ground passes carry a stuck chance ----

        [Fact]
        public void Location_HighGroundAndStuckChanceDefaultToOff()
        {
            var landmark = new Landmark("Test Rock", Climate.Moderate);

            Assert.False(landmark.HighGround);
            Assert.Equal(0, landmark.StuckChance);
        }

        [Fact]
        public void TrailRegistry_MountainPassesAreHighGroundWithStuckChance()
        {
            var locations = Game.Trail.Locations;

            var southPass = locations.First(location => location.Name == "South Pass");
            Assert.True(southPass.HighGround);
            Assert.Equal(80, southPass.StuckChance);

            var blueMountains = locations.First(location => location.Name == "Blue Mountains");
            Assert.True(blueMountains.HighGround);
            Assert.Equal(70, blueMountains.StuckChance);

            // A normal settlement is not high ground and has no stuck chance.
            var independence = locations.First(location => location.Name == "Independence");
            Assert.False(independence.HighGround);
            Assert.Equal(0, independence.StuckChance);
        }

        // ---- Mountains #1/#4/#5: the new lose-time events exist and are bounded ----

        [Fact]
        public void MountainEvents_AreRegisteredLoseTimeEvents()
        {
            var factory = new EventFactory();

            foreach (var eventType in new[] {typeof(StuckInMountains), typeof(CaveIn), typeof(Blizzard)})
            {
                var created = factory.CreateInstance(eventType);
                Assert.NotNull(created);
                Assert.IsAssignableFrom<LoseTime>(created);

                // All three are manual-only so they never show up in random category rolls.
                var attribute = eventType.GetCustomAttributes<DirectorEventAttribute>(false).First();
                Assert.Equal(EventExecution.ManualOnly, attribute.EventExecutionType);
            }
        }

        [Fact]
        public void StuckInMountains_NeverStrandsPartyMoreThanTenDays()
        {
            var stuck = new StuckInMountains();
            var daysToSkip = typeof(StuckInMountains)
                .GetMethod("DaysToSkip", BindingFlags.Instance | BindingFlags.NonPublic);

            // Randomizer is not seedable, so sample the roll many times and assert the 10-day ceiling never breaks.
            for (var i = 0; i < 1000; i++)
            {
                var days = (int) daysToSkip.Invoke(stuck, null);
                Assert.InRange(days, 1, 10);
            }
        }

        // ---- Random Events #2/#3: per-event probability weighting ----

        [Fact]
        public void DirectorEventAttribute_DefaultsToEqualWeightAndClampsNegatives()
        {
            Assert.Equal(100, new DirectorEventAttribute(EventCategory.Person).EventProbability);
            Assert.Equal(25, new DirectorEventAttribute(EventCategory.Person, EventExecution.RandomOrManual, 25)
                .EventProbability);
            Assert.Equal(0, new DirectorEventAttribute(EventCategory.Person, EventExecution.RandomOrManual, -5)
                .EventProbability);
        }

        [Fact]
        public void EventFactory_WeightedSelection_StillHonorsRequestedCategory()
        {
            var factory = new EventFactory();

            // Weighted cumulative selection must only ever return an event of the requested category.
            for (var i = 0; i < 200; i++)
            {
                var created = factory.CreateRandomByType(EventCategory.Vehicle);
                Assert.NotNull(created);
                var attribute = created.GetType().GetCustomAttributes<DirectorEventAttribute>(false).First();
                Assert.Equal(EventCategory.Vehicle, attribute.EventCategory);
            }
        }

        // ---- Travel #20/#21: hunting tally and food cap ----

        [Fact]
        public void HuntManager_CarryCapIsTwoHundredFiftyPounds()
        {
            Assert.Equal(250, HuntManager.MAXFOOD);
        }

        [Fact]
        public void Vehicle_AnimalsKilledStartsAtZeroIncrementsAndResets()
        {
            var vehicle = new VehicleEntity();
            Assert.Equal(0, vehicle.AnimalsKilled);

            vehicle.IncrementAnimalKillCount();
            vehicle.IncrementAnimalKillCount();
            Assert.Equal(2, vehicle.AnimalsKilled);

            vehicle.ResetVehicle(100);
            Assert.Equal(0, vehicle.AnimalsKilled);
        }

        // ---- Travel #10: departing a fort slows the next travel turn ----

        [Fact]
        public void Vehicle_FortDeparturePenaltyConsumedOnNextMovingTick()
        {
            var vehicle = new VehicleEntity();
            vehicle.Status = OregonTrailDotNet.Entity.Vehicle.VehicleStatus.Moving;
            vehicle.FortDeparturePenalty = true;

            vehicle.OnTick(false, false);

            // The penalty is spent on this moving tick (and cut mileage stays a valid, non-negative figure).
            Assert.False(vehicle.FortDeparturePenalty);
            Assert.True(vehicle.Mileage >= 0);
        }

        [Fact]
        public void Vehicle_FortDeparturePenaltyRetainedWhileStopped()
        {
            var vehicle = new VehicleEntity();
            vehicle.Status = OregonTrailDotNet.Entity.Vehicle.VehicleStatus.Stopped;
            vehicle.FortDeparturePenalty = true;

            vehicle.OnTick(false, false);

            // A stopped vehicle does not travel, so the pending penalty is preserved for the next real turn.
            Assert.True(vehicle.FortDeparturePenalty);
        }

        // ---- Illness #3: medical supplies are a real, purchasable item ----

        [Fact]
        public void Medicine_IsADistinctPurchasableInventoryItem()
        {
            Assert.Equal(Entities.Medicine, Resources.Medicine.Category);

            var vehicle = new VehicleEntity();
            Assert.True(vehicle.Inventory.ContainsKey(Entities.Medicine));
        }

        // ---- Travel #16 + #15/#17: prices align to the README and climb at each fort ----

        [Fact]
        public void StorePrices_StartAtBaseValuesBeforeAnyFortDeparted()
        {
            // Booted simulation has every location Unreached, so no forts have been departed yet.
            Assert.Equal(0.10, (double) Resources.Food.Cost, 2);
            Assert.Equal(10f, Resources.Clothing.Cost);
            Assert.Equal(2f, Resources.Bullets.Cost);
            Assert.Equal(20f, Parts.Oxen.Cost);
            Assert.Equal(10f, Parts.Axle.Cost);
        }

        [Fact]
        public void StorePrices_RiseByPerFortDeltaAfterDepartingASettlement()
        {
            // Mark the first settlement on the trail as departed to simulate leaving one fort behind.
            var firstFort = Game.Trail.Locations.First(location => location is Settlement);
            firstFort.Status = LocationStatus.Departed;

            Assert.Equal(0.20, (double) Resources.Food.Cost, 2); // +$0.10 per fort
            Assert.Equal(12.5f, Resources.Clothing.Cost); // +$2.5 per fort
            Assert.Equal(4.5f, Resources.Bullets.Cost); // +$2.5 per fort
            Assert.Equal(25f, Parts.Oxen.Cost); // +$5 per fort
            Assert.Equal(12.5f, Parts.Axle.Cost); // +$2.5 per fort
        }

        // ---- Death #2: cause of death renders a human-readable reason ----

        [Fact]
        public void CauseOfDeath_UnknownIsBlankAndOthersDescribeThemselves()
        {
            Assert.Equal(string.Empty, CauseOfDeath.Unknown.ToDescriptionAttribute());
            Assert.False(string.IsNullOrEmpty(CauseOfDeath.Starvation.ToDescriptionAttribute()));
            Assert.False(string.IsNullOrEmpty(CauseOfDeath.Illness.ToDescriptionAttribute()));
        }

        // ---- Input: spaces are accepted (e.g. tombstone epitaphs) despite the WolfCurses letter/digit filter ----

        [Fact]
        public void InputBuffer_AcceptsSpaces_SoEpitaphsCanContainThem()
        {
            // WolfCurses' AddCharToInputBuffer now accepts any printable character (not just letters/digits), so spaces reach
            // the input buffer for free-text forms such as the tombstone epitaph editor. Regression guard in case a future
            // WolfCurses release reintroduces the letter/digit-only filter.
            Game.InputManager.ClearBuffer();
            Game.InputManager.AddCharToInputBuffer('H');
            Game.InputManager.AddCharToInputBuffer('i');
            Game.InputManager.AddCharToInputBuffer(' ');
            Game.InputManager.AddCharToInputBuffer('u');

            Assert.Equal("Hi u", Game.InputManager.InputBuffer);
        }
    }
}
