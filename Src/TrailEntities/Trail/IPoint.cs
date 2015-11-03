using System.Collections.Generic;

namespace TrailEntities
{
    /// <summary>
    ///     Defines a point on the trail where the player can buy items, talk to people, and rest without worry of thieves.
    /// </summary>
    public interface IPoint
    {
        /// <summary>
        ///     Name of the landmark as it should be shown to the player in the simulation.
        /// </summary>
        string Name { get; }

        /// <summary>
        ///     Defines a list of items which the store has inside of it which were established in the constructor for the store.
        /// </summary>
        IEnumerable<Item> StoreItems { get; }

        /// <summary>
        ///     Determines if this location has a store, calculation is made by checking count of store items making it a computed
        ///     property.
        /// </summary>
        bool HasStore { get; }

        /// <summary>
        ///     Determines if the player can rest here, it is an optional parameter with default value of true. Typically the only
        ///     time in a trail where you cannot rest is when you reach the last location in the game before point calculation game
        ///     mode is attached.
        /// </summary>
        bool CanRest { get; }
    }
}