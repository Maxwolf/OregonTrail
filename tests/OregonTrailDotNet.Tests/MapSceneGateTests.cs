using OregonTrailDotNet.Presentation;
using OregonTrailDotNet.Presentation.Audio;
using OregonTrailDotNet.Window.Travel;
using OregonTrailDotNet.Window.Travel.Command;
using OregonTrailDotNet.Window.Travel.Scene;
using WolfCurses.Graphics;
using Xunit;

namespace OregonTrailDotNet.Tests
{
    /// <summary>
    ///     Pins the phase-5 gate: both map dispatch sites route through <see cref="MapScene.FormType" />, which
    ///     resolves to the plain text progress table for every headless host and to the plotted DOS map when
    ///     presentation is on. Same flag hygiene as the other gate fixtures; the map is silent, but the audio
    ///     teardown stays for symmetry.
    /// </summary>
    public class MapSceneGateTests : SimulationTestBase
    {
        public override void Dispose()
        {
            GameSimulationApp.PresentationEnabled = false;
            Music.Shutdown();
            base.Dispose();
        }

        [Fact]
        public void FlagOff_MapIsThePlainTextForm()
        {
            Assert.Equal(typeof(LookAtMap), MapScene.FormType);
        }

        [Fact]
        public void FlagOn_MapIsTheScene()
        {
            GameSimulationApp.PresentationEnabled = true;
            Assert.Equal(typeof(MapScene), MapScene.FormType);
        }

        [Fact]
        public void MapScene_Composes_AtTheStartOfTheTrail()
        {
            GameSimulationApp.PresentationEnabled = true;

            // Boot state: nothing visited yet (Independence is Unreached until the store is left) — the scene
            // must still render the map picture and the dismiss hint without a route to plot.
            var window = new Travel(GameSimulationApp.Instance);
            var scene = new MapScene(window);
            scene.OnFormPostCreate();

            var frame = scene.OnRenderForm();
            Assert.False(string.IsNullOrWhiteSpace(frame));
            Assert.Contains("Press ENTER to continue", frame);
        }

        [Fact]
        public void PlotRoute_FromBareCoordinates_DrawsOntoTheFrame()
        {
            // A white frame: the route plots in black, so any route pixel is a measurable change.
            var frame = new PixelBuffer(100, 100);
            for (var y = 0; y < frame.Height; y++)
                for (var x = 0; x < frame.Width; x++)
                    frame.SetPixel(x, y, Palette.White);

            MapArt.PlotRoute(frame, new[] { (10, 10), (60, 40) }, (60, 40), (90, 90), 0.5);

            var black = 0;
            for (var y = 0; y < frame.Height; y++)
                for (var x = 0; x < frame.Width; x++)
                    if (frame.GetPixel(x, y).R == 0)
                        black++;

            // The polyline plus half the in-progress leg: well over the 2x2 blocks of a single point.
            Assert.True(black > 50, $"Expected a plotted route, found {black} black pixels.");
        }
    }
}
