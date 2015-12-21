// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AbandonedVehicle.cs" company="Ron 'Maxwolf' McDowell">
//   ron.mcdowell@gmail.com
// </copyright>
// <summary>
//   Discover a vehicle on the side of the road that might have some items inside of it that will be added to the
//   players inventory.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using TrailSimulation.Entity;
using TrailSimulation.Game;

namespace TrailSimulation.Event
{
    /// <summary>
    ///     Discover a vehicle on the side of the road that might have some items inside of it that will be added to the
    ///     players inventory.
    /// </summary>
    [DirectorEvent(EventCategory.Wild)]
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public sealed class AbandonedVehicle : EventItemCreator
    {
        /// <summary>Fired by the event prefab after the event has executed.</summary>
        /// <param name="createdItems"></param>
        /// <returns>The <see cref="string"/>.</returns>
        protected override string OnPostCreateItems(IDictionary<Entities, int> createdItems)
        {
            return createdItems.Count > 0 ? $"and find:{Environment.NewLine}" : "but it is empty";
        }

        /// <summary>
        ///     Fired by the event prefab before the event has executed.
        /// </summary>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        protected override string OnPreCreateItems()
        {
            var _eventText = new StringBuilder();
            _eventText.AppendLine("You find an abandoned wagon,");
            return _eventText.ToString();
        }
    }
}