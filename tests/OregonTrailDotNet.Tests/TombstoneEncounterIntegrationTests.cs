using System.Collections.Generic;
using System.Text;
using OregonTrailDotNet.Entity.Item;
using OregonTrailDotNet.Entity.Location;
using OregonTrailDotNet.Entity.Person;
using OregonTrailDotNet.Module.Time;
using OregonTrailDotNet.Module.Tombstone;
using OregonTrailDotNet.Window.MainMenu;
using OregonTrailDotNet.Window.Travel.Command;
using OregonTrailDotNet.Window.Travel.Dialog;
using Xunit;
using TravelWindow = OregonTrailDotNet.Window.Travel.Travel;

namespace OregonTrailDotNet.Tests
{
    /// <summary>
    ///     End-to-end proof that a party actually driving down the trail is offered the tombstone of an earlier party when it
    ///     crosses that grave. Unlike the focused unit tests (which tick a hand-built <see cref="ContinueOnTrail" /> in
    ///     isolation) this boots a real, survivable party, seeds a grave, and drives the live simulation through the actual
    ///     Travel window until the <see cref="TombstoneQuestion" /> appears. The grave-crossing check only runs inside
    ///     <see cref="ContinueOnTrail" />.OnTick while the Travel window is focused, so exercising the real drive loop is the
    ///     only way to catch a regression in the whole path. These tests failed against the pre-fix code (the party drove far
    ///     past the grave and was never offered it), reproducing the reported bug.
    ///     Mileage per turn is random and the Randomizer is not seedable, so both tests assert "the grave is offered within a
    ///     generous tick budget", never on an exact tick.
    /// </summary>
    public sealed class TombstoneEncounterIntegrationTests : SimulationTestBase
    {
        private const int TickBudget = 800;

        /// <summary>
        ///     Sets up a well-provisioned party (so it survives long enough to reach the grave), advances it to the first
        ///     landmark, and drops the main menu so the Travel window is focused and ready to drive. Returns the distance from
        ///     the first landmark to the next, which the tests use to place a grave at a known point.
        /// </summary>
        private static int SeedDepartedParty()
        {
            Game.SetStartInfo(new NewGameInfo
            {
                PlayerNames = new List<string> {"Alice", "Bob", "Carol", "Dave", "Eve"},
                PlayerProfession = Profession.Farmer,
                StartingMonies = 1600,
                StartingMonth = Month.April
            });

            var vehicle = Game.Vehicle;
            vehicle.Purchase(new SimItem(Parts.Oxen, 8));
            vehicle.Purchase(new SimItem(Resources.Food, 2000));
            vehicle.Purchase(new SimItem(Resources.Clothing, 10));
            vehicle.Purchase(new SimItem(Parts.Wheel, 2));
            vehicle.Purchase(new SimItem(Parts.Axle, 2));
            vehicle.Purchase(new SimItem(Parts.Tongue, 2));

            Game.Trail.ArriveAtNextLocation();
            Assert.Equal("Independence", Game.Trail.CurrentLocation.Name);
            var distanceToKansas = Game.Trail.DistanceToNextLocation;
            Assert.True(distanceToKansas > 0);

            // Boot leaves the main menu focused above Travel; drop it so the real drive loop runs.
            Game.WindowManager.FocusedWindow.RemoveWindowNextTick();
            Game.OnTick(false);
            Game.OnTick(false);
            Assert.IsType<TravelWindow>(Game.WindowManager.FocusedWindow);

            return distanceToKansas;
        }

        /// <summary>
        ///     Drives the live simulation until the party is offered a tombstone or the tick budget runs out. Keeps the party
        ///     moving across landmark arrivals (re-departing when the drive form is replaced) and dismisses any random-event
        ///     windows with ENTER so a stray event cannot stall the drive. A short movement trace is captured for diagnostics.
        /// </summary>
        private static bool DriveUntilTombstoneOffered(StringBuilder trace)
        {
            for (var tick = 0; tick < TickBudget; tick++)
            {
                var focused = Game.WindowManager.FocusedWindow;
                var form = focused?.CurrentForm;
                var windowName = focused?.GetType().Name ?? "<none>";
                var formName = form?.GetType().Name ?? "<null>";

                if (trace.Length < 8000)
                    trace.AppendLine(
                        $"t{tick,3} win={windowName,-12} form={formName,-18} odo={Game.Vehicle.Odometer,4} " +
                        $"cursor={Game.Vehicle.LastGraveCheckOdometer,4} dist={Game.Trail.DistanceToNextLocation,4} " +
                        $"loc={Game.Trail.CurrentLocation.Name}");

                if (form is TombstoneQuestion)
                    return true;

                if (focused is TravelWindow travel)
                {
                    if (form is ContinueOnTrail)
                    {
                        Game.OnTick(false);
                    }
                    else
                    {
                        // Arrived (or some other travel form is up) — re-depart so the drive continues onto the next leg.
                        Game.Trail.CurrentLocation.Status = LocationStatus.Departed;
                        travel.ContinueOnTrail();
                        Game.OnTick(false);
                    }
                }
                else
                {
                    // A non-travel window (e.g. a random event) is up; press ENTER to clear it and get back to driving.
                    Game.InputManager.SendInputBufferAsCommand();
                    Game.OnTick(false);
                    Game.OnTick(false);
                }
            }

            return false;
        }

        [Fact]
        public void GraveCrossedMidLeg_OffersTombstoneQuestion()
        {
            var distanceToKansas = SeedDepartedParty();

            // A grave a third of the way to the next landmark: squarely mid-leg, reached within a couple of turns.
            var graveMarker = distanceToKansas / 3;
            Assert.True(graveMarker > 0);
            Game.Tombstone.Add(new Tombstone(
                Tombstone.CalculateTrailHalf(graveMarker, Game.Trail.Length),
                graveMarker, "Ezekiel", "died of not reading the manual", "Independence", "Kansas River Crossing",
                distanceToKansas - graveMarker));

            var trace = new StringBuilder();
            var offered = DriveUntilTombstoneOffered(trace);

            Assert.True(offered,
                $"Party never got the tombstone question crossing a mid-leg grave at mile {graveMarker}.\n{trace}");
        }

        [Fact]
        public void GraveOnLandmarkBoundary_OffersTombstoneQuestionOnNextLeg()
        {
            var distanceToKansas = SeedDepartedParty();

            // A grave exactly on the landmark boundary — where deaths are recorded (a party noticed dead on arrival). This is
            // the stretch the old form-local cursor dropped; the vehicle-anchored cursor must pick it up as the party crosses.
            var graveMarker = distanceToKansas;
            Game.Tombstone.Add(new Tombstone(
                Tombstone.CalculateTrailHalf(graveMarker, Game.Trail.Length),
                graveMarker, "Josiah", "should have forded the river", "Independence", "Kansas River Crossing", 0));

            var trace = new StringBuilder();
            var offered = DriveUntilTombstoneOffered(trace);

            Assert.True(offered,
                $"Party never got the tombstone question for a grave on the landmark boundary at mile {graveMarker}.\n{trace}");

            // The grave must be offered only once the party has actually reached the boundary odometer, not before.
            Assert.True(Game.Vehicle.Odometer >= graveMarker,
                $"Grave was offered before the party reached the boundary odometer. Trace:\n{trace}");
        }
    }
}
