using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using TrailCommon;

namespace TrailEntities
{
    /// <summary>
    ///     Manages a general store where the player can buy food, clothes, bullets, and parts for their vehicle.
    /// </summary>
    public class StoreMode : IStore
    {
        private readonly string _storeName;
        private List<IItem> _inventory;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailEntities.Store" /> class.
        /// </summary>
        protected StoreMode(string storeName, uint storeBalance, IVehicle vehicle, List<IItem> inventory)
        {
            _inventory = inventory;
            StoreBalance = storeBalance;
            _storeName = storeName;
            Vehicle = vehicle;
        }

        public ModeType Mode
        {
            get { return ModeType.Store; }
        }

        public void TickMode()
        {
            throw new NotImplementedException();
        }

        public IVehicle Vehicle { get; }

        public ReadOnlyCollection<IItem> StoreInventory
        {
            get { return new ReadOnlyCollection<IItem>(_inventory); }
        }

        public string StoreName
        {
            get { return _storeName; }
        }

        public uint StoreBalance { get; private set; }

        public void BuyItems(IItem item)
        {
            var playerCost = item.Cost*item.Quantity;
            if (Vehicle.Balance >= playerCost)
            {
                // Store earns the money from vehicle.
                StoreBalance += playerCost;
                Vehicle.BuyItem(item);
            }
        }

        public void SellItem(IItem item)
        {
            var storeCost = item.Cost*item.Quantity;
            if (StoreBalance >= storeCost)
            {
                StoreBalance -= storeCost;
                BuyItems(item);
            }
        }

        public IStore StoreController
        {
            get { return this; }
        }
    }
}