using System.Collections.Generic;

namespace TrailEntities
{
    /// <summary>
    ///     Defines a location in the game that is added to a list of points that make up the entire trail which the player and
    ///     his vehicle travel upon.
    /// </summary>
    public abstract class PointOfInterest : IPoint
    {
        /// <summary>
        ///     Reference to all of the items this landmark has for sale in a store.
        /// </summary>
        private HashSet<Item> _storeItems;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailEntities.PointOfInterest" /> class.
        /// </summary>
        protected PointOfInterest(
            string name,
            ulong distanceLength,
            IEnumerable<Item> pointInventory = null,
            bool canRest = true)
        {
            // Name of the point as it should be known to the player.
            Name = name;

            // How many 'miles' the player and his vehicle must travel to reach this point.
            DistanceLength = distanceLength;

            // Use the null coalescing operator and an instance of empty List.
            _storeItems = new HashSet<Item>(pointInventory ?? new HashSet<Item>());

            // We know the settlement has a store if there are items in it to sell.
            HasStore = _storeItems.Count > 0;

            // Setup basic information about this settlement.
            CanRest = canRest;
        }

        /// <summary>
        ///     Determines the total number of 'miles' which the vehicle must travel before it is considered to have arrived at the
        ///     next point of interest.
        /// </summary>
        public ulong DistanceLength { get; }

        /// <summary>
        ///     Defines the current game mode the inheriting class is going to take responsibility for when attached to the
        ///     simulation.
        /// </summary>
        public abstract ModeType ModeType { get; }

        /// <summary>
        ///     Name of the current point of interest as it should be known to the player.
        /// </summary>
        public string Name { get; }

        /// <summary>
        ///     Defines a list of items which the store has inside of it which were established in the constructor for the store.
        /// </summary>
        public IEnumerable<Item> StoreItems
        {
            get { return _storeItems; }
        }

        /// <summary>
        ///     Determines if this location has a store, calculation is made by checking count of store items making it a computed
        ///     property.
        /// </summary>
        public bool HasStore { get; }

        /// <summary>
        ///     Determines if the player can rest here, it is an optional parameter with default value of true. Typically the only
        ///     time in a trail where you cannot rest is when you reach the last location in the game before point calculation game
        ///     mode is attached.
        /// </summary>
        public bool CanRest { get; }
    }
}