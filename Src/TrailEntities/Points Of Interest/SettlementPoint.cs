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
        private List<IItem> _storeItems;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailEntities.Settlement" /> class.
        /// </summary>
        public SettlementPoint(string name, ulong distanceLength, bool canRest, IEnumerable<Item> storeStock = null)
            : base(name, distanceLength)
        {
            // Use the null coalescing operator and an instance of empty List.
            _storeItems = new List<IItem>(storeStock ?? new List<Item>());

            // We know the settlement has a store if there are items in it to sell.
            HasStore = _storeItems.Count > 0;

            // Setup basic information about this settlement.
            CanRest = canRest;
        }

        public override ModeType ModeType
        {
            get { return ModeType.Settlement; }
        }

        public bool CanRest { get; }
        public bool HasStore { get; }

        public ReadOnlyCollection<IItem> StoreItems
        {
            get { return _storeItems.AsReadOnly(); }
        }

        public void GoToStore()
        {
            // Store takes all needed data from current trail point location.
            GameSimulationApp.Instance.AddMode(ModeType.Store);
        }
    }
}