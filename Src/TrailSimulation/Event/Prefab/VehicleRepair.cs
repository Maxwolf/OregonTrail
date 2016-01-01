// Created by Ron 'Maxwolf' McDowell (ron.mcdowell@gmail.com) 
// Timestamp 12/31/2015@3:04 AM

namespace TrailSimulation
{
    /// <summary>
    ///     Used by events that would like to magically repair the vehicle, typically this is called after removing spare parts
    ///     from the vehicles inventory but it could also be called by anything since it actually performs the work of
    ///     repairing the vehicle.
    /// </summary>
    public abstract class VehicleRepair : EventProduct
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
            vehicle?.RepairAllParts();
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
            return OnVehicleRepairReason(_partName);
        }

        /// <summary>
        ///     Called by the vehicle repair prefab so implementations can return the reason why the vehicle was repaired for the
        ///     player so they know why it happened.
        /// </summary>
        /// <param name="partName">Name of the part that was repaired on the vehicle.</param>
        /// <returns>Formatted string that contains the reason why the vehicle was repaired.</returns>
        protected abstract string OnVehicleRepairReason(string partName);
    }
}