// Created by Ron 'Maxwolf' McDowell (ron.mcdowell@gmail.com) 
// Timestamp 12/12/2015@6:43 AM

namespace TrailSimulation.Event
{
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using Entity;
    using Game;

    /// <summary>
    ///     Causes some of the vehicle food stores to be lost due to spoilage or improper storage. The amount taken will be
    ///     randomly generated but never go above quarter of the total food reserves.
    /// </summary>
    [DirectorEvent(EventCategory.Wild)]
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public sealed class FoodSpoilage : EventProduct
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
            Debug.Assert(vehicle != null, "vehicle != null");

            // Check there is food to even remove.
            if (vehicle.Inventory[Entities.Food].Quantity <= 0)
                return;

            // Check if there is enough food to cut up into four pieces.
            if (vehicle.Inventory[Entities.Food].Quantity < 4)
                return;

            // Determine the amount of food we will destroy up to.
            var spoiledFood = vehicle.Inventory[Entities.Food].Quantity/4;

            // Remove some random amount of food, the minimum being three pieces.
            // Note: If you only had four this would take 3/4 of food but you already had almost none.
            vehicle.Inventory[Entities.Food].ReduceQuantity(GameSimulationApp.Instance.Random.Next(3, spoiledFood));
        }

        /// <summary>
        ///     Fired when the simulation would like to render the event, typically this is done AFTER executing it but this could
        ///     change depending on requirements of the implementation.
        /// </summary>
        /// <param name="userData"></param>
        /// <returns>Text user interface string that can be used to explain what the event did when executed.</returns>
        protected override string OnRender(RandomEventInfo userData)
        {
            return "Food spoilage.";
        }
    }
}