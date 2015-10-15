using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using TrailCommon;
using TrailEntities;

namespace TrailGame
{
    public class StoreMode : GameMode, IStore
    {
        private List<IItem> _inventory;
        private readonly string _storeName;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailEntities.GameWindow" /> class.
        /// </summary>
        public StoreMode(Vehicle vehicle, List<IItem> inventory, string storeName, uint storeBalance) : base(vehicle)
        {
            _inventory = inventory;
            _storeName = storeName;
            StoreBalance = storeBalance;

            Console.WriteLine("Welcome to " + storeName);
        }

        public override string Name
        {
            get { return "Store"; }
        }

        public ReadOnlyCollection<IItem> Inventory
        {
            get { return new ReadOnlyCollection<IItem>(_inventory); }
        }

        public string StoreName
        {
            get { return _storeName; }
        }

        public uint StoreBalance { get; private set; }

        /// <summary>
        ///     Remove the item from store, give to vehicle.
        /// </summary>
        public void BuyItems(IItem item)
        {
            var playerCost = item.Cost*item.Quantity;
            if (_vehicle.Balance >= playerCost)
            {
                // Store earns the money from vehicle.
                StoreBalance += playerCost;
                _vehicle.BuyItem(item);
            }
        }

        /// <summary>
        ///     Give the item to store, remove from vehicle.
        /// </summary>
        public void SellItem(IItem item)
        {
            var storeCost = item.Cost*item.Quantity;
            if (StoreBalance >= storeCost)
            {
                StoreBalance -= storeCost;
                BuyItems(item);
            }
        }
    }
}