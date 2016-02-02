// Created by Ron 'Maxwolf' McDowell (ron.mcdowell@gmail.com) 
// Timestamp 01/03/2016@1:50 AM

namespace TrailSimulation
{
    using System.Text;

    /// <summary>
    ///     Manually triggered event for when the player decides to not and repair their vehicle and instead opts to use spare
    ///     parts and or willingly want to be stranded unable to continue their journey.
    /// </summary>
    [DirectorEvent(EventCategory.Vehicle, EventExecution.ManualOnly)]
    public sealed class NoRepairVehicle : EventProduct
    {
        /// <summary>
        ///     Fired when the event handler associated with this enum type triggers action on target entity. Implementation is
        ///     left completely up to handler.
        /// </summary>
        /// <param name="eventExecutor">
        ///     Entities which the event is going to directly affect. This way there is no confusion about
        ///     what entity the event is for. Will require casting to correct instance type from interface instance.
        /// </param>
        public override void Execute(RandomEventInfo eventExecutor)
        {
            // Nothing to see here, move along...
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
            var repairPrompt = new StringBuilder();
            repairPrompt.AppendLine("You did not repair the broken ");
            repairPrompt.AppendLine(
                $"{GameSimulationApp.Instance.Vehicle.BrokenPart.Name.ToLowerInvariant()}. You must replace it with a ");
            repairPrompt.Append("spare part.");
            return repairPrompt.ToString();
        }

        /// <summary>
        ///     Fired after the event is executed and allows the inheriting event prefab know post event execution.
        /// </summary>
        /// <param name="eventExecutor">Form that executed the event from the random event window.</param>
        internal override bool OnPostExecute(EventExecutor eventExecutor)
        {
            // Check to make sure the source entity is a vehicle.
            var vehicle = eventExecutor.UserData.SourceEntity as Vehicle;

            // Skip if the vehicle is null.
            if (vehicle == null)
                return false;

            // Check to ensure 
            eventExecutor.SetForm(vehicle.TryUseSparePart()
                ? typeof(VehicleUseSparePart)
                : typeof(VehicleNoSparePart));

            // Default response allows event to execute normally.
            return false;
        }
    }
}