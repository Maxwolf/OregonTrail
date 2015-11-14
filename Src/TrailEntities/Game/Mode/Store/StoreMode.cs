using System;
using System.Text;
using TrailEntities.Entity;
using TrailEntities.Simulation;

namespace TrailEntities.Game
{
    /// <summary>
    ///     Manages a general store where the player can buy food, clothes, bullets, and parts for their vehicle.
    /// </summary>
    public sealed class StoreMode : ModeProduct<StoreCommands>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailEntities.Game.StoreMode" /> class.
        /// </summary>
        public StoreMode() : base(false)
        {
            // User data for states, keeps track of all new game information.
            StoreInfo = new StoreInfo();

            // Print out store good and their prices for user selection.
            UpdateDebts();

            // Trigger the store advice automatically on the first location, deeper check is making sure we are in new game mode also (travel mode always there).
            if (GameSimulationApp.Instance.Trail.IsFirstLocation() &&
                GameSimulationApp.Instance.WindowManagerMod.ModeCount > 1)
            {
                StoreAdvice();
            }
        }

        /// <summary>
        ///     Defines the text prefix which will go above the menu, used to show any useful information the game mode might need
        ///     to at the top of menu selections.
        /// </summary>
        protected override string MenuHeader { get; set; }

        /// <summary>
        ///     Defines the current game mode the inheriting class is going to take responsibility for when attached to the
        ///     simulation.
        /// </summary>
        public override GameMode GameMode
        {
            get { return GameMode.Store; }
        }

        /// <summary>
        ///     Holds all of the pending transactions the player would like to make with the store.
        /// </summary>
        private StoreInfo StoreInfo { get; }

        /// <summary>
        ///     Offers chance to purchase a special vehicle part that is also an animal that eats grass and can die if it starves.
        /// </summary>
        private void BuyOxen()
        {
            //CurrentState = new BuyItemState(Parts.Oxen, this, StoreInfo);
            SetState(typeof (BuyItemState));
        }

        /// <summary>
        ///     Offers the chance to buy some food for the players to eat everyday.
        /// </summary>
        private void BuyFood()
        {
            //CurrentState = new BuyItemState(Resources.Food, this, StoreInfo);
            SetState(typeof (BuyItemState));
        }

        /// <summary>
        ///     Offers chance to buy some clothing to protect the players party in harsh climates.
        /// </summary>
        private void BuyClothing()
        {
            //CurrentState = new BuyItemState(Resources.Clothing, this, StoreInfo);
            SetState(typeof (BuyItemState));
        }

        /// <summary>
        ///     Offers chance to buy bullets for hunting animals and killing them for food.
        /// </summary>
        private void BuyAmmunition()
        {
            //CurrentState = new BuyItemState(Resources.Bullets, this, StoreInfo);
            SetState(typeof (BuyItemState));
        }

        /// <summary>
        ///     Offers a chance to purchase some spare wheels for the vehicle.
        /// </summary>
        private void BuySpareWheels()
        {
            //CurrentState = new BuyItemState(Parts.Wheel, this, StoreInfo);
            SetState(typeof (BuyItemState));
        }

        /// <summary>
        ///     Offers a chance to purchase some spare axles for the vehicle.
        /// </summary>
        private void BuySpareAxles()
        {
            //CurrentState = new BuyItemState(Parts.Axle, this, StoreInfo);
            SetState(typeof (BuyItemState));
        }

        /// <summary>
        ///     Offers a chance to purchase some spare vehicle tongues.
        /// </summary>
        private void BuySpareTongues()
        {
            //CurrentState = new BuyItemState(Parts.Tongue, this, StoreInfo);
            SetState(typeof (BuyItemState));
        }

        /// <summary>
        ///     Attaches a game mode state what will show the player some basic information about what the various items mean and
        ///     what their purpose is in the simulation.
        /// </summary>
        private void StoreAdvice()
        {
            //CurrentState = new StoreAdviceState(this, StoreInfo);
            SetState(typeof (StoreAdviceState));
        }

        /// <summary>
        ///     Detaches the store mode from the simulation and returns to the one previous.
        /// </summary>
        private void LeaveStore()
        {
            // Complain if the player does not have any oxen to pull their vehicle.
            if (GameSimulationApp.Instance.Trail.IsFirstLocation() &&
                StoreInfo.Transactions[SimEntity.Animal].Quantity <= 0)
            {
                StoreInfo.MissingItemEntity = Parts.Oxen;
                //CurrentState = new MissingItemState(this, StoreInfo);
                SetState(typeof (MissingItemState));
                return;
            }

            // Check if player can afford the items they have selected.
            if (GameSimulationApp.Instance.Vehicle.Balance < StoreInfo.GetTransactionTotalCost())
            {
                //CurrentState = new StoreDebtState(this, StoreInfo);
                SetState(typeof (StoreDebtState));
                return;
            }

            // Remove the store if we make this far!
            RemoveModeNextTick();
        }

