using System.Collections.Generic;
using OregonTrailDotNet.Entity;
using OregonTrailDotNet.Entity.Person;
using OregonTrailDotNet.Event.Vehicle;
using OregonTrailDotNet.Event.Weather;
using OregonTrailDotNet.Module.Time;
using OregonTrailDotNet.Presentation.Audio;
using OregonTrailDotNet.Window.MainMenu;
using Xunit;

namespace OregonTrailDotNet.Tests
{
    /// <summary>
    ///     Pins the sound-effect funnel in EventExecutor. The 1990 DOS port sounded exactly two random events —
    ///     the severe-thunderstorm rumble and the wagon-part breakdown whoops (docs/legacy-sounds.md §1.2) — and
    ///     only for a human player: with presentation off (the bot and these suites) no effect may ever fire, and
    ///     with it on the noisiest-looking of the silent events, the hail storm, must stay silent too. The suite
    ///     runs muted throughout so asserting the funnel never actually plays anything; Sfx records the cue
    ///     regardless of the mute, which is what makes it observable here.
    /// </summary>
    public class SfxGateTests : SimulationTestBase
    {
        private readonly bool _wasMuted;

        public SfxGateTests()
        {
            _wasMuted = Music.Muted;
            if (!Music.Muted)
                Music.ToggleMute();
        }

        public override void Dispose()
        {
            GameSimulationApp.PresentationEnabled = false;
            if (Music.Muted != _wasMuted)
                Music.ToggleMute();
            base.Dispose();
        }

        /// <summary>Outfits a real party so the item-destroyer events have something to take.</summary>
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
        }

        [Fact]
        public void FlagOff_TheThunderstorm_FiresNoEffect()
        {
            BootParty();

            // Seed a cue the event funnel can never emit (muted, so it only records): LastCue is process-wide
            // and sticky, and asserting against whatever an earlier test left behind would let the exact
            // regression this test exists for — a dropped presentation gate — pass unnoticed.
            Sfx.Gunshot(100);

            Game.EventDirector.TriggerEvent(Game.Vehicle, typeof(SevereWeather));

            Assert.Equal("gunshot", Sfx.LastCue);
        }

        [Fact]
        public void FlagOn_TheThunderstorm_SoundsTheRumble()
        {
            BootParty();
            GameSimulationApp.PresentationEnabled = true;

            Game.EventDirector.TriggerEvent(Game.Vehicle, typeof(SevereWeather));

            Assert.Equal("thunderstorm", Sfx.LastCue);
        }

        [Fact]
        public void FlagOn_ABrokenPart_SoundsTheBreakdown()
        {
            BootParty();
            GameSimulationApp.PresentationEnabled = true;

            Game.EventDirector.TriggerEvent(Game.Vehicle, typeof(BrokenVehiclePart));

            Assert.Equal("breakdown", Sfx.LastCue);
        }

        [Fact]
        public void FlagOn_TheHailStorm_StaysSilent()
        {
            BootParty();
            GameSimulationApp.PresentationEnabled = true;

            // Same sentinel trick as the flag-off test: any cue the funnel wrongly fired would overwrite it.
            Sfx.Gunshot(100);

            Game.EventDirector.TriggerEvent(Game.Vehicle, typeof(HailStorm));

            Assert.Equal("gunshot", Sfx.LastCue);
        }
    }
}
