using System.Reflection;
using OregonTrailDotNet.Entity;
using OregonTrailDotNet.Window.Travel;
using OregonTrailDotNet.Window.Travel.RiverCrossing;
using Xunit;

namespace OregonTrailDotNet.Tests
{
    /// <summary>
    ///     Characterization tests for the river-crossing simulation, written before its extraction so the graphical
    ///     crossing scene provably runs the identical logic. They pin the subtleties an extraction could silently
    ///     drop: the ferry charges only on strictly-more cash while the Indian guide takes an exact payment, the
    ///     finishing tick freezes progress without another turn, and ENTER does nothing until the far bank.
    /// </summary>
    public class CrossingSimulationTests : SimulationTestBase
    {
        /// <summary>
        ///     Walks the boot state to the Kansas River crossing (the first river on the trail) and builds a live
        ///     crossing form on it, mirroring how HuntScreenTests spins up hunts.
        /// </summary>
        private (Travel Window, TravelInfo Data, CrossingTick Form) StartCrossing(RiverCrossChoiceEnum type)
        {
            // The first turn sets up the world; the arrival after it steps onto the Kansas River.
            Game.TakeTurn(false);
            Game.Trail.ArriveAtNextLocation();
            Assert.Equal("Kansas River Crossing", Game.Trail.CurrentLocation.Name);

            var window = new Travel(GameSimulationApp.Instance);
            var data = (TravelInfo) window.GetType().BaseType!
                .GetProperty("UserData", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)!
                .GetValue(window)!;
            data.GenerateRiver();
            data.River.CrossingType = type;
            var form = new CrossingTick(window);
            return (window, data, form);
        }

        /// <summary>Sets an inventory item to an exact quantity regardless of its starting value.</summary>
        private static void SetQuantity(EntitiesEnum entity, int quantity)
        {
            var item = GameSimulationApp.Instance.Vehicle.Inventory[entity];
            item.ReduceQuantity(int.MaxValue);
            item.AddQuantity(quantity);
        }

        [Fact]
        public void Ferry_DoesNotCharge_WhenCashOnlyEqualsTheFare()
        {
            var (_, data, form) = StartCrossing(RiverCrossChoiceEnum.Ferry);
            data.River.FerryCost = 5;
            SetQuantity(EntitiesEnum.Cash, 5);

            form.OnFormPostCreate();

            // Strictly-greater-than: an exact fare is not taken, and the cost stands unpaid.
            Assert.Equal(5, GameSimulationApp.Instance.Vehicle.Inventory[EntitiesEnum.Cash].Quantity);
            Assert.Equal(5, data.River.FerryCost);
        }

        [Fact]
        public void Ferry_Charges_WhenCashExceedsTheFare()
        {
            var (_, data, form) = StartCrossing(RiverCrossChoiceEnum.Ferry);
            data.River.FerryCost = 5;
            SetQuantity(EntitiesEnum.Cash, 6);

            form.OnFormPostCreate();

            Assert.Equal(1, GameSimulationApp.Instance.Vehicle.Inventory[EntitiesEnum.Cash].Quantity);
            Assert.Equal(0, data.River.FerryCost);
        }

        [Fact]
        public void IndianGuide_TakesAnExactPayment()
        {
            var (_, data, form) = StartCrossing(RiverCrossChoiceEnum.Indian);
            data.River.FerryCost = 0;
            data.River.IndianCost = 2;
            SetQuantity(EntitiesEnum.Clothes, 2);

            form.OnFormPostCreate();

            // Greater-or-equal: a party with exactly the asking price pays it, matching the offer gate.
            Assert.Equal(0, GameSimulationApp.Instance.Vehicle.Inventory[EntitiesEnum.Clothes].Quantity);
            Assert.Equal(0, data.River.IndianCost);
        }

        [Fact]
        public void Crossing_Finishes_FreezesProgress_AndGatesEnterOnTheFarBank()
        {
            // Ferry, not Ford: CrossingResult's Ford branch rolls its own 50/50 and can auto-finish straight to
            // the departure screen inside its prompt, which would make the final assertion a coin flip.
            var (window, data, form) = StartCrossing(RiverCrossChoiceEnum.Ferry);
            data.River.FerryCost = 0;
            form.OnFormPostCreate();

            // ENTER mid-river does nothing — the form stays put until the far bank.
            form.OnInputBufferReturned(string.Empty);
            Assert.Null(window.CurrentForm);

            // The river is at most width feet at 1+ feet a tick; the loop bound is generous.
            var width = data.River.RiverWidth;
            var finished = $"River crossed: {width:N0} feet";
            var ticks = 0;
            while (!form.OnRenderForm().Contains(finished) && ticks++ < width + 10)
                form.OnTick(false, false);
            Assert.Contains(finished, form.OnRenderForm());

            // The finishing tick freezes the crossing: further ticks change nothing.
            form.OnTick(false, false);
            form.OnTick(false, false);
            Assert.Contains(finished, form.OnRenderForm());

            // And only now does ENTER advance to the result.
            form.OnInputBufferReturned(string.Empty);
            Assert.IsType<CrossingResult>(window.CurrentForm);
        }
    }
}
