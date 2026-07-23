using System.Reflection;
using OregonTrailDotNet.Presentation.Audio;
using OregonTrailDotNet.Window.Travel;
using OregonTrailDotNet.Window.Travel.RiverCrossing;
using OregonTrailDotNet.Window.Travel.Scene;
using Xunit;

namespace OregonTrailDotNet.Tests
{
    /// <summary>
    ///     Compose smoke for the crossing scene — the one animated scene without a frame test (the drive, hunt and
    ///     raft scenes have theirs beside their gate tests). This guards the Build path and the first frame of the
    ///     ferry and ford pictures; the disaster branches (wreck swap, swamp wedge, far-bank fan) render only from
    ///     live midstream state and stay covered by play.
    /// </summary>
    public class SceneComposeSmokeTests : SimulationTestBase
    {
        private readonly bool _wasMuted;

        public SceneComposeSmokeTests()
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

        [Theory]
        [InlineData(RiverCrossChoiceEnum.Ferry)]
        [InlineData(RiverCrossChoiceEnum.Ford)]
        public void CrossingScene_Composes_TheRiverPicture(RiverCrossChoiceEnum choice)
        {
            // The first river on the trail, with a live crossing agreed, the way CrossingSimulationTests set up.
            Game.TakeTurn(false);
            Game.Trail.ArriveAtNextLocation();
            Assert.Equal("Kansas River Crossing", Game.Trail.CurrentLocation.Name);

            var window = new Travel(GameSimulationApp.Instance);
            var data = (TravelInfo) window.GetType().BaseType!
                .GetProperty("UserData", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)!
                .GetValue(window)!;
            data.GenerateRiver();
            data.River.CrossingType = choice;
            data.River.FerryCost = 0;

            GameSimulationApp.PresentationEnabled = true;
            var scene = new CrossingScene(window);
            scene.OnFormPostCreate();

            Assert.False(string.IsNullOrWhiteSpace(scene.OnRenderForm()));
        }
    }
}
