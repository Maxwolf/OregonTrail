// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FindBerries.cs" company="Ron 'Maxwolf' McDowell">
//   ron.mcdowell@gmail.com
// </copyright>
// <summary>
//   Vehicle comes across some wild berries which the party picks up to eat.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace TrailSimulation.Event
{
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using Entity;
    using Game;

    /// <summary>
    ///     Vehicle comes across some wild berries which the party picks up to eat.
    /// </summary>
    [DirectorEvent(EventCategory.Wild)]
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public sealed class FindBerries : EventProduct
    {
        /// <summary>Fired when the event handler associated with this enum type triggers action on target entity. Implementation is
        ///     left completely up to handler.</summary>
        /// <param name="userData">Entities which the event is going to directly affect. This way there is no confusion about
        ///     what entity the event is for. Will require casting to correct instance type from interface instance.</param>
        public override void Execute(RandomEventInfo userData)
        {
            // Cast the source entity as vehicle.
            var vehicle = userData.SourceEntity as Vehicle;
            Debug.Assert(vehicle != null, "vehicle != null");

            // Add the berries to vehicle food stores.
            vehicle.Inventory[Entities.Food].AddQuantity(5);
        }

        /// <summary>Fired when the simulation would like to render the event, typically this is done AFTER executing it but this could
        ///     change depending on requirements of the implementation.</summary>
        /// <param name="userData"></param>
        /// <returns>Text user interface string that can be used to explain what the event did when executed.</returns>
        protected override string OnRender(RandomEventInfo userData)
        {
            return "Find wild berries";
        }
    }
}