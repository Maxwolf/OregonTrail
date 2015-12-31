// Created by Ron 'Maxwolf' McDowell (ron.mcdowell@gmail.com) 
// Timestamp 12/31/2015@2:14 AM

namespace TrailSimulation.Event
{
    using System;
    using Entity;
    using Game;

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
        ///     Defines the name of the broken part that was on the vehicle before the player magically repaired it.
        /// </summary>
        private string _partName;

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
            // Cast the source entity as vehicle.
            var vehicle = userData.SourceEntity as Vehicle;

            // Grab the name of the broken part before we clear it out.
            _partName = vehicle?.BrokenPart.Name.ToLowerInvariant();

            // Fix the vehicle and change it's status to stopped.
            vehicle?.RepairParts();
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
            return $"{Environment.NewLine}You were able to repair the vehicle" +
                   $"{_partName}.{Environment.NewLine}";
        }
    }
}