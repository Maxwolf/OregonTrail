using System.Collections.Generic;
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
using OregonTrailDotNet.Window.MainMenu;
using OregonTrailDotNet.Window.Travel.Hunt;
using WolfCurses.Utility;
using Xunit;
using TravelWindow = OregonTrailDotNet.Window.Travel.Travel;
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
        // ---- Travel #3: journey day tracking. MaxTravelDays survives as the bot's pacing horizon only — the 1985 game
        // had no time limit (a party could idle for years and still finish) and neither does this port.

        [Fact]
        public void TimeModule_ExposesTotalDaysAndMaxTravelDays()
        {
            Assert.Equal(246, TimeModule.MaxTravelDays);
            Assert.True(Game.Time.TotalDays >= 0);
        }

        [Fact]
        public void Journey_DoesNotEndAtMaxTravelDays()
        {
            // Boot a real party, then advance the clock far past the old 246-day forced ending. Keep the party fed and
            // clothed while the clock runs — the test is about the calendar, and a starved-out party would legitimately
            // end the game through the death path instead.
            Game.SetStartInfo(new NewGameInfo
            {
                PlayerNames = new List<string> {"Alice", "Bob", "Carol", "Dan", "Eve"},
                PlayerProfession = ProfessionEnum.Farmer,
                StartingMonies = 400,
                StartingMonth = MonthEnum.April
            });
            Game.Vehicle.Inventory[EntitiesEnum.Clothes].AddQuantity(255);
            for (var day = 0; day < TimeModule.MaxTravelDays + 30; day++)
            {
                Game.Vehicle.Inventory[EntitiesEnum.Food].AddQuantity(200);
                Game.TakeTurn(false);
            }

            Assert.True(Game.Time.TotalDays > TimeModule.MaxTravelDays);
            Assert.False(Game.Vehicle.PassengersDead); // a fed, clothed, idle party survives the long calendar

            // Activate the travel window — the exact seam where the old day cap forced the end-game routine — and let
            // any queued window attach. With a living party short of Oregon, nothing may end the game anymore.
            var travel = new TravelWindow(GameSimulationApp.Instance);
            travel.OnWindowActivate();
            Game.OnTick(false);

            Assert.NotEqual("GameOver", Game.WindowManager.FocusedWindow?.GetType().Name);
        }

        // ---- Mountains #2/#3: high-ground passes carry a stuck chance ----

        [Fact]
        public void Location_HighGroundAndStuckChanceDefaultToOff()
        {
            var landmark = new Landmark("Test Rock", ClimateEnum.MissouriValley);

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
                Assert.Equal(EventExecutionEnum.ManualOnly, attribute.EventExecutionType);
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
            Assert.Equal(100, new DirectorEventAttribute(EventCategoryEnum.Person).EventProbability);
            Assert.Equal(25, new DirectorEventAttribute(EventCategoryEnum.Person, EventExecutionEnum.RandomOrManual, 25)
                .EventProbability);
            Assert.Equal(0, new DirectorEventAttribute(EventCategoryEnum.Person, EventExecutionEnum.RandomOrManual, -5)
                .EventProbability);
        }

        [Fact]
        public void EventFactory_WeightedSelection_StillHonorsRequestedCategory()
        {
            var factory = new EventFactory();

            // Weighted cumulative selection must only ever return an event of the requested category.
            for (var i = 0; i < 200; i++)
            {
                var created = factory.CreateRandomByType(EventCategoryEnum.Vehicle);
                Assert.NotNull(created);
                var attribute = created.GetType().GetCustomAttributes<DirectorEventAttribute>(false).First();
                Assert.Equal(EventCategoryEnum.Vehicle, attribute.EventCategory);
            }
        }

        // ---- Travel #20/#21: hunting tally and food cap ----

        [Fact]
        public void HuntManager_CarryCapIsOneHundredPounds()
        {
            Assert.Equal(100, HuntManager.MAXFOOD);
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
            vehicle.Status = OregonTrailDotNet.Entity.Vehicle.VehicleStatusEnum.Moving;
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
            vehicle.Status = OregonTrailDotNet.Entity.Vehicle.VehicleStatusEnum.Stopped;
            vehicle.FortDeparturePenalty = true;

            vehicle.OnTick(false, false);

            // A stopped vehicle does not travel, so the pending penalty is preserved for the next real turn.
            Assert.True(vehicle.FortDeparturePenalty);
        }

        // ---- Illness #3: medical supplies are a real, purchasable item ----

        [Fact]
        public void Medicine_IsADistinctPurchasableInventoryItem()
        {
            Assert.Equal(EntitiesEnum.Medicine, Resources.Medicine.Category);

            var vehicle = new VehicleEntity();
            Assert.True(vehicle.Inventory.ContainsKey(EntitiesEnum.Medicine));
        }

        // ---- Travel #16 + #15/#17: prices align to the README and climb at each fort ----

        [Fact]
        public void StorePrices_StartAtBaseValuesBeforeAnyFortDeparted()
        {
            // Booted simulation has every location Unreached, so no forts have been departed yet. These are the prices
            // Matt's General Store quotes in the 1985 original, read out of its VAR.BIN price table.
            Assert.Equal(0.20, (double) Resources.Food.Cost, 2); // $0.20 per pound
            Assert.Equal(10f, Resources.Clothing.Cost); // $10.00 per set
            Assert.Equal(0.10, (double) Resources.Bullets.Cost, 2); // $2.00 per box of 20 bullets
            Assert.Equal(20f, Parts.Oxen.Cost); // $40.00 per yoke of two
            Assert.Equal(10f, Parts.Axle.Cost);
        }

        [Fact]
        public void StorePrices_RiseByAQuarterOfBaseAfterDepartingASettlement()
        {
            // Mark the first settlement on the trail as departed to simulate leaving one fort behind.
            var firstFort = Game.Trail.Locations.First(location => location is Settlement);
            firstFort.Status = LocationStatusEnum.Departed;

            // The original marked every quoted price up by a quarter of its base at each fort: cost * (1 + .25 * Q).
            Assert.Equal(0.25, (double) Resources.Food.Cost, 2);
            Assert.Equal(12.5f, Resources.Clothing.Cost);
            Assert.Equal(0.125, (double) Resources.Bullets.Cost, 3);
            Assert.Equal(25f, Parts.Oxen.Cost);
            Assert.Equal(12.5f, Parts.Axle.Cost);
        }

        [Fact]
        public void StorePrices_StopClimbingAfterTheOriginalsSixThresholds()
        {
            // Departing every settlement on the trail reaches (and, once fork branches insert their own forts, exceeds)
            // the original's six price thresholds. The markup must stop at 2.5x base either way.
            foreach (var fort in Game.Trail.Locations.Where(location => location is Settlement))
                fort.Status = LocationStatusEnum.Departed;

            Assert.Equal(25f, Resources.Clothing.Cost); // 10 * (1 + .25 * 6)
            Assert.Equal(50f, Parts.Oxen.Cost); // 20 * (1 + .25 * 6)
            Assert.Equal(0.50, (double) Resources.Food.Cost, 2); // 0.20 * (1 + .25 * 6)
        }

        // ---- Death #2: cause of death renders a human-readable reason ----

        [Fact]
        public void CauseOfDeath_UnknownIsBlankAndOthersDescribeThemselves()
        {
            Assert.Equal(string.Empty, CauseOfDeathEnum.Unknown.ToDescriptionAttribute());
            Assert.False(string.IsNullOrEmpty(CauseOfDeathEnum.Starvation.ToDescriptionAttribute()));
            Assert.False(string.IsNullOrEmpty(CauseOfDeathEnum.Illness.ToDescriptionAttribute()));
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
