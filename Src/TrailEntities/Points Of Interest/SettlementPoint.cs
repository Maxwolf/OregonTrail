using System.Collections.Generic;
using System.Collections.ObjectModel;
using TrailCommon;

namespace TrailEntities
{
    /// <summary>
    ///     Defines a location that has a store, people to talk to, safe to rest. Forts are always in good condition since they
    ///     are run
    ///     by the military and always have a source of funding to perform maintenance and upkeep.
    /// </summary>
    public sealed class SettlementPoint : PointOfInterest, ISettlementPoint
    {
        private List<Item> _storeItems;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailEntities.Settlement" /> class.
        /// </summary>
        public SettlementPoint(string name, ulong distanceLength, bool canRest, IEnumerable<Item> pointInventory = null)
            : base(name, distanceLength)
        {
            // Use the null coalescing operator and an instance of empty List.
            _storeItems = new List<Item>(pointInventory ?? new List<Item>());

            // We know the settlement has a store if there are items in it to sell.
            HasStore = _storeItems.Count > 0;

            // Create a random balance for the store to work with when we create it.
            PointBalance = (uint) GameSimulationApp.Instance.Random.Next(50, 1000);

            // Setup basic information about this settlement.
            CanRest = canRest;
        }

        public uint PointBalance { get; }

        public override ModeType ModeType
        {
            get { return ModeType.Settlement; }
        }

        public ReadOnlyCollection<Item> StoreItems
        {
            get { return _storeItems.AsReadOnly(); }
        }

        public bool HasStore { get; }

        public bool CanRest { get; }

        public float StoreBalance
        {
            get { return PointBalance; }
        }

        public void GoToStore()
        {
            // Store takes all needed data from current trail point location.
            GameSimulationApp.Instance.AddMode(ModeType.Store);
        }
    }
}