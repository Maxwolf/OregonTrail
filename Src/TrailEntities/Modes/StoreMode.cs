using System.Collections.Generic;
using System.Collections.ObjectModel;
using TrailCommon;

namespace TrailEntities
{
    /// <summary>
    ///     Manages a general store where the player can buy food, clothes, bullets, and parts for their vehicle.
    /// </summary>
    public sealed class StoreMode : GameMode, IStore
    {
        private readonly string _storeName;
        private List<IItem> _inventory;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailEntities.GameMode" /> class.
        /// </summary>
        public StoreMode(IGameServer game) : base(game)
        {
            _storeName = "Unknown General Store";
            _inventory = new List<IItem>();
            StoreBalance = (uint) game.Random.Next(100, 800);
        }

        public override ModeType Mode
        {
            get { return ModeType.Store; }
        }

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
            if (Game.Vehicle.Balance >= playerCost)
            {
                // Store earns the money from vehicle.
                StoreBalance += playerCost;
                Game.Vehicle.BuyItem(item);
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
    }
}