// Created by Ron 'Maxwolf' McDowell (ron.mcdowell@gmail.com) 
// Timestamp 01/01/2016@3:27 AM

namespace TrailSimulation.Event.Vehicle
{
    using System;
    using Module.Director;
    using Window.RandomEvent;

    /// <summary>
    ///     Called by the vehicle broken event when it is closing out from user interaction. If it detects that vehicle
    ///     inventory does not contain a spare part of the type required by the vehicle to continue this will be presented to
    ///     let the player know they are going to be stuck and unable to continue this way it will not be a surprise when they
    ///     try to continue and the game tells them they cannot.
    /// </summary>
    [DirectorEvent(EventCategory.Vehicle, EventExecution.ManualOnly)]
    public sealed class NoSparePart : EventProduct
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
            return $"{Environment.NewLine}Since you don't have a spare {userData.BrokenPart.Name.ToLowerInvariant()}" +
                   $"you must trade for one.{Environment.NewLine}";
        }
    }
}