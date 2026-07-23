using OregonTrailDotNet.Presentation.Audio;
using OregonTrailDotNet.Window.Travel;
using OregonTrailDotNet.Window.Travel.Hunt.Help;
using OregonTrailDotNet.Window.Travel.Scene;
using Xunit;

namespace OregonTrailDotNet.Tests
{
    /// <summary>
    ///     Pins the phase-8 hunt gate and the wrapper the real-time hunt applies at its end. With the flag off the
    ///     hunt entry resolves to the word-typing game's instruction dialog exactly as before; with it on, the
    ///     real-time field's. The wrapper is pinned as a pure function in the original's order: dress by halving,
    ///     zero against a full wagon, clamp to remaining space, cap at the 100 lb carry.
    /// </summary>
    public class HuntSceneGateTests : SimulationTestBase
    {
        public override void Dispose()
        {
            GameSimulationApp.PresentationEnabled = false;
            Music.Shutdown();
            base.Dispose();
        }

        [Fact]
        public void FlagOff_HuntEntry_IsTheWordGamePrompt()
        {
            Assert.Equal(typeof(HuntingPrompt), HuntSceneHelp.FormType);
        }

        [Fact]
        public void FlagOn_HuntEntry_IsTheSceneHelp()
        {
            GameSimulationApp.PresentationEnabled = true;
            Assert.Equal(typeof(HuntSceneHelp), HuntSceneHelp.FormType);
        }

        [Fact]
        public void Wrapper_DressesByHalving_AndPassesSmallKillsWhole()
        {
            // 350 raw dresses to 175, room for all of it, but the carry cap holds it to 100.
            Assert.Equal(100, HuntSceneResult.DressAndLoad(350, 0, 2000));

            // A 2 lb rabbit passes through whole — halving starts at three pounds.
            Assert.Equal(2, HuntSceneResult.DressAndLoad(2, 0, 2000));

            // 60 raw dresses to 30 and fits under every cap.
            Assert.Equal(30, HuntSceneResult.DressAndLoad(60, 0, 2000));
        }

        [Fact]
        public void Wrapper_ZeroesAgainstAFullWagon_AndClampsToSpace()
        {
            // The wagon is full: everything shot is left on the field.
            Assert.Equal(0, HuntSceneResult.DressAndLoad(350, 2000, 2000));

            // Only 40 pounds of space: the space clamp lands before the carry cap can.
            Assert.Equal(40, HuntSceneResult.DressAndLoad(350, 1960, 2000));

            // Nothing shot means nothing to load.
            Assert.Equal(0, HuntSceneResult.DressAndLoad(0, 0, 2000));
        }

        [Fact]
        public void HuntScene_Composes_WithTheHud()
        {
            GameSimulationApp.PresentationEnabled = true;

            var window = new Travel(GameSimulationApp.Instance);
            var scene = new HuntScene(window);
            scene.OnFormPostCreate();

            // The HUD is pixel-font text inside the picture now, so the frame is pure ANSI imagery.
            var frame = scene.OnRenderForm();
            Assert.False(string.IsNullOrWhiteSpace(frame));
        }
    }
}
