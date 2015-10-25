using System.Collections.Generic;
using System.Collections.ObjectModel;
using TrailCommon;

namespace TrailEntities
{
    /// <summary>
    ///     Manages a general store where the player can buy food, clothes, bullets, and parts for their vehicle.
    /// </summary>
    public sealed class StoreMode : GameMode<StoreCommands>, IStoreMode
    {
        private List<IItem> _inventory;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailEntities.StoreMode" /> class.
        /// </summary>
        public StoreMode()
        {
            StoreName = "Unknown General Store";
            _inventory = new List<IItem>();
            StoreBalance = (uint) GameSimulationApp.Instance.Random.Next(100, 800);
        }

        public override ModeType ModeType
        {
            get { return ModeType.Store; }
        }

        public ReadOnlyCollection<IItem> StoreInventory
        {
            get { return new ReadOnlyCollection<IItem>(_inventory); }
        }

        public string StoreName { get; }

        public float StoreBalance { get; private set; }

        public void BuyItems(IItem item)
        {
            var playerCost = item.Cost*item.Quantity;
            if (GameSimulationApp.Instance.Vehicle.Balance < playerCost)
                return;

            // Store earns the money from vehicle.
            StoreBalance += playerCost;
            GameSimulationApp.Instance.Vehicle.BuyItem(item);
        }

        public void SellItem(IItem item)
        {
            var storeCost = item.Cost*item.Quantity;
            if (StoreBalance < storeCost)
                return;

            StoreBalance -= storeCost;
            BuyItems(item);
        }
    }
}