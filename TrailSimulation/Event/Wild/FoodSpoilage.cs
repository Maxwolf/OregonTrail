// Created by Ron 'Maxwolf' McDowell (ron.mcdowell@gmail.com) 
// Timestamp 01/01/2016@3:27 AM

namespace TrailSimulation.Event.Wild
{
    using Module.Director;
    using Prefab;

    /// <summary>
    ///     Causes some of the vehicle food stores to be lost due to spoilage or improper storage. The amount taken will be
    ///     randomly generated but never go above quarter of the total food reserves.
    /// </summary>
    [DirectorEvent(EventCategory.Wild)]
    public sealed class FoodSpoilage : FoodDestroyer
    {
        /// <summary>
        ///     Fired by the food spoiler event prefab allowing implementations to explain the reason why the food went bad and or
        ///     was destroyed.
        /// </summary>
        /// <returns>Reason why the food was destroyed and or went bad.</returns>
        protected override string OnFoodSpoilReason()
        {
            return "Food spoilage.";
        }
    }
}