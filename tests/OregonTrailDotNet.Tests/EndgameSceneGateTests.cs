using OregonTrailDotNet.Presentation.Audio;
using OregonTrailDotNet.Window.GameOver;
using OregonTrailDotNet.Window.Graveyard;
using Xunit;

namespace OregonTrailDotNet.Tests
{
    /// <summary>
    ///     Pins the phase-4 gate sites: with PresentationEnabled off, the Graveyard and GameOver windows resolve to
    ///     the plain text forms exactly as before; with it on they choose the scenes. Same flag/audio hygiene as the
    ///     landmark-card fixture: reset in Dispose, mute before any scene can cue, shut the player down afterwards.
    /// </summary>
    public class EndgameSceneGateTests : SimulationTestBase
    {
        private readonly bool _wasMuted;

        public EndgameSceneGateTests()
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
        public void FlagOff_StoneAndQuestion_AreThePlainTextForms()
        {
            Assert.Equal(typeof(TombstoneView), TombstoneScene.ViewFormType);
            Assert.Equal(typeof(EpitaphQuestion), EpitaphQuestionScene.QuestionFormType);
        }

        [Fact]
        public void FlagOn_StoneAndQuestion_AreTheScenes()
        {
            GameSimulationApp.PresentationEnabled = true;
            Assert.Equal(typeof(TombstoneScene), TombstoneScene.ViewFormType);
            Assert.Equal(typeof(EpitaphQuestionScene), EpitaphQuestionScene.QuestionFormType);
        }

        [Fact]
        public void VictoryScene_Composes_WithTheCongratulations()
        {
            GameSimulationApp.PresentationEnabled = true;

            var window = new GameOver(GameSimulationApp.Instance);
            var scene = new VictoryScene(window);
            scene.OnFormPostCreate();

            var frame = scene.OnRenderForm();
            Assert.Contains("Congratulations", frame);
            Assert.Contains("Press ENTER to continue", frame);
        }

        [Fact]
        public void DeathScene_Composes_WithTheWreckAndSupplies()
        {
            GameSimulationApp.PresentationEnabled = true;

            var window = new GameOver(GameSimulationApp.Instance);
            var scene = new DeathScene(window);
            scene.OnFormPostCreate();

            var frame = scene.OnRenderForm();
            Assert.Contains("Your party has perished", frame);
            Assert.Contains("Remaining supplies", frame);
        }
    }
}
