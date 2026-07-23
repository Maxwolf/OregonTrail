using OregonTrailDotNet.Presentation.Audio;
using OregonTrailDotNet.Window.Travel;
using OregonTrailDotNet.Window.Travel.Dialog;
using OregonTrailDotNet.Window.Travel.Scene;
using Xunit;

namespace OregonTrailDotNet.Tests
{
    /// <summary>
    ///     Pins the phase-3 gate sites: with PresentationEnabled off (every headless host) the depart chooser and the
    ///     look-around branch resolve to the plain text forms exactly as before; with it on they choose the card
    ///     scenes. Flag hygiene per the plan: the flag is a process-wide static in a serial suite, so it is reset in
    ///     Dispose (SimulationTestBase also force-clears it before every test). Audio hygiene: attaching a cued scene
    ///     would open a real waveOut device, so the fixture mutes first and shuts the player down afterwards.
    /// </summary>
    public class LandmarkCardGateTests : SimulationTestBase
    {
        private readonly bool _wasMuted;

        public LandmarkCardGateTests()
        {
            _wasMuted = Music.Muted;
            if (!Music.Muted)
                Music.ToggleMute();
        }

        public override void Dispose()
        {
            GameSimulationApp.PresentationEnabled = false;
            Music.Shutdown();
            if (!_wasMuted && Music.Muted)
                Music.ToggleMute();
            base.Dispose();
        }

        [Fact]
        public void FlagOff_DepartChooser_PicksThePlainTextForm()
        {
            Assert.False(GameSimulationApp.PresentationEnabled);
            Assert.Equal(typeof(LocationDepart), TravelInfo.DepartFormType);
        }

        [Fact]
        public void FlagOff_LookAround_ShowsNoCard()
        {
            Assert.False(LandmarkCard.ShouldShow);
        }

        [Fact]
        public void FlagOn_DepartChooser_PicksTheCardScene()
        {
            GameSimulationApp.PresentationEnabled = true;

            // The boot state sits at Independence, which has card p0.
            Assert.Equal(typeof(LandmarkDepartCard), TravelInfo.DepartFormType);
        }

        [Fact]
        public void FlagOn_LookAround_ShowsTheCard()
        {
            GameSimulationApp.PresentationEnabled = true;
            Assert.True(LandmarkCard.ShouldShow);
        }

        [Fact]
        public void DriveChooser_IsTheTextDriveForm_UntilTheTravelScenePhase()
        {
            Assert.Equal(typeof(OregonTrailDotNet.Window.Travel.Command.ContinueOnTrail), TravelInfo.DriveFormType);
            GameSimulationApp.PresentationEnabled = true;
            Assert.Equal(typeof(OregonTrailDotNet.Window.Travel.Command.ContinueOnTrail), TravelInfo.DriveFormType);
        }

        [Fact]
        public void FlagOff_Opening_StaysTheTextPrompt()
        {
            Assert.False(OpeningCard.ShouldShow);
        }

        [Fact]
        public void FlagOn_Opening_ShowsTheIndependenceCard()
        {
            GameSimulationApp.PresentationEnabled = true;

            // Boot state sits at the trail's first location.
            Assert.True(OpeningCard.ShouldShow);
        }

        [Fact]
        public void OpeningCard_Composes_WithTheTimeTravelLine()
        {
            GameSimulationApp.PresentationEnabled = true;

            var window = new Travel(GameSimulationApp.Instance);
            var card = new OpeningCard(window);
            card.OnFormPostCreate();

            var frame = card.OnRenderForm();
            Assert.Contains("Going back to", frame);
            Assert.Contains("Press ENTER to continue", frame);
        }

        [Fact]
        public void ArrivalCard_Composes_WithCaptionAndDismissHint()
        {
            GameSimulationApp.PresentationEnabled = true;

            var window = new Travel(GameSimulationApp.Instance);
            var card = new LandmarkCard(window);
            card.OnFormPostCreate();

            var frame = card.OnRenderForm();
            Assert.False(string.IsNullOrWhiteSpace(frame));
            Assert.Contains("Press ENTER to continue", frame);
        }

        [Fact]
        public void DepartCard_Composes_WithTheDepartSentence()
        {
            GameSimulationApp.PresentationEnabled = true;

            var window = new Travel(GameSimulationApp.Instance);
            var card = new LandmarkDepartCard(window);
            card.OnFormPostCreate();

            var frame = card.OnRenderForm();
            Assert.Contains("From Independence it is", frame);
            Assert.Contains("miles to", frame);
        }
    }
}
