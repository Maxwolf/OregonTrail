using OregonTrailDotNet.Entity;
using OregonTrailDotNet.Entity.Location;
using OregonTrailDotNet.Entity.Vehicle;
using OregonTrailDotNet.Window.Travel;
using OregonTrailDotNet.Window.Travel.Command;
using OregonTrailDotNet.Window.Travel.Dialog;
using Xunit;
using TombstoneEntity = OregonTrailDotNet.Module.Tombstone.Tombstone;

namespace OregonTrailDotNet.Tests
{
    /// <summary>
    ///     Guards the "you pass a gravesite" moment: while the party is driving and the odometer sweeps past a grave an earlier
    ///     party left, <see cref="ContinueOnTrail" /> must stop and offer the <see cref="TombstoneQuestion" />.
    ///     Two things used to break this: (1) the <c>!CurrentLocation.ArrivalFlag</c> guard, which is always true mid-leg
    ///     because CurrentLocation is the just-departed location, silently suppressed every crossing; and (2) the scan cursor
    ///     lived on the drive form and was re-seeded to the current odometer each leg, so the final stretch before a landmark —
    ///     covered on the arriving turn, after which the drive form is torn down for the arrival screen — was never scanned.
    ///     Deaths are recorded at that landmark odometer, so that dropped stretch is exactly where saved graves sit.
    /// </summary>
    public sealed class TombstoneEncounterTests : SimulationTestBase
    {
        // Puts the vehicle into a genuine "driving between two landmarks" state: oxen so CheckStatus keeps it Moving, a
        // Moving status, and the just-departed location flagged Arrived/Departed (the trap the old guard fell into).
        private static void StartDriving()
        {
            var game = GameSimulationApp.Instance;
            game.Vehicle.Inventory[Entities.Animal].AddQuantity(4);
            game.Vehicle.Status = VehicleStatus.Moving;
            game.Trail.CurrentLocation.ArrivalFlag = true;
            game.Trail.CurrentLocation.Status = LocationStatus.Departed;
        }

        [Fact]
        public void ContinueOnTrail_OffersTombstone_WhenAGraveIsCrossedMidLeg()
        {
            var game = GameSimulationApp.Instance;
            StartDriving();

            // The party last scanned for graves at mile 100 and the odometer now reads 200, so the stretch just covered is
            // (100, 200]. Drop a grave at mile 150 squarely inside it.
            SetOdometer(game.Vehicle, 200);
            game.Vehicle.LastGraveCheckOdometer = 100;
            game.Tombstone.Add(new TombstoneEntity(0, 150, "Ghost", "welp", "Fort Kearney", "Chimney Rock", 10));

            var window = new Travel(game);
            var form = new ContinueOnTrail(window);
            form.OnFormPostCreate();
            form.OnTick(false, false);

            Assert.IsType<TombstoneQuestion>(window.CurrentForm);
        }

        [Fact]
        public void ContinueOnTrail_KeepsDriving_WhenNoGraveIsInTheStretch()
        {
            var game = GameSimulationApp.Instance;
            StartDriving();

            // Grave sits at mile 400, well beyond the (100, 200] stretch just travelled, so the drive continues uninterrupted.
            SetOdometer(game.Vehicle, 200);
            game.Vehicle.LastGraveCheckOdometer = 100;
            game.Tombstone.Add(new TombstoneEntity(0, 400, "Ghost", "welp", "Fort Hall", "Fort Boise", 10));

            var window = new Travel(game);
            var form = new ContinueOnTrail(window);
            form.OnFormPostCreate();
            form.OnTick(false, false);

            Assert.IsNotType<TombstoneQuestion>(window.CurrentForm);
        }

        [Fact]
        public void NewDriveForm_DoesNotResetTheScanCursor_SoTheFinalStretchIsNotSkipped()
        {
            var game = GameSimulationApp.Instance;
            StartDriving();

            // Reproduce a leg boundary: on the arriving turn the party rolled from mile 180 (last scan) to mile 220 (the
            // landmark) and a grave — a previous party that died on arrival — sits at 220. The drive form was torn down for
            // the arrival screen before it could scan (180, 220], leaving the cursor stuck at 180. Now a fresh drive form
            // starts the next leg.
            SetOdometer(game.Vehicle, 220);
            game.Vehicle.LastGraveCheckOdometer = 180;
            game.Tombstone.Add(new TombstoneEntity(0, 220, "Ghost", "welp", "Fort Kearney", "Chimney Rock", 10));

            var window = new Travel(game);
            var form = new ContinueOnTrail(window);
            form.OnFormPostCreate();

            // The cursor must survive the new form (it used to be re-seeded to the current odometer here, which is the bug).
            Assert.Equal(180, game.Vehicle.LastGraveCheckOdometer);

            // And the very first tick of the new leg picks up the grave that was dropped at the previous landmark.
            form.OnTick(false, false);
            Assert.IsType<TombstoneQuestion>(window.CurrentForm);
        }

        [Fact]
        public void ResetVehicle_ClearsTheScanCursor_ForAFreshJourney()
        {
            var game = GameSimulationApp.Instance;
            game.Vehicle.LastGraveCheckOdometer = 999;

            game.Vehicle.ResetVehicle();

            // A new game must start scanning the trail from mile zero, not from a previous journey's odometer, or every grave
            // before that stale mark would be invisible.
            Assert.Equal(0, game.Vehicle.LastGraveCheckOdometer);
        }

        private static void SetOdometer(Vehicle vehicle, int odometer) =>
            typeof(Vehicle).GetProperty(nameof(Vehicle.Odometer))!.SetValue(vehicle, odometer);
    }
}
