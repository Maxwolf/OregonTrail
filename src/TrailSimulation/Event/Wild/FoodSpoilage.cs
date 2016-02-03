// Created by Ron 'Maxwolf' McDowell (ron.mcdowell@gmail.com) 
// Timestamp 01/03/2016@1:50 AM

namespace TrailSimulation
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    ///     Causes some of the vehicle food stores to be lost due to spoilage or improper storage. The amount taken will be
    ///     randomly generated but never go above quarter of the total food reserves.
    /// </summary>
    [DirectorEvent(EventCategory.Wild)]
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
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