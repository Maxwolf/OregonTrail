using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using TrailCommon;

namespace TrailEntities
{
    /// <summary>
    ///     Manages a general store where the player can buy food, clothes, bullets, and parts for their vehicle.
    /// </summary>
    public abstract class Store : IStore
    {
        private uint _storeBalance;
        private List<IItem> _inventory;
        private IVehicle _vehicle;
        private string _storeName;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailEntities.Store" /> class.
        /// </summary>
        protected Store(IVehicle vehicle)
        {
            _storeBalance = 0;
            _inventory = new List<IItem>();
            _vehicle = vehicle;
        }

        public IVehicle Vehicle
        {
            get { return _vehicle; }
        }

        public ReadOnlyCollection<IItem> Inventory
        {
            get { return new ReadOnlyCollection<IItem>(_inventory); }
        }

        public string StoreName
        {
            get { return _storeName; }
        }

        public uint StoreBalance
        {
            get { return _storeBalance; }
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