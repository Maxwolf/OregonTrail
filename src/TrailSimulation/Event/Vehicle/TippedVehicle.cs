// Created by Ron 'Maxwolf' McDowell (ron.mcdowell@gmail.com) 
// Timestamp 12/12/2015@6:09 AM

namespace TrailSimulation
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Text;

    /// <summary>
    ///     Vehicle was going around a bend, hit a bump, rough trail, or any of the following it now tipped over and supplies
    ///     could be destroyed and passengers can be crushed to death.
    /// </summary>
    [DirectorEvent(EventCategory.Vehicle)]
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public sealed class TippedVehicle : ItemDestroyer
    {
        /// <summary>Fired by the item destroyer event prefab before items are destroyed.</summary>
        /// <param name="destroyedItems">Items that were destroyed from the players inventory.</param>
        /// <returns>The <see cref="string" />.</returns>
        protected override string OnPostDestroyItems(IDictionary<Entities, int> destroyedItems)
        {
            // Change event text depending on if items were destroyed or not.
            return destroyedItems.Count > 0
                ? TryKillPassengers("crushed")
                : "no loss of items.";
        }

        /// <summary>
        ///     Fired by the item destroyer event prefab after items are destroyed.
        /// </summary>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        protected override string OnPreDestroyItems()
        {
            var capsizePrompt = new StringBuilder();
            capsizePrompt.Clear();
            capsizePrompt.AppendLine("The wagon tipped over.");
            capsizePrompt.Append("Results in ");
            return capsizePrompt.ToString();
        }
    }
}