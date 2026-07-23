using System.Collections.Generic;
using OregonTrailDotNet.Entity;
using OregonTrailDotNet.Entity.Person;
using OregonTrailDotNet.Entity.Vehicle;
using OregonTrailDotNet.Event.Weather;
using OregonTrailDotNet.Module.Director;
using OregonTrailDotNet.Module.Time;
using OregonTrailDotNet.Window.MainMenu;
using Xunit;

namespace OregonTrailDotNet.Tests
{
    /// <summary>
    ///     The four weather events were dead content for years — nothing ever rolled or triggered them — and code
    ///     that just came back from the dead is where latent crashes live. Each execution test asserts evidence
    ///     only a real execution produces (the random-event window with the right form attached — a blizzard's
    ///     skip-day countdown proves its days actually landed), and the gate tests pin both halves of the design
    ///     rule that revived them: a stopped party is never struck, and a storm parks the wagon so the rest days
    ///     that follow are storm-proof.
    /// </summary>
    public class WeatherEventTests : SimulationTestBase
    {
        /// <summary>Outfits a real party with enough aboard for an item destroyer to have something to take.</summary>
        private void BootParty()
        {
            Game.SetStartInfo(new NewGameInfo
            {
                PlayerNames = new List<string> { "Alice", "Bob", "Carol", "Dave" },
                PlayerProfession = ProfessionEnum.Banker,
                StartingMonies = 1600,
                StartingMonth = MonthEnum.April
            });

            Game.Vehicle.Inventory[EntitiesEnum.Food].AddQuantity(200);
            Game.Vehicle.Inventory[EntitiesEnum.Clothes].AddQuantity(20);
            Game.Vehicle.Inventory[EntitiesEnum.Ammo].AddQuantity(100);
            Game.Vehicle.Inventory[EntitiesEnum.Animal].AddQuantity(4);
        }

        [Theory]
        [InlineData(typeof(Blizzard), "EventSkipDay", true)]
        [InlineData(typeof(HailStorm), "EventExecutor", false)]
        [InlineData(typeof(SevereWeather), "EventExecutor", false)]
        [InlineData(typeof(HeavyFog), "EventExecutor", true)]
        public void EachWeatherEvent_ExecutesAgainstTheVehicle(System.Type eventType, string expectedForm,
            bool survivalGuaranteed)
        {
            BootParty();

            Game.EventDirector.TriggerEvent(Game.Vehicle, eventType);

            // Evidence only an execution produces: the random-event window is up with the form the event's own
            // flow chooses — a blizzard's lost days attach the skip-day countdown (proof DaysToSkip landed), the
            // others hold the executor's message. The recorder is secondary (it writes before execution).
            var window = Game.WindowManager.FocusedWindow;
            Assert.NotNull(window);
            Assert.Equal("RandomEvent", window.GetType().Name);
            Assert.Equal(expectedForm, window.CurrentForm?.GetType().Name);
            Assert.Equal(eventType.Name, SceneEvents.LastEventName);

            // The item destroyers can legitimately freeze passengers (hail kills at 15% a head when it takes
            // items), so only the lose-time events assert survival.
            if (survivalGuaranteed)
                Assert.False(Game.Vehicle.PassengersDead);
        }

        [Fact]
        public void ABlizzard_ParksTheWagon()
        {
            BootParty();
            Game.Vehicle.Status = VehicleStatusEnum.Moving;

            Game.EventDirector.TriggerEvent(Game.Vehicle, typeof(Blizzard));

            // The other half of the no-chaining rule: the skip-day form stops the wagon, and a stopped wagon is
            // exempt from the sky (the invariant below) — so the storm's own rest days can never roll a second
            // storm on top of the first.
            Assert.Equal(VehicleStatusEnum.Stopped, Game.Vehicle.Status);
        }

        [Fact]
        public void AStoppedParty_IsNeverStruckByTheSky()
        {
            BootParty();
            Game.Vehicle.Status = VehicleStatusEnum.Stopped;

            var nameBefore = SceneEvents.LastEventName;
            var turnBefore = SceneEvents.LastEventTurn;

            // Whatever the sky does across a year of days, a party that is not moving is not exposed. The
            // climate tick is the only seam that fires weather, so an unchanged record is proof of silence.
            for (var day = 0; day < 366; day++)
                Game.Climate.Tick();

            Assert.Equal(nameBefore, SceneEvents.LastEventName);
            Assert.Equal(turnBefore, SceneEvents.LastEventTurn);
        }
    }
}
