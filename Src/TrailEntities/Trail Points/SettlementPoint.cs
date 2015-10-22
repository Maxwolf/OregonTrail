using System;
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
        private bool _canRest;
        private StoreMode _storeMode;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailEntities.Settlement" /> class.
        /// </summary>
        public SettlementPoint(string name, ulong distanceLength, bool canRest) : base(name, distanceLength)
        {
            _canRest = canRest;
            _storeMode = null;
        }

        public override SimulationMode Mode
        {
            get { return SimulationMode.Settlement; }
        }

        public bool CanRest
        {
            get { return _canRest; }
        }

        public IStoreMode StoreMode
        {
            get { return _storeMode; }
        }

        public void GoToStore()
        {
            // TODO: Create instance of store.
            throw new NotImplementedException();
        }
    }
}