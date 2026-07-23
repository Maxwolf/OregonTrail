using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using OregonTrailDotNet.Entity;
using OregonTrailDotNet.Entity.Location.Point;
using OregonTrailDotNet.Entity.Person;
using OregonTrailDotNet.Module.Time;
using OregonTrailDotNet.Presentation.Audio;
using OregonTrailDotNet.Window.GameOver;
using OregonTrailDotNet.Window.MainMenu;
using OregonTrailDotNet.Window.Travel;
using OregonTrailDotNet.Window.Travel.Dialog;
using OregonTrailDotNet.Window.Travel.RiverCrossing.Help;
using OregonTrailDotNet.Window.Travel.Scene;
using Xunit;

namespace OregonTrailDotNet.Tests
{
    /// <summary>
    ///     Pins the one scene chain that had no flow coverage — the game's ending. The Columbia routes to the raft
    ///     only with presentation on (headless hosts keep the crossing menu), the intro hands off to the run, and
    ///     the result form's two exits both work: survivors depart down the last leg, and a dead party raises the
    ///     normal game-over flow. Same flag/audio hygiene as the other gate fixtures.
    /// </summary>
    public class RaftFlowGateTests : SimulationTestBase
    {
        private readonly bool _wasMuted;

        public RaftFlowGateTests()
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

        /// <summary>
        ///     Walks the trail to The Dalles and splices in the Columbia branch, exactly as choosing the fork does,
        ///     then arrives on the raft crossing.
        /// </summary>
        private void ArriveAtTheColumbia()
        {
            Game.TakeTurn(false);

            var guard = 0;
            while (Game.Trail.CurrentLocation.Name != "The Dalles" && guard++ < 40)
                Game.Trail.ArriveAtNextLocation();
            var dalles = Assert.IsType<ForkInRoad>(Game.Trail.CurrentLocation);

            var columbia = dalles.SkipChoices.OfType<RiverCrossing>().Single(river => river.RaftCrossing);
            Game.Trail.InsertLocation(columbia);
            Game.Trail.ArriveAtNextLocation();

            Assert.Equal("Columbia River", Game.Trail.CurrentLocation.Name);
        }

        /// <summary>Builds a live Travel window and hands back its shared user data, the gate-test way.</summary>
        private static (Travel Window, TravelInfo Data) BuildTravelWindow()
        {
            var window = new Travel(GameSimulationApp.Instance);
            var data = (TravelInfo) window.GetType().BaseType!
                .GetProperty("UserData", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)!
                .GetValue(window)!;
            return (window, data);
        }

        /// <summary>Outfits a real party so the raft has stakes to charge.</summary>
        private void BootParty()
        {
            Game.SetStartInfo(new NewGameInfo
            {
                PlayerNames = new List<string> { "Alice", "Bob", "Carol", "Dave" },
                PlayerProfession = ProfessionEnum.Banker,
                StartingMonies = 1600,
                StartingMonth = MonthEnum.April
            });
        }

        [Fact]
        public void FlagOff_TheColumbia_KeepsTheCrossingMenu()
        {
            BootParty();
            ArriveAtTheColumbia();

            var (window, _) = BuildTravelWindow();
            window.ContinueOnTrail();

            Assert.IsType<RiverCrossHelp>(window.CurrentForm);
        }

        [Fact]
        public void FlagOn_TheColumbia_IsTheRaft_AndTheIntroHandsOffToTheRun()
        {
            BootParty();
            ArriveAtTheColumbia();
            GameSimulationApp.PresentationEnabled = true;

            var (window, _) = BuildTravelWindow();
            window.ContinueOnTrail();
            Assert.IsType<RaftIntro>(window.CurrentForm);

            // ENTER shoves off: the instruction card hands the party to the river.
            window.CurrentForm.OnInputBufferReturned(string.Empty);
            var scene = Assert.IsType<RaftScene>(window.CurrentForm);

            // The run's frame is the pure river picture — no footer, no prompt; the intro's "Press ENTER" hint
            // must not survive into it.
            var frame = scene.OnRenderForm();
            Assert.False(string.IsNullOrWhiteSpace(frame));
            Assert.DoesNotContain("ENTER", frame);
        }

        [Fact]
        public void FlagOn_EveryOtherRiver_StaysTheCrossingMenu()
        {
            BootParty();
            Game.TakeTurn(false);
            Game.Trail.ArriveAtNextLocation();
            Assert.Equal("Kansas River Crossing", Game.Trail.CurrentLocation.Name);
            GameSimulationApp.PresentationEnabled = true;

            var (window, _) = BuildTravelWindow();
            window.ContinueOnTrail();

            Assert.IsType<RiverCrossHelp>(window.CurrentForm);
        }

        [Fact]
        public void Survivors_ComeAshore_OntoTheLastLeg()
        {
            BootParty();
            ArriveAtTheColumbia();

            var (window, data) = BuildTravelWindow();
            data.RaftReport = new RaftReport(new List<string>(), false, false);

            var result = new RaftResult(window);
            result.OnFormPostCreate();
            result.OnInputBufferReturned(string.Empty);

            // Ashore and on down the trail: the depart chooser (always the plain text form) takes over.
            Assert.IsType<LocationDepart>(window.CurrentForm);
            Assert.Null(data.RaftReport);
        }

        [Fact]
        public void ADeadParty_RaisesTheGameOverFlow()
        {
            BootParty();
            ArriveAtTheColumbia();

            // The discriminating case: the raft came through INTACT but the party drowned across ordinary hits
            // (each rock rolls each swimmer at 0.60 — no destruction needed). The result form must key on the
            // dead party, not the destroyed-raft flag; a destroyed raft always implies a dead party, so testing
            // that corner would prove nothing about which signal the code reads.
            foreach (var passenger in Game.Vehicle.Passengers)
                passenger.Kill(CauseOfDeathEnum.Drowned);
            Assert.True(Game.Vehicle.PassengersDead);

            var (window, data) = BuildTravelWindow();
            data.RaftReport = new RaftReport(new List<string> { "everything" }, false, false);

            var result = new RaftResult(window);
            result.OnFormPostCreate();
            result.OnInputBufferReturned(string.Empty);

            // The form clears itself and re-adds Travel; the arrival check sees the dead party and raises the
            // game-over window — the same path any fatal day on the trail takes.
            Assert.Null(window.CurrentForm);
            Game.PumpInput();
            Assert.IsType<GameOver>(Game.WindowManager.FocusedWindow);
        }
    }
}
