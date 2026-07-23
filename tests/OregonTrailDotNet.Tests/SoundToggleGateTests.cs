using OregonTrailDotNet.Presentation.Audio;
using OregonTrailDotNet.Window.MainMenu;
using Xunit;

namespace OregonTrailDotNet.Tests
{
    /// <summary>
    ///     Pins the main menu's sound toggle — the original's "Turn sound off": offered only with presentation on
    ///     (headless hosts have no sound and the bot's menu text must not change), printed as its frozen enum
    ///     value 6, flipping the one process-wide mute, and reading the state back through the masthead since a
    ///     command's label cannot change. Nothing here plays a cue, so no audio device is ever opened.
    /// </summary>
    public class SoundToggleGateTests : SimulationTestBase
    {
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
        public void FlagOff_TheMenu_OffersNoSoundToggle()
        {
            var window = new MainMenu(GameSimulationApp.Instance);
            window.OnWindowPostCreate();

            Assert.DoesNotContain("Turn sound", window.OnRenderWindow());
        }

        [Fact]
        public void FlagOn_TheToggle_Mutes_Announces_AndUnmutesAgain()
        {
            GameSimulationApp.PresentationEnabled = true;
            Game.Restart();
            Game.PumpInput();

            var menu = Assert.IsType<MainMenu>(Game.WindowManager.FocusedWindow);
            Assert.Contains("6. Turn sound on/off", menu.OnRenderWindow());
            Assert.False(Music.Muted);

            SendCommand("6");
            Assert.True(Music.Muted);
            Assert.Contains("(sound is off)", menu.OnRenderWindow());

            SendCommand("6");
            Assert.False(Music.Muted);
            Assert.DoesNotContain("(sound is off)", menu.OnRenderWindow());
        }
    }
}
