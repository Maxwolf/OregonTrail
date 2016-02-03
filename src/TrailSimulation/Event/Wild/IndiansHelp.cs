// Created by Ron 'Maxwolf' McDowell (ron.mcdowell@gmail.com) 
// Timestamp 01/03/2016@1:50 AM

namespace TrailSimulation
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    ///     Indians help you find some free food, this event will be called manually more often if you are low on food to
    ///     simulate the effect of them noticing you need help.
    /// </summary>
    [DirectorEvent(EventCategory.Wild)]
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public sealed class IndiansHelp : EventProduct
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
            // Cast the source entity as vehicle.
            var vehicle = eventExecutor.SourceEntity as Vehicle;

            // Indians hook you up with free food, what nice guys.
            vehicle?.Inventory[Entities.Food].AddQuantity(14);
        }

        /// <summary>
        ///     Fired when the simulation would like to render the event, typically this is done AFTER executing it but this could
        ///     change depending on requirements of the implementation.
        /// </summary>
        /// <param name="userData"></param>
        /// <returns>Text user interface string that can be used to explain what the event did when executed.</returns>
        protected override string OnRender(RandomEventInfo userData)
        {
            return "helpful Indians show you where to find more food";
        }
    }
}