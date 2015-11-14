using System.Collections.Generic;
using TrailEntities.Entity;

namespace TrailEntities.Game
{
    /// <summary>
    ///     Defines a location in the game that is added to a list of points that make up the entire trail which the player and
    ///     his vehicle travel upon.
    /// </summary>
    public class Location
    {
        /// <summary>
        ///     Reference to all of the items this landmark has for sale in a store.
        /// </summary>
        private Dictionary<SimEntity, SimItem> _storeItems;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailEntities.Game.Location" /> class.
        /// </summary>
        public Location(string name, bool storeOpen = false)
        {
            // Name of the point as it should be known to the player.
            Name = name;

            // We know the settlement has a store if there are items in it to sell.
            _storeItems = new Dictionary<SimEntity, SimItem>(GameSimulationApp.DefaultInventory);
            HasStore = _storeItems.Count > 0 && storeOpen;
        }

        /// <summary>
        ///     Defines the current game mode the inheriting class is going to take responsibility for when attached to the
        ///     simulation.
        /// </summary>
        public virtual GameMode GameMode
        {
            get { return GameMode.Travel; }
        }

        /// <summary>
        ///     Name of the current point of interest as it should be known to the player.
        /// </summary>
        public string Name { get; }

        /// <summary>
        ///     Defines a list of items which the store has inside of it which were established in the constructor for the store.
        /// </summary>
        public IDictionary<SimEntity, SimItem> StoreItems
        {
            get { return _storeItems; }
        }

        public bool HasStore { get; }
    }
}