using System;
using System.Collections.Generic;
using TrailCommon;

namespace TrailEntities
{
    /// <summary>
    ///     Manages a general store where the player can buy food, clothes, bullets, and parts for their vehicle.
    /// </summary>
    public abstract class Store : IStore
    {
        private uint _balance;
        private SortedSet<IItem> _inventory;
        private IVehicle _vehicle;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailEntities.Store" /> class.
        /// </summary>
        protected Store(IVehicle vehicle)
        {
            _balance = 0;
            _inventory = new SortedSet<IItem>();
            _vehicle = vehicle;
        }

        public IVehicle Vehicle
        {
            get { return _vehicle; }
        }

        public SortedSet<IItem> Inventory
        {
            get { return _inventory; }
        }

        public uint Balance
        {
            get { return _balance; }
        }

        public void BuyItems(IItem item)
        {
            throw new NotImplementedException();
        }

        public void SellItem(IItem item)
        {
            throw new NotImplementedException();
        }
    }
}