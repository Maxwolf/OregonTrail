using System.Reflection;
using OregonTrailDotNet.Entity;
using OregonTrailDotNet.Entity.Person;
using OregonTrailDotNet.Event.Vehicle;
using OregonTrailDotNet.Window.Travel.Hunt;
using Xunit;
using PersonEntity = OregonTrailDotNet.Entity.Person.Person;

namespace OregonTrailDotNet.Tests
{
    /// <summary>
    ///     Regression coverage for specific defects found during the code audit. Each test pins the exact
    ///     behavior that was wrong so the bug cannot silently return. Runs against a booted simulation because
    ///     all three paths reach through the game singleton (vehicle inventory, hunting, and the event window).
    /// </summary>
    public class RegressionTests : SimulationTestBase
    {
        /// <summary>
        ///     ConsumeFood runs once per living passenger each traveling day, and each call must only eat a single
        ///     person's ration share. The bug multiplied every call by the whole living-party count, so a party of N
        ///     ate ration * N^2 pounds per day instead of ration * N, starving larger parties far too fast.
        /// </summary>
        [Fact]
        public void ConsumeFood_PartyDailyConsumptionIsLinearInPartySize_NotQuadratic()
        {
            const int partySize = 5;

            var vehicle = Game.Vehicle;
            vehicle.ResetVehicle();
            for (var i = 0; i < partySize; i++)
                vehicle.AddPerson(new PersonEntity(ProfessionEnum.Farmer, $"P{i}", i == 0));

            vehicle.ChangeRations(RationLevelEnum.Filling);

            // Plenty of food so nobody hits the starvation branch during the tick and every passenger eats.
            vehicle.Inventory[EntitiesEnum.Food].AddQuantity(1000);
            var foodBefore = vehicle.Inventory[EntitiesEnum.Food].Quantity;

            // One traveling day: tick every passenger exactly once. The vehicle stays Stopped, so no mileage or
            // random events fire to perturb the food count - the only change is the day's eating.
            vehicle.OnTick(false, false);

            var consumed = foodBefore - vehicle.Inventory[EntitiesEnum.Food].Quantity;

            // Linear: 5 people * 1 lb (Filling) = 5 lb. The old quadratic bug consumed 5 * (1 * 5) = 25 lb.
            Assert.Equal(partySize * (int) RationLevelEnum.Filling, consumed);
        }

        [Fact]
        public void NoFood_StarvesAnOxEachDay()
        {
            var vehicle = Game.Vehicle;
            vehicle.ResetVehicle();
            vehicle.AddPerson(new PersonEntity(ProfessionEnum.Farmer, "Leader", true));

            // ResetVehicle zeroes the larder, so the party has no food. Give them a team of oxen.
            vehicle.Inventory[EntitiesEnum.Animal].AddQuantity(6);
            Assert.Equal(0, vehicle.Inventory[EntitiesEnum.Food].Quantity);

            // One real day with an empty larder: the oxen go hungry and the team loses one.
            vehicle.OnTick(false, false);

            Assert.Equal(5, vehicle.Inventory[EntitiesEnum.Animal].Quantity);
        }

        [Fact]
        public void WithFood_OxenAreNotStarved()
        {
            var vehicle = Game.Vehicle;
            vehicle.ResetVehicle();
            vehicle.AddPerson(new PersonEntity(ProfessionEnum.Farmer, "Leader", true));

            vehicle.Inventory[EntitiesEnum.Animal].AddQuantity(6);
            vehicle.Inventory[EntitiesEnum.Food].AddQuantity(1000);

            // A well-fed party keeps its whole team - food covers the animals too.
            vehicle.OnTick(false, false);

            Assert.Equal(6, vehicle.Inventory[EntitiesEnum.Animal].Quantity);
        }

        /// <summary>
        ///     A successful hunting shot must strictly consume bullets. The bug derived the cost from the ammo stack's
        ///     monetary TotalValue and could produce a negative amount, which ReduceQuantity turned into a gain (clamped
        ///     up to the maximum) - so bagging an animal could add ammunition or, when large, wipe the stack to zero.
        /// </summary>
        [Fact]
        public void TryShoot_SuccessfulKill_StrictlyConsumesTenToThirteenBullets()
        {
            var vehicle = Game.Vehicle;
            vehicle.ResetVehicle();

            // The miss roll reads the party leader's profession, so the vehicle needs a leader aboard.
            vehicle.AddPerson(new PersonEntity(ProfessionEnum.Farmer, "Hunter", true));

            var ammo = vehicle.Inventory[EntitiesEnum.Ammo];
            ammo.AddQuantity(ammo.MaxQuantity); // top the stack off to a known amount that a single shot cannot floor.
            var ammoBefore = ammo.Quantity;
            Assert.True(ammoBefore > 13);

            var hunt = new HuntManager();

            // A freshly generated prey has TargetTime 0, so it passes both the miss roll and the on-time-shot gate,
            // deterministically reaching the ammunition-deduction path. The target field is private; set it directly.
            typeof(HuntManager)
                .GetField("_target", BindingFlags.NonPublic | BindingFlags.Instance)
                .SetValue(hunt, new PreyItem());

            Assert.True(hunt.TryShoot());

            var consumed = ammoBefore - ammo.Quantity;
            Assert.InRange(consumed, 10, 13);
            Assert.True(ammo.Quantity < ammoBefore, "a kill must never increase ammunition");
            Assert.True(ammo.Quantity >= 0);
        }

        /// <summary>
        ///     A lose-time event must attach the day-skipping form so the days it declares actually elapse. The bug
        ///     inverted the guard in LoseTime.OnPostExecute, so the form was never attached and every time-loss event
        ///     (stuck in mud, impassable trail, blizzard, etc.) silently cost zero days. StuckInMud always skips one.
        /// </summary>
        [Fact]
        public void LoseTimeEvent_ActuallyAdvancesTheCalendarByTheSkippedDays()
        {
            var daysBefore = Game.Time.TotalDays;

            // Executes synchronously: the event's Execute + OnPostExecute run inside TriggerEvent, attaching EventSkipDay.
            Game.EventDirector.TriggerEvent(Game.Vehicle, typeof(StuckInMud));

            // EventSkipDay ticks the calendar forward one day per simulation tick until its counter reaches zero.
            // Game.OnTick(false) never advances the calendar on its own, so the change is attributable to the skip.
            for (var i = 0; i < 5; i++)
                Game.OnTick(false);

            Assert.Equal(daysBefore + 1, Game.Time.TotalDays);
        }
    }
}
