using System;
using System.Collections.Generic;
using TrailSimulation.Entity;

namespace TrailSimulation.Game
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
        ///     Initializes a new instance of the <see cref="T:TrailSimulation.Game.Location" /> class.
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

        /// <summary>
        ///     Determines if this location has a store, it is built up in the constructor for the location itself and will help
        ///     determine menu items shown.
        /// </summary>
        public bool HasStore { get; }

        /// <summary>
        ///     Determines if this location has already been visited by the vehicle and party members.
        /// </summary>
        /// <returns>TRUE if location has been passed by, FALSE if location has yet to be reached.</returns>
        public bool HasVisited { get; private set; }

        /// <summary>
        ///     Determines if the vehicle has reached this location or not, this can only be set once and will throw an exception
        ///     is hit after it is set.
        /// </summary>
        public void SetVisited()
        {
            // Complain if visited is already true.
            if (HasVisited)
                throw new InvalidOperationException("Visited flag for location " + Name +
                                                    " has already been set to TRUE. Cannot do this twice!");

            HasVisited = true;
        }
    }
}