// Created by Ron 'Maxwolf' McDowell (ron.mcdowell@gmail.com) 
// Timestamp 01/03/2016@1:50 AM

using System;
using OregonTrailDotNet.TrailSimulation.Entity.Vehicle;
using OregonTrailDotNet.TrailSimulation.Module.Director;
using OregonTrailDotNet.TrailSimulation.Window.RandomEvent;

namespace OregonTrailDotNet.TrailSimulation.Event.Vehicle
{
    /// <summary>
    ///     Manually triggered when random event system damages or makes equipment on the vehicle malfunction. This gives the
    ///     player a chance to magically fix the vehicle part when they attempt to fix it. If the player tries to repair it
    ///     this form is shown if the dice roll went in their favor, otherwise they better hope they have a spare part to fix
    ///     it or they are going to be stranded in a broken vehicle unable to continue their journey.
    /// </summary>
    [DirectorEvent(EventCategory.Vehicle, EventExecution.ManualOnly)]
    public sealed class RepairVehiclePart : EventProduct
    {
        /// <summary>
        ///     Fired when the event handler associated with this enum type triggers action on target entity. Implementation is
        ///     left completely up to handler.
        /// </summary>
        /// <param name="userData">
        ///     Entities which the event is going to directly affect. This way there is no confusion about
        ///     what entity the event is for. Will require casting to correct instance type from interface instance.
        /// </param>
        public override void Execute(RandomEventInfo userData)
        {
            // Nothing to see here, move along...
        }

        /// <summary>
        ///     Fired after the event is executed and allows the inheriting event prefab know post event execution.
        /// </summary>
        /// <param name="eventExecutor">Form that executed the event from the random event window.</param>
        internal override bool OnPostExecute(EventExecutor eventExecutor)
        {
            // Check to make sure the source entity is a vehicle.
            var vehicle = eventExecutor.UserData.SourceEntity as Entity.Vehicle.Vehicle;
            if (vehicle == null)
                return true;

            // Ensures the vehicle will be able to continue down the trail.
            vehicle.Status = VehicleStatus.Stopped;
            return false;
        }

        /// <summary>
        ///     Fired when the simulation would like to render the event, typically this is done AFTER executing it but this could
        ///     change depending on requirements of the implementation.
        /// </summary>
        /// <param name="userData">
        ///     Entities which the event is going to directly affect. This way there is no confusion about
        ///     what entity the event is for. Will require casting to correct instance type from interface instance.
        /// </param>
        /// <returns>Text user interface string that can be used to explain what the event did when executed.</returns>
        protected override string OnRender(RandomEventInfo userData)
        {
            return $"{Environment.NewLine}You were able to repair the " +
                   $"{GameSimulationApp.Instance.Vehicle.BrokenPart.Name.ToLowerInvariant()}.{Environment.NewLine}";
        }
    }
}