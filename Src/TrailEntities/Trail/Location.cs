using System.Collections.Generic;

namespace TrailEntities
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
        ///     Initializes a new instance of the <see cref="T:TrailEntities.Location" /> class.
        /// </summary>
        public Location(
            string name,
            int distanceLength)
        {
            // Name of the point as it should be known to the player.
            Name = name;

            // How many 'miles' the player and his vehicle must travel to reach this point.
            DistanceLength = distanceLength;

            // Use the null coalescing operator and an instance of empty List.
            _storeItems = new Dictionary<SimEntity, SimItem>(Resources.DefaultStore);

            // We know the settlement has a store if there are items in it to sell.
            HasStore = _storeItems.Count > 0;
        }

        /// <summary>
        ///     Determines the total number of 'miles' which the vehicle must travel before it is considered to have arrived at the
        ///     next point of interest.
        /// </summary>
        public int DistanceLength { get; }

        /// <summary>
        ///     Defines the current game mode the inheriting class is going to take responsibility for when attached to the
        ///     simulation.
        /// </summary>
        public virtual ModeType ModeType
        {
            get { return ModeType.Travel; }
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
        ///     Determines if this location has a store, calculation is made by checking count of store items making it a computed
        ///     property.
        /// </summary>
        public bool HasStore { get; }
    }
}