        /// <summary>
        ///     Fired when this game mode is removed from the list of available and ticked modes in the simulation.
        /// </summary>
        protected override void OnModeRemoved(GameMode gameMode)
        {
            base.OnModeRemoved(gameMode);

            // Store is only going to process transactions on removal when it is the one up for removal.
            if (gameMode != GameMode.Store)
                return;

            // When detaching the store for first time we need to move the vehicle to the first spot on our virtual trail.
            if (GameSimulationApp.Instance.Trail.IsFirstLocation())
            {
                GameSimulationApp.Instance.Trail.ArriveAtNextLocation();
            }

            // Modify the vehicles cash from purchases they made.
            var totalBill = StoreInfo.GetTransactionTotalCost();
            var amountPlayerHas = GameSimulationApp.Instance.Vehicle.Balance - totalBill;
            StoreInfo.Transactions[SimEntity.Cash] =
                new SimItem(StoreInfo.Transactions[SimEntity.Cash],
                    (int) amountPlayerHas);

            // Process all of the pending transactions in the store receipt info object.
            foreach (var transaction in StoreInfo.Transactions)
            {
                GameSimulationApp.Instance.Vehicle.BuyItem(transaction.Value);
            }

            // Remove all the transactions now that we have processed them.
            StoreInfo.ClearTransactions();
        }

        /// <summary>
        ///     Build up representation of store inventory as text.
        /// </summary>
        private void UpdateDebts()
        {
            // Header text for above menu.
            var headerText = new StringBuilder();
            headerText.Append($"--------------------------------{Environment.NewLine}");
            headerText.Append($"{CurrentPoint?.Name} General Store{Environment.NewLine}");
            headerText.Append($"{GameSimulationApp.Instance.Time.Date}{Environment.NewLine}");
            headerText.Append("--------------------------------");
            MenuHeader = headerText.ToString();

            // Keep track if this is the first point of interest, it will alter how the store shows values.
            var isFirstPoint = GameSimulationApp.Instance.Trail.IsFirstLocation();

            // Clear all the commands store had, then re-populate the list with them again so we can change the titles dynamically.
            ClearCommands();

            // Animals
            AddCommand(BuyOxen, StoreCommands.BuyOxen,
                $"Oxen              {StoreInfo.Transactions[SimEntity.Animal].ToString(isFirstPoint)}");

            // Food
            AddCommand(BuyFood, StoreCommands.BuyFood,
                $"Food              {StoreInfo.Transactions[SimEntity.Food].ToString(isFirstPoint)}");

            // Clothes
            AddCommand(BuyClothing, StoreCommands.BuyClothing,
                $"Clothing          {StoreInfo.Transactions[SimEntity.Clothes].ToString(isFirstPoint)}");

            // Bullets
            AddCommand(BuyAmmunition, StoreCommands.BuyAmmunition,
                $"Ammunition        {StoreInfo.Transactions[SimEntity.Ammo].ToString(isFirstPoint)}");

            // Wheel
            AddCommand(BuySpareWheels, StoreCommands.BuySpareWheel,
                $"Vehicle wheels    {StoreInfo.Transactions[SimEntity.Wheel].ToString(isFirstPoint)}");

            // Axle
            AddCommand(BuySpareAxles, StoreCommands.BuySpareAxles,
                $"Vehicle axles     {StoreInfo.Transactions[SimEntity.Axle].ToString(isFirstPoint)}");

            // Tongue
            AddCommand(BuySpareTongues, StoreCommands.BuySpareTongues,
                $"Vehicle tongues   {StoreInfo.Transactions[SimEntity.Tongue].ToString(isFirstPoint)}");

            // Exit store
            AddCommand(LeaveStore, StoreCommands.LeaveStore, "Leave store");

            // Footer text for below menu.
            var footerText = new StringBuilder();
            footerText.Append($"{Environment.NewLine}--------------------------------{Environment.NewLine}");

            // Calculate how much monies the player has and the total amount of monies owed to store for pending transaction receipt.
            var totalBill = StoreInfo.GetTransactionTotalCost();
            var amountPlayerHas = GameSimulationApp.Instance.Vehicle.Balance - totalBill;

            // If at first location we show the total cost of the bill so far the player has racked up.
            footerText.Append(isFirstPoint
                ? $"Total bill:            {totalBill.ToString("C2")}" +
                  $"{Environment.NewLine}Amount you have:       {amountPlayerHas.ToString("C2")}"
                : $"You have {GameSimulationApp.Instance.Vehicle.Balance.ToString("C2")} to spend.");
            MenuFooter = footerText.ToString();
        }
    }
}