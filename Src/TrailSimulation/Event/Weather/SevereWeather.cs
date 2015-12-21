// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SevereWeather.cs" company="Ron 'Maxwolf' McDowell">
//   ron.mcdowell@gmail.com
// </copyright>
// <summary>
//   Severe weather will cause destruction of items and waste your time, but nobody will get killed.
// </summary>
// --------------------------------------------------------------------------------------------------------------------



using System;
using System.Collections.Generic;
using TrailSimulation.Entity;
using TrailSimulation.Game;

namespace TrailSimulation.Event
{
    /// <summary>
    ///     Severe weather will cause destruction of items and waste your time, but nobody will get killed.
    /// </summary>
    [DirectorEvent(EventCategory.Weather, EventExecution.ManualOnly)]
    public sealed class SevereWeather : EventItemDestroyer
    {
        /// <summary>
        /// Fired by the item destroyer event prefab before items are destroyed.
        /// </summary>
        /// <param name="destroyedItems">
        /// Items that were destroyed from the players inventory.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        protected override string OnPostDestroyItems(IDictionary<Entities, int> destroyedItems)
        {
            return destroyedItems.Count > 0
                ? $"time and supplies lost:{Environment.NewLine}"
                : "no items lost.";
        }

        /// <summary>
        /// Fired by the item destroyer event prefab after items are destroyed.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        protected override string OnPreDestroyItems()
        {
            return "heavy rains---";
        }
    }
}