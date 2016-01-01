// Created by Ron 'Maxwolf' McDowell (ron.mcdowell@gmail.com) 
// Timestamp 01/01/2016@3:27 AM

namespace TrailSimulation.Event.Prefab
{
    using Entity;
    using Entity.Vehicle;
    using Module.Director;
    using Window.RandomEvent;

    /// <summary>
    ///     Destroys a random amount of food from the vehicles inventory, the amount of food destroyed typically will be about
    ///     one-third (1/3) of the entire food quantity the player has as their food storage.
    /// </summary>
    public abstract class FoodDestroyer : EventProduct
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

            // Skip if the source entity is not a vehicle.
            if (vehicle == null)
                return;

            // Check there is food to even remove.
            if (vehicle.Inventory[Entities.Food].Quantity <= 0)
                return;

            // Check if there is enough food to cut up into four pieces.
            if (vehicle.Inventory[Entities.Food].Quantity < 4)
                return;

            // Determine the amount of food we will destroy up to.
            var spoiledFood = vehicle.Inventory[Entities.Food].Quantity/4;

            // Remove some random amount of food, the minimum being three pieces.
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
            return OnFoodSpoilReason();
        }

        /// <summary>
        ///     Fired by the food spoiler event prefab allowing implementations to explain the reason why the food went bad and or
        ///     was destroyed.
        /// </summary>
        /// <returns>Reason why the food was destroyed and or went bad.</returns>
        protected abstract string OnFoodSpoilReason();
    }
}