using System;
using System.Collections.Generic;
using System.Reflection;
using OregonTrailDotNet.Entity;
using OregonTrailDotNet.Entity.Person;
using OregonTrailDotNet.Module.Time;
using OregonTrailDotNet.Presentation;
using OregonTrailDotNet.Window.MainMenu;
using OregonTrailDotNet.Window.Travel;
using OregonTrailDotNet.Window.Travel.Scene;
using Xunit;

namespace OregonTrailDotNet.Tests
{
    /// <summary>
    ///     Covers the ability to leave a hunt early instead of standing in the field until the countdown runs out.
    ///     There is one hunt now - the real-time field - and ESC is its stop word: it hands whatever is already on the
    ///     ground to <see cref="HuntSceneResult" /> exactly as running out of time does, rather than throwing the meat
    ///     away for leaving before dark.
    ///     <para>
    ///         The other half of that contract matters just as much and is pinned here too: nothing else ends a hunt.
    ///         An empty ENTER in particular is what the headless training bot submits between animals, and in this
    ///         hunt it is the original's own walk toggle - if it ever ended the day instead, every training run would
    ///         quietly stop hunting.
    ///     </para>
    /// </summary>
    public class HuntEarlyExitTests : SimulationTestBase
    {
        public HuntEarlyExitTests()
        {
            // The hunt runs the identical simulation either way; the flag only decides whether each step is resampled
            // into a picture. Off here so nothing composes ANSI for a reader who does not exist.
            SceneHost.Graphical = false;
        }

        public override void Dispose()
        {
            // Process-wide static in a serial suite: put it back however this test left it.
            SceneHost.Graphical = false;
            base.Dispose();
        }

        /// <summary>
        ///     Puts a real party in the wagon and a live hunt on a Travel window, the way the travel menu does - the
        ///     scene builds its own field on post-create and publishes it as the window's active hunt.
        /// </summary>
        private static (Travel Window, TravelInfo Data, HuntScene Scene) StartHunt()
        {
            Game.SetStartInfo(new NewGameInfo
            {
                PlayerNames = new List<string> {"Alice", "Bob"},
                PlayerProfession = ProfessionEnum.Farmer,
                StartingMonies = 400,
                StartingMonth = MonthEnum.April
            });

            // The rifle is loaded straight out of the wagon, so a hunt started on an empty ammunition stack could
            // never pull the trigger at all - which is a different screen's problem, not this one's.
            Game.Vehicle.Inventory[EntitiesEnum.Ammo].AddQuantity(100);

            var window = new Travel(GameSimulationApp.Instance);

            // The window's UserData (shared with its forms) is protected; reach it to read back what the hunt hands
            // over on its way out.
            var data = (TravelInfo) window.GetType().BaseType!
                .GetProperty("UserData", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)!
                .GetValue(window)!;

            var scene = new HuntScene(window);
            scene.OnFormPostCreate();
            return (window, data, scene);
        }

        [Fact]
        public void PressingEscape_EndsTheHunt_AndKeepsTheBag()
        {
            var (window, data, scene) = StartHunt();
            var hunt = window.ActiveHunt;
            Assert.NotNull(hunt);

            // A hunt with meat already on the ground. Pounds only ever moves when a bullet lands on an animal, and
            // the scene seeds its field from the simulation's randomizer - which is not seedable - so there is no
            // bounded, honest way to put a carcass under the muzzle from out here. The bag is therefore planted on
            // the live game through Pounds' backing field; what is under test is that leaving early keeps it.
            typeof(HuntGame)
                .GetField("<Pounds>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic)!
                .SetValue(hunt, 350);

            // One genuine trigger pull, so the round count that reaches the result screen is the game's own tally
            // rather than another planted number.
            scene.OnKeyPressed(new ConsoleKeyInfo(' ', ConsoleKey.Spacebar, false, false, false));
            Assert.Equal(1, hunt.ShotsFired);

            scene.OnKeyPressed(new ConsoleKeyInfo((char) 27, ConsoleKey.Escape, false, false, false));

            // Straight to the reckoning, with the whole hunt handed across intact - the meat, the rounds spent and
            // the kill count all travel, so the wagon gets exactly what a hunt that ran to the last tick would give.
            Assert.IsType<HuntSceneResult>(window.CurrentForm);
            Assert.NotNull(data.HuntOutcome);
            Assert.Equal(350, data.HuntOutcome.RawPounds);
            Assert.Equal(1, data.HuntOutcome.ShotsFired);
        }

        [Fact]
        public void AnyOtherKey_KeepsHunting()
        {
            // Only ESC leaves. A key the hunt understands (O swings the rifle north) and one it does not are both
            // just another tick of the same hunt.
            var (window, data, scene) = StartHunt();

            scene.OnKeyPressed(new ConsoleKeyInfo('o', ConsoleKey.O, false, false, false));
            scene.OnKeyPressed(new ConsoleKeyInfo('z', ConsoleKey.Z, false, false, false));

            Assert.Null(window.CurrentForm);
            Assert.Null(data.HuntOutcome);
        }

        [Fact]
        public void EmptyInput_DoesNotEndTheHunt_ItWalksTheHunter()
        {
            // An empty ENTER is what the headless bot submits between animals; ending the hunt on it would break
            // every training run. Here it is the original's Return binding and nothing more.
            var (window, data, scene) = StartHunt();
            var hunt = window.ActiveHunt;

            scene.OnInputBufferReturned(string.Empty);

            Assert.True(hunt.Walking);
            Assert.Null(window.CurrentForm);
            Assert.Null(data.HuntOutcome);
        }
    }
}
