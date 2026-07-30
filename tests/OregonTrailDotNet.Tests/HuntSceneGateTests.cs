using OregonTrailDotNet.Entity;
using OregonTrailDotNet.Presentation;
using OregonTrailDotNet.Presentation.Audio;
using OregonTrailDotNet.Window.Travel;
using OregonTrailDotNet.Window.Travel.Scene;
using Xunit;

namespace OregonTrailDotNet.Tests
{
    /// <summary>
    ///     Pins the hunt as ONE game, and the wrapper it applies at the end.
    ///     <para>
    ///         There used to be a gate here: with presentation off the hunt entry resolved to a word-typing game with
    ///         its own species, its own hit rule and ammunition charged per kill, and with presentation on to the
    ///         real-time field. That meant the training bot spent every hunt of its life optimizing an economy no
    ///         player ever saw. The fork is gone and these tests exist to keep it gone - the hunt entry is the same
    ///         type whichever way the flag is set, because which game gets played must never depend on who is
    ///         watching.
    ///     </para>
    ///     <para>
    ///         The wrapper is pinned as a pure function in the original's order: dress by halving, zero against a
    ///         full wagon, clamp to remaining space, cap at the 100 lb carry.
    ///     </para>
    /// </summary>
    public class HuntSceneGateTests : SimulationTestBase
    {
        public override void Dispose()
        {
            GameSimulationApp.PresentationEnabled = false;
            SceneHost.Graphical = false;
            Music.Shutdown();
            base.Dispose();
        }

        [Fact]
        public void HuntEntry_IsTheSameForm_WhicheverWayTheFlagIsSet()
        {
            GameSimulationApp.PresentationEnabled = false;
            var headless = HuntEntryType();

            GameSimulationApp.PresentationEnabled = true;
            var graphical = HuntEntryType();

            Assert.Equal(typeof(HuntSceneHelp), headless);
            Assert.Equal(headless, graphical);
        }

        [Fact]
        public void TheWordTypingHunt_IsGone()
        {
            // Named types rather than a namespace sweep, so this fails loudly if anyone reintroduces the fork by
            // restoring the old files rather than by adding a gate.
            var assembly = typeof(GameSimulationApp).Assembly;
            Assert.Null(assembly.GetType("OregonTrailDotNet.Window.Travel.Hunt.HuntManager"));
            Assert.Null(assembly.GetType("OregonTrailDotNet.Window.Travel.Hunt.Hunting"));
            Assert.Null(assembly.GetType("OregonTrailDotNet.Window.Travel.Hunt.PreyItem"));
            Assert.Null(assembly.GetType("OregonTrailDotNet.Window.Travel.Hunt.Help.HuntingPrompt"));
        }

        [Fact]
        public void AmmunitionIsChargedPerTriggerPull_NotPerKill()
        {
            // The single largest divergence the old pair had: the word hunt charged 10-13 rounds per landed kill and
            // nothing at all for a miss. One round leaves the wagon per shot, hit or miss, for everybody.
            var game = new HuntGame(seed: 1, bullets: 20);
            game.Fire();

            Assert.Equal(19, game.Bullets);
            Assert.Equal(1, game.ShotsFired);
        }

        [Fact]
        public void Wrapper_DressesByHalving_AndPassesSmallKillsWhole()
        {
            // 350 raw dresses to 175, room for all of it, but the carry cap holds it to 100.
            Assert.Equal(100, HuntSceneResult.DressAndLoad(350, 0, 2000));

            // A 2 lb rabbit passes through whole - halving starts at three pounds.
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
        public void HuntScene_Composes_ThePureFieldPicture()
        {
            GameSimulationApp.PresentationEnabled = true;
            SceneHost.Graphical = true;

            var window = new Travel(GameSimulationApp.Instance);
            var scene = new HuntScene(window);
            scene.OnFormPostCreate();

            // The game's hunt carries no HUD text at all - the frame is the pure ANSI field picture (the tally
            // waits for the result screen) - so all this can pin is that a frame composes.
            var frame = scene.OnRenderForm();
            Assert.False(string.IsNullOrWhiteSpace(frame));
        }

        [Fact]
        public void HuntScene_Headless_RunsTheSameHunt_WithoutDrawingIt()
        {
            SceneHost.Graphical = false;

            var window = new Travel(GameSimulationApp.Instance);
            var scene = new HuntScene(window);
            scene.OnFormPostCreate();

            // The simulation is live and readable - the same object the picture would be drawn from - while the
            // frame is only a caption. That is the whole contract: same game, no pixels.
            Assert.NotNull(window.ActiveHunt);
            Assert.Equal("[HuntScene]", scene.OnRenderForm());

            // And it steps. One simulation tick is one tick of the hunt, no wall clock involved.
            var before = window.ActiveHunt.Tick;
            scene.OnTick(false, false);
            Assert.Equal(before + 1, window.ActiveHunt.Tick);
        }

        /// <summary>Runs the travel menu's own hunt dispatch and reports which form it landed on.</summary>
        private static System.Type HuntEntryType()
        {
            var window = new Travel(GameSimulationApp.Instance);
            window.OnWindowPostCreate();

            // The dispatch refuses a hunt with no ammunition, which is not what is under test here.
            GameSimulationApp.Instance.Vehicle.Inventory[EntitiesEnum.Ammo].AddQuantity(100);

            var method = typeof(Travel).GetMethod("HuntForFood",
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            Assert.NotNull(method);
            method.Invoke(window, null);

            return window.CurrentForm.GetType();
        }
    }
}
