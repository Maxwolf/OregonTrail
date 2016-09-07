// Created by Ron 'Maxwolf' McDowell (ron.mcdowell@gmail.com) 
// Timestamp 01/03/2016@1:50 AM

using System.Diagnostics.CodeAnalysis;
using OregonTrailDotNet.TrailSimulation.Entity;
using OregonTrailDotNet.TrailSimulation.Module.Director;
using OregonTrailDotNet.TrailSimulation.Window.RandomEvent;

namespace OregonTrailDotNet.TrailSimulation.Event.Vehicle
{
    /// <summary>
    ///     Oxen is damaged, which decreases the ability for the vehicle to be pulled forward. It is possible for this event to
    ///     make the vehicle stuck, unable to continue until the player acquires another oxen via trading.
    /// </summary>
    [DirectorEvent(EventCategory.Vehicle)]
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public sealed class OxenDied : EventProduct
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
            var vehicle = eventExecutor.SourceEntity as Entity.Vehicle.Vehicle;

            // Skip if the source entity is not a vehicle.
            if (vehicle == null)
                return;

            // Damages the oxen, could make vehicle stuck.
            vehicle.Inventory[Entities.Animal].ReduceQuantity(1);

            // Reduce the total possible mileage of the vehicle this turn.
            vehicle.ReduceMileage(25);
        }

        /// <summary>
        ///     Fired when the simulation would like to render the event, typically this is done AFTER executing it but this could
        ///     change depending on requirements of the implementation.
        /// </summary>
        /// <param name="userData"></param>
        /// <returns>Text user interface string that can be used to explain what the event did when executed.</returns>
        protected override string OnRender(RandomEventInfo userData)
        {
            return "ox injures leg---you have to put it down";
        }
    }
}