// Created by Ron 'Maxwolf' McDowell (ron.mcdowell@gmail.com) 
// Timestamp 01/01/2016@7:40 PM

namespace TrailSimulation
{
    using System;

    /// <summary>
    ///     Called by the simulation when the player uses a spare part to repair their vehicle. This event will not actually
    ///     remove the item it should be done before this is attached, the event serves as more of a dialog box to inform the
    ///     user that the repair has taken place and the item removed from vehicle inventory.
    /// </summary>
    [DirectorEvent(EventCategory.Vehicle, EventExecution.ManualOnly)]
    public sealed class UseSparePart : EventProduct
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
            throw new NotImplementedException();
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
                               $"{GameSimulationApp.Instance.Vehicle.BrokenPart.Name.ToLowerInvariant()} using your spare.{Environment.NewLine}";
        }
    }
}