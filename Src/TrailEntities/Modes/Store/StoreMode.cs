using System;
using TrailCommon;

namespace TrailEntities
{
    /// <summary>
    ///     Manages a general store where the player can buy food, clothes, bullets, and parts for their vehicle.
    /// </summary>
    public sealed class StoreMode : GameMode<StoreCommands>, IStoreMode
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailEntities.StoreMode" /> class.
        /// </summary>
        public StoreMode()
        {
            // Cast the current point of interest into a settlement object since that is where stores are.
            CurrentSettlement = GameSimulationApp.Instance.TrailSim.GetCurrentPointOfInterest() as SettlementPoint;
            if (CurrentSettlement == null)
                throw new InvalidCastException("Unable to cast current point of interest into a settlement point!");

            // Store name is the location and general store added to the end of that.
            StoreName = CurrentSettlement?.Name + " General Store";
        }

        public float StoreBalance { get; private set; }

        /// <summary>
        ///     Defines the current game mode the inheriting class is going to take responsibility for when attached to the
        ///     simulation.
        /// </summary>
        public override ModeType ModeType
        {
            get { return ModeType.Store; }
        }

        public PointOfInterest CurrentSettlement { get; }

        public string StoreName { get; }

        public void BuyItems(IItem item)
        {
            var playerCost = item.Cost*item.Quantity;
            if (GameSimulationApp.Instance.Vehicle.Balance < playerCost)
                return;

            // Store earns the money from vehicle.
            StoreBalance += playerCost;
            GameSimulationApp.Instance.Vehicle.BuyItem(item);
        }

        public override string GetTUI()
        {
            return "Welcome to " + StoreName;
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