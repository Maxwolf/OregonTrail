using System;
using TrailCommon;

namespace TrailEntities
{
    /// <summary>
    ///     Defines a location that has a store, people to talk to, safe to rest. Forts are always in good condition since they
    ///     are run
    ///     by the military and always have a source of funding to perform maintenance and upkeep.
    /// </summary>
    public abstract class Settlement : PointOfInterest, ISettlement
    {
        private bool _canRest;
        private Store _store;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailEntities.Settlement" /> class.
        /// </summary>
        protected Settlement(string name, bool canRest, Store store) : base(name)
        {
            _canRest = canRest;
            _store = store;
        }

        public bool CanRest
        {
            get { return _canRest; }
        }

        public IStore Store
        {
            get { return _store; }
        }

        public void GoToStore()
        {
            throw new NotImplementedException();
        }
    }
}