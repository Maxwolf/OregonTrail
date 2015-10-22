using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using TrailCommon;

namespace TrailEntities
{
    /// <summary>
    ///     Manages a general store where the player can buy food, clothes, bullets, and parts for their vehicle.
    /// </summary>
    public sealed class StoreMode : GameMode<StoreCommands>, IStore
    {
        private readonly string _storeName;
        private List<IItem> _inventory;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailEntities.StoreMode" /> class.
        /// </summary>
        public StoreMode()
        {
            _storeName = "Unknown General Store";
            _inventory = new List<IItem>();
            StoreBalance = (uint) GameSimulationApp.Instance.Random.Next(100, 800);
        }

        /// <summary>
        ///     Fired by game simulation system timers timer which runs on same thread, only fired for active (last added), or
        ///     top-most game mode.
        /// </summary>
        public override void TickMode()
        {
            throw new NotImplementedException();
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

        /// <summary>
        ///     Called by the active game mode when the text user interface is called. This will create a string builder with all
        ///     the data and commands that represent the concrete handler for this game mode.
        /// </summary>
        protected override string OnGetModeTUI()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Fired when this game mode is removed from the list of available and ticked modes in the simulation.
        /// </summary>
        public override void OnModeRemoved()
        {
            throw new NotImplementedException();
        }
    }
}