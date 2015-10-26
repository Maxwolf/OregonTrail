using System;
using System.Linq;
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
            // User data for states, keeps track of all new game information.
            StoreReceiptInfo = new StoreReceiptInfo();

            // Cast the current point of interest into a settlement object since that is where stores are.
            CurrentSettlement = GameSimulationApp.Instance.TrailSim.GetCurrentPointOfInterest() as SettlementPoint;
            if (CurrentSettlement == null)
                throw new InvalidCastException("Unable to cast current point of interest into a settlement point!");

            // Store name is the location and general store added to the end of that.
            StoreName = CurrentSettlement?.Name + " General Store";

            // Add all the commands a store has.
            AddCommand(BuyOxen, StoreCommands.BuyOxen, "Oxen");
            AddCommand(BuyFood, StoreCommands.BuyFood, "Food");
            AddCommand(BuyClothing, StoreCommands.BuyClothing, "Clothing");
            AddCommand(BuyAmmunition, StoreCommands.BuyAmmunition, "Ammunition");
            AddCommand(BuySpareWheels, StoreCommands.BuySpareWheel, "Vehicle wheels");
            AddCommand(BuySpareAxles, StoreCommands.BuySpareAxles, "Vehicle axles");
            AddCommand(BuySpareTongues, StoreCommands.BuySpareTongues, "Vehicle tongues");

            // If we are on the first part of the trail then we can show advice about store purchasing decisions.
            if (GameSimulationApp.Instance.TrailSim.VehicleLocation <= 0)
                AddCommand(StoreAdvice, StoreCommands.StoreAdvice, "Ask for advice");

            AddCommand(LeaveStore, StoreCommands.LeaveStore, "Leave store");
        }

        /// <summary>
        ///     Amount of money the store has to sell items to the player.
        /// </summary>
        public float StoreBalance { get; private set; }

        /// <summary>
        ///     Offers chance to purchase a special vehicle part that is also an animal that eats grass and can die if it starves.
        /// </summary>
        public void BuyOxen()
        {
            CurrentState = new BuyItemState("How many oxen?",
                CurrentSettlement.StoreItems.First(item => item is OxenItem), this, StoreReceiptInfo);
        }

        /// <summary>
        ///     Offers the chance to buy some food for the players to eat everyday.
        /// </summary>
        public void BuyFood()
        {
            CurrentState = new BuyItemState("How many pounds of food?",
                CurrentSettlement.StoreItems.First(item => item is FoodItem), this, StoreReceiptInfo);
        }

        /// <summary>
        ///     Offers chance to buy some clothing to protect the players party in harsh climates.
        /// </summary>
        public void BuyClothing()
        {
            CurrentState = new BuyItemState("How many clothing sets?",
                CurrentSettlement.StoreItems.First(item => item is ClothingItem), this, StoreReceiptInfo);
        }

        /// <summary>
        ///     Offers chance to buy bullets for hunting animals and killing them for food.
        /// </summary>
        public void BuyAmmunition()
        {
            CurrentState = new BuyItemState("How many ammo boxes?",
                CurrentSettlement.StoreItems.First(item => item is BulletsItem), this, StoreReceiptInfo);
        }

        /// <summary>
        ///     Offers a chance to purchase some spare wheels for the vehicle.
        /// </summary>
        public void BuySpareWheels()
        {
            CurrentState = new BuyItemState("How many spare wheels?",
                CurrentSettlement.StoreItems.First(item => item is PartWheelItem), this, StoreReceiptInfo);
        }

        /// <summary>
        ///     Offers a chance to purchase some spare axles for the vehicle.
        /// </summary>
        public void BuySpareAxles()
        {
            CurrentState = new BuyItemState("How many spare axles?",
                CurrentSettlement.StoreItems.First(item => item is PartAxleItem), this, StoreReceiptInfo);
        }

        /// <summary>
        ///     Offers a chance to purchase some spare vehicle tongues.
        /// </summary>
        public void BuySpareTongues()
        {
            CurrentState = new BuyItemState("How many spare tongues?",
                CurrentSettlement.StoreItems.First(item => item is PartTongueItem), this, StoreReceiptInfo);
        }

        /// <summary>
        ///     Attaches a game mode state what will show the player some basic information about what the various items mean and
        ///     what their purpose is in the simulation.
        /// </summary>
        public void StoreAdvice()
        {
            CurrentState = new StoreAdviceState(this, StoreReceiptInfo);
        }

        /// <summary>
        ///     Detaches the store mode from the simulation and returns to the one previous.
        /// </summary>
        public void LeaveStore()
        {
            RemoveModeNextTick();
        }

        /// <summary>
        ///     Defines the current game mode the inheriting class is going to take responsibility for when attached to the
        ///     simulation.
        /// </summary>
        public override ModeType ModeType
        {
            get { return ModeType.Store; }
        }

        /// <summary>
        ///     Holds all of the pending transactions the player would like to make with the store.
        /// </summary>
        public StoreReceiptInfo StoreReceiptInfo { get; }

        /// <summary>
        ///     Current point of interest the store is inside of which should be a settlement point since that is the lowest tier
        ///     class where they become available.
        /// </summary>
        public ISettlementPoint CurrentSettlement { get; }

        /// <summary>
        ///     Name of the store is typically the location name with 'general store' added to the end of the name.
        /// </summary>
        public string StoreName { get; }

        /// <summary>
        ///     Removes item from the store and adds it to the players inventory.
        /// </summary>
        /// <param name="item">Item that the player wants.</param>
        public void BuyItems(Item item)
        {
            var playerCost = item.Cost*item.Quantity;
            if (GameSimulationApp.Instance.Vehicle.Balance < playerCost)
                return;

            // Store earns the money from vehicle.
            StoreBalance += playerCost;
            GameSimulationApp.Instance.Vehicle.BuyItem(item);
        }

        /// <summary>
        ///     Removes an item from the player, and adds it to the store inventory.
        /// </summary>
        /// <param name="item">Item the player is going to give away in exchange for something else like money or more goods.</param>
        public void SellItem(Item item)
        {
            var storeCost = item.Cost*item.Quantity;
            if (StoreBalance < storeCost)
                return;

            StoreBalance -= storeCost;
            BuyItems(item);
        }
    }
}