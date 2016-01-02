// Created by Ron 'Maxwolf' McDowell (ron.mcdowell@gmail.com) 
// Timestamp 01/01/2016@7:40 PM

namespace TrailSimulation
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    ///     Used when we want to trigger special event that will damage one of the vehicles parts making it unable to continue
    ///     the journey until the player decides to either repair or replace the part in question.
    /// </summary>
    [DirectorEvent(EventCategory.Vehicle)]
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public sealed class BrokenVehiclePart : EventProduct
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
            // Cast the source entity as vehicle.
            var vehicle = userData.SourceEntity as Vehicle;

            // Skip if there is no vehicle to affect.
            if (vehicle == null)
                return;

            // Break some random part on the vehicle.
            userData.BrokenPart = vehicle.BreakRandomPart();

            // Set vehicle broken status.
            vehicle.Status = VehicleStatus.Broken;
        }

        /// <summary>
        ///     Fired after the event is executed and allows the inheriting event prefab know post event execution.
        /// </summary>
        /// <param name="eventExecutor">Form that executed the event from the random event window.</param>
        internal override bool OnPostExecute(EventExecutor eventExecutor)
        {
            base.OnPostExecute(eventExecutor);

            // Check to make sure the source entity is a vehicle.
            var vehicle = eventExecutor.UserData.SourceEntity as Vehicle;
            if (vehicle == null)
                return false;

            // Check to make sure we should load the broken vehicle form.
            if (eventExecutor.UserData.BrokenPart == null ||
                vehicle.BrokenPart == null ||
                vehicle.Status != VehicleStatus.Broken)
                return false;

            // Loads form for random event system that deals with broken vehicle parts.
            eventExecutor.SetForm(typeof (EventVehicleBroken));
            return true;
        }

        /// <summary>
        ///     Fired when the simulation would like to render the event, typically this is done AFTER executing it but this could
        ///     change depending on requirements of the implementation.
        /// </summary>
        /// <param name="userData"></param>
        /// <returns>Text user interface string that can be used to explain what the event did when executed.</returns>
        protected override string OnRender(RandomEventInfo userData)
        {
            // Cast the source entity as vehicle.
            var vehicle = userData.SourceEntity as Vehicle;
            return $"Broken vehicle {vehicle?.BrokenPart.Name.ToLowerInvariant()}.";
        }
    }
}