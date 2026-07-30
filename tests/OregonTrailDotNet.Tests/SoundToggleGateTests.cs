using OregonTrailDotNet.Presentation.Audio;
using OregonTrailDotNet.Window.MainMenu;
using Xunit;

namespace OregonTrailDotNet.Tests
{
    /// <summary>
    ///     Pins the main menu's sound toggle - the original's "Turn sound off": printed as option 4 exactly as the
    ///     DOS port numbered it, with management options at 5 and "End" last at 6. The menu is the same six choices
    ///     for every host, because a choice belongs to the game and not to the drawing; the presentation flag only
    ///     decides whether sound is audible. Toggling flips the one process-wide mute and is read back through the
    ///     masthead, since a command's label cannot change. Nothing here plays a cue, so no device is ever opened.
    /// </summary>
    public class SoundToggleGateTests : SimulationTestBase
    {
        /// <summary>The whole menu, in printed order, as both hosts must show it.</summary>
        private static readonly string[] Menu =
        {
            "1. Travel the trail",
            "2. Learn about the trail",
            "3. See the Oregon Top Ten",
            "4. Turn sound on/off",
            "5. Choose Management Options",
            "6. End"
        };

        private readonly bool _wasMuted;

        public SoundToggleGateTests()
        {
            // Start unmuted so "toggle → sound off" reads deterministically; restored in Dispose.
            _wasMuted = Music.Muted;
            if (Music.Muted)
                Music.ToggleMute();
        }

        public override void Dispose()
        {
            GameSimulationApp.PresentationEnabled = false;
            if (Music.Muted != _wasMuted)
                Music.ToggleMute();
            base.Dispose();
        }

        [Fact]
        public void BothHosts_OfferTheSameSixChoices_SoundAtFourAndEndLast()
        {
            var headless = new MainMenu(GameSimulationApp.Instance);
            headless.OnWindowPostCreate();
            var headlessMenu = headless.OnRenderWindow();

            GameSimulationApp.PresentationEnabled = true;
            var graphical = new MainMenu(GameSimulationApp.Instance);
            graphical.OnWindowPostCreate();
            var graphicalMenu = graphical.OnRenderWindow();

            foreach (var choice in Menu)
            {
                Assert.Contains(choice, headlessMenu);
                Assert.Contains(choice, graphicalMenu);
            }

            // Nothing is numbered past the end, so no host ever had a gap where a choice should be.
            Assert.DoesNotContain("7.", headlessMenu);
            Assert.DoesNotContain("7.", graphicalMenu);
        }

        [Fact]
        public void FlagOn_TheToggle_Mutes_Announces_AndUnmutesAgain()
        {
            GameSimulationApp.PresentationEnabled = true;
            Game.Restart();
            Game.PumpInput();

            var menu = Assert.IsType<MainMenu>(Game.WindowManager.FocusedWindow);
            Assert.False(Music.Muted);

            SendCommand("4");
            Assert.True(Music.Muted);
            Assert.Contains("(sound is off)", menu.OnRenderWindow());

            SendCommand("4");
            Assert.False(Music.Muted);
            Assert.DoesNotContain("(sound is off)", menu.OnRenderWindow());
        }

        [Fact]
        public void FlagOff_TheToggle_StillWorks_AndTheTextMenuAnnouncesIt()
        {
            // A headless host has no audio device, but the choice still does what it says and says what it did.
            Game.Restart();
            Game.PumpInput();

            var menu = Assert.IsType<MainMenu>(Game.WindowManager.FocusedWindow);
            Assert.False(Music.Muted);

            SendCommand("4");
            Assert.True(Music.Muted);
            Assert.Contains("(sound is off)", menu.OnRenderWindow());

            SendCommand("4");
            Assert.False(Music.Muted);
            Assert.DoesNotContain("(sound is off)", menu.OnRenderWindow());
        }
    }
}
