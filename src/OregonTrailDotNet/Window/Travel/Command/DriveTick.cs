using System;
using OregonTrailDotNet.Entity.Location;
using OregonTrailDotNet.Entity.Location.Point;
using OregonTrailDotNet.Entity.Vehicle;

namespace OregonTrailDotNet.Window.Travel.Command
{
    /// <summary>
    ///     What one simulated day on the trail actually does, extracted from <see cref="ContinueOnTrail" /> so the
    ///     graphical drive scene runs the identical simulation: check the vehicle, scan the odometer stretch for
    ///     graves, take the turn. The forms translate the result into their own form changes and visuals; nothing
    ///     here renders.
    /// </summary>
    internal static class DriveTick
    {
        /// <summary>What the day's tick amounted to, for the hosting form to dispatch on.</summary>
        internal enum ResultEnum
        {
            /// <summary>The vehicle is stopped; nothing happened.</summary>
            Stopped,

            /// <summary>The vehicle broke down and cannot continue — show the repair flow.</summary>
            Disabled,

            /// <summary>The party crossed a gravesite this stretch — offer to look closer. No turn was taken.</summary>
            GraveCrossed,

            /// <summary>An ordinary day of travel was simulated.</summary>
            Traveled
        }

        /// <summary>
        ///     The departure duties a drive form performs when it attaches mid-arrival: mark the location departed,
        ///     charge the fort-departure penalty, and roll the mountain-pass stuck chance. One-shot by the status
        ///     check — departing is only possible once.
        /// </summary>
        internal static void Depart()
        {
            var game = GameSimulationApp.Instance;

            if ((game.Trail.DistanceToNextLocation <= 0) ||
                (game.Trail.CurrentLocation.Status != LocationStatusEnum.Arrived))
                return;

            var departingLocation = game.Trail.CurrentLocation;

            // Leaving a fort costs the party most of a day getting resupplied and back on the trail, so the next
            // travel turn covers dramatically fewer miles.
            if (departingLocation is Settlement)
                game.Vehicle.FortDeparturePenalty = true;

            departingLocation.Status = LocationStatusEnum.Departed;

            // High mountain passes have a chance to leave the party stuck for several days as they head out.
            if ((departingLocation.StuckChance > 0) &&
                (game.Random.Next(100) < departingLocation.StuckChance))
                game.EventDirector.TriggerEvent(game.Vehicle,
                    typeof(OregonTrailDotNet.Event.Vehicle.StuckInMountains));
        }

        /// <summary>
        ///     One simulation tick of driving. The order is load-bearing and pinned by the tombstone-encounter tests:
        ///     status first, then the grave scan over the stretch covered since the last look (the cursor lives on
        ///     the vehicle so a grave in the last stretch before a landmark is caught on the following leg), and the
        ///     turn is only taken when no grave interrupted.
        /// </summary>
        internal static ResultEnum Run()
        {
            var game = GameSimulationApp.Instance;

            // Checks if the vehicle is stuck or broken, if not it is set to moving state.
            game.Vehicle.CheckStatus();

            switch (game.Vehicle.Status)
            {
                case VehicleStatusEnum.Stopped:
                    return ResultEnum.Stopped;
                case VehicleStatusEnum.Disabled:
                    return ResultEnum.Disabled;
                case VehicleStatusEnum.Moving:
                    if (game.Tombstone.FindTombstoneBetween(game.Vehicle.LastGraveCheckOdometer,
                            game.Vehicle.Odometer, out _))
                    {
                        game.Vehicle.LastGraveCheckOdometer = game.Vehicle.Odometer;
                        return ResultEnum.GraveCrossed;
                    }

                    game.Vehicle.LastGraveCheckOdometer = game.Vehicle.Odometer;

                    // Processes the next turn in the game simulation.
                    game.TakeTurn(false);
                    return ResultEnum.Traveled;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        ///     The player pulling up: stop the vehicle if it is moving. The form clears itself back to the menu.
        /// </summary>
        internal static void Stop()
        {
            if (GameSimulationApp.Instance.Vehicle.Status == VehicleStatusEnum.Moving)
                GameSimulationApp.Instance.Vehicle.Status = VehicleStatusEnum.Stopped;
        }
    }
}
