using System;
using System.Linq;
using System.Text;

namespace TrailEntities
{
    /// <summary>
    ///     Manages a general store where the player can buy food, clothes, bullets, and parts for their vehicle.
    /// </summary>
    public sealed class StoreMode : GameMode<StoreCommands>
    {
        /// <summary>
        ///     Represents a string that represents a currency value of zero dollars and zero cents.
        /// </summary>
        private const string ZERO_MONIES = "$0.00";

        /// <summary>
        ///     Represents a string that represents an item that should be in the store but for some reason we could not find it.
        /// </summary>
        private const string ITEM_NOT_FOUND = "[ITEM NOT FOUND]";

        private string _axlesAmount;
        private string _bulletsAmount;
        private string _clothingAmount;
        private string _foodAmount;
        private string _oxenAmount;
        private string _tonguesAmount;
        private string _wheelsAmount;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailEntities.StoreMode" /> class.
        /// </summary>
        public StoreMode(bool showAdvice = false) : base(false)
        {
            // User data for states, keeps track of all new game information.
            StoreInfo = new StoreInfo(showAdvice);

            UpdateDebts();
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
        public override ModeType ModeType
        {
            get { return ModeType.Store; }
        }

        /// <summary>
        ///     Holds all of the pending transactions the player would like to make with the store.
        /// </summary>
        public StoreInfo StoreInfo { get; }

        /// <summary>
        ///     Offers chance to purchase a special vehicle part that is also an animal that eats grass and can die if it starves.
        /// </summary>
        public void BuyOxen()
        {
            CurrentState = new BuyItemState(CurrentPoint.StoreItems.First(item => item is Oxen), this, StoreInfo);
        }

        /// <summary>
        ///     Offers the chance to buy some food for the players to eat everyday.
        /// </summary>
        public void BuyFood()
        {
            CurrentState = new BuyItemState(CurrentPoint.StoreItems.First(item => item is Food), this, StoreInfo);
        }

        /// <summary>
        ///     Offers chance to buy some clothing to protect the players party in harsh climates.
        /// </summary>
        public void BuyClothing()
        {
            CurrentState = new BuyItemState(CurrentPoint.StoreItems.First(item => item is Clothing), this, StoreInfo);
        }

        /// <summary>
        ///     Offers chance to buy bullets for hunting animals and killing them for food.
        /// </summary>
        public void BuyAmmunition()
        {
            CurrentState = new BuyItemState(CurrentPoint.StoreItems.First(item => item is Bullets), this, StoreInfo);
        }

        /// <summary>
        ///     Offers a chance to purchase some spare wheels for the vehicle.
        /// </summary>
        public void BuySpareWheels()
        {
            CurrentState = new BuyItemState(CurrentPoint.StoreItems.First(item => item is PartWheel), this,
                StoreInfo);
        }

        /// <summary>
        ///     Offers a chance to purchase some spare axles for the vehicle.
        /// </summary>
        public void BuySpareAxles()
        {
            CurrentState = new BuyItemState(CurrentPoint.StoreItems.First(item => item is PartAxle), this, StoreInfo);
        }

        /// <summary>
        ///     Offers a chance to purchase some spare vehicle tongues.
        /// </summary>
        public void BuySpareTongues()
        {
            CurrentState = new BuyItemState(CurrentPoint.StoreItems.First(item => item is PartTongue), this,
                StoreInfo);
        }

        /// <summary>
        ///     Attaches a game mode state what will show the player some basic information about what the various items mean and
        ///     what their purpose is in the simulation.
        /// </summary>
        public void StoreAdvice()
        {
            CurrentState = new StoreAdviceState(this, StoreInfo);
        }

        /// <summary>
        ///     Detaches the store mode from the simulation and returns to the one previous.
        /// </summary>
        public void LeaveStore()
        {
            // First point of interest has a few extra checks before player allowed on the trail.
            if (GameSimulationApp.Instance.Trail.IsFirstPointOfInterest())
            {
                // Check if the player has any oxen to pull their vehicle.
                var boughtOxen = false;
                foreach (var pendingBuy in StoreInfo.Transactions)
                {
                    if (!(pendingBuy.Item is Oxen))
                        continue;

                    boughtOxen = true;
                    break;
                }

                // Complain if the player does not have any oxen to pull their vehicle.
                if (!boughtOxen)
                {
                    CurrentState = new MissingItemState(new Oxen(0), this, StoreInfo);
                    return;
                }
            }

            // Check if player can afford the items they have selected.
            if (GameSimulationApp.Instance.Vehicle.Balance < StoreInfo.GetTransactionTotalCost())
            {
                CurrentState = new StoreDebtState(this, StoreInfo);
                return;
            }

            // Remove the store if we make this far!
            RemoveModeNextTick();
        }

        /// <summary>
        ///     Removes item from the store and adds it to the players inventory.
        /// </summary>
        public void BuyItems(StoreTransactionItem transaction)
        {
            var playerCost = transaction.Item.Cost*transaction.Quantity;
            if (GameSimulationApp.Instance.Vehicle.Balance < playerCost)
                return;

            GameSimulationApp.Instance.Vehicle.BuyItem(transaction);
        }

        /// <summary>
        ///     Removes an item from the player, and adds it to the store inventory.
        /// </summary>
        public void SellItem(StoreTransactionItem transaction)
        {
            var storeCost = transaction.Item.Cost*transaction.Quantity;
            if (storeCost <= 0)
                return;

            BuyItems(transaction);
            GameSimulationApp.Instance.Vehicle.SellItem(transaction);
        }

        /// <summary>
        ///     Fired when the current game modes state is altered, it could be removed and null or a new one added up to
        ///     implementation to check.
        /// </summary>
        protected override void OnStateChanged()
        {
            base.OnStateChanged();

            // Skip if current state is not null.
            if (CurrentState != null)
                return;

            UpdateDebts();
        }

        /// <summary>
        ///     Fired when this game mode is removed from the list of available and ticked modes in the simulation.
        /// </summary>
        /// <param name="modeType"></param>
        protected override void OnModeRemoved(ModeType modeType)
        {
            base.OnModeRemoved(modeType);

            // Store is only going to process transactions on removal when it is the one up for removal.
            if (modeType != ModeType.Store)
                return;

            // First point of simulation when leaving store will setup the trail to be there.
            if (GameSimulationApp.Instance.Trail.IsFirstPointOfInterest())
            {
                GameSimulationApp.Instance.Trail.MoveTowardsNextPointOfInterest();
            }

            // Process all of the pending transactions in the store receipt info object.
            foreach (var transaction in StoreInfo.Transactions)
            {
                GameSimulationApp.Instance.Vehicle.BuyItem(transaction);
            }

            // Remove all the transactions now that we have processed them.
            StoreInfo.ClearTransactions();
        }

        /// <summary>
        ///     Build up representation of store inventory as text.
        /// </summary>
        private void UpdateDebts()
        {
            // We will only modify store visualization of prices when at the first location on the trail.
            var firstPoint = GameSimulationApp.Instance.Trail.IsFirstPointOfInterest();
            if (firstPoint)
            {
                // First store is slightly different and shows total monies against store transactions items instead.
                _oxenAmount = ZERO_MONIES;
                _foodAmount = ZERO_MONIES;
                _clothingAmount = ZERO_MONIES;
                _bulletsAmount = ZERO_MONIES;
                _wheelsAmount = ZERO_MONIES;
                _axlesAmount = ZERO_MONIES;
                _tonguesAmount = ZERO_MONIES;

                // Loop through every pending transaction and match it item type, the transaction will print out a nice cost of itself.
                foreach (var pendingBuy in StoreInfo.Transactions)
                {
                    if (pendingBuy.Item is Oxen)
                        _oxenAmount = pendingBuy.ToString();

                    if (pendingBuy.Item is Food)
                        _foodAmount = pendingBuy.ToString();

                    if (pendingBuy.Item is Clothing)
                        _clothingAmount = pendingBuy.ToString();

                    if (pendingBuy.Item is Bullets)
                        _bulletsAmount = pendingBuy.ToString();

                    if (pendingBuy.Item is PartWheel)
                        _wheelsAmount = pendingBuy.ToString();

                    if (pendingBuy.Item is PartAxle)
                        _axlesAmount = pendingBuy.ToString();

                    if (pendingBuy.Item is PartTongue)
                        _tonguesAmount = pendingBuy.ToString();
                }
            }
            else
            {
                // Default store print out looks for matching items the store sells.
                _oxenAmount = ITEM_NOT_FOUND;
                _foodAmount = ITEM_NOT_FOUND;
                _clothingAmount = ITEM_NOT_FOUND;
                _bulletsAmount = ITEM_NOT_FOUND;
                _wheelsAmount = ITEM_NOT_FOUND;
                _axlesAmount = ITEM_NOT_FOUND;
                _tonguesAmount = ITEM_NOT_FOUND;

                // Loop through every item the store sells and print out what it costs per unit.
                foreach (var storeItem in CurrentPoint.StoreItems)
                {
                    if (storeItem is Oxen)
                        _oxenAmount = storeItem.ToString();

                    if (storeItem is Food)
                        _foodAmount = storeItem.ToString();

                    if (storeItem is Clothing)
                        _clothingAmount = storeItem.ToString();

                    if (storeItem is Bullets)
                        _bulletsAmount = storeItem.ToString();

                    if (storeItem is PartWheel)
                        _wheelsAmount = storeItem.ToString();

                    if (storeItem is PartAxle)
                        _axlesAmount = storeItem.ToString();

                    if (storeItem is PartTongue)
                        _tonguesAmount = storeItem.ToString();
                }
            }

            // Header text for above menu.
            var headerText = new StringBuilder();
            headerText.Append($"--------------------------------{Environment.NewLine}");
            headerText.Append($"{CurrentPoint?.Name} General Store{Environment.NewLine}");
            headerText.Append($"{GameSimulationApp.Instance.Time.Date}{Environment.NewLine}");
            headerText.Append("--------------------------------");
            MenuHeader = headerText.ToString();

            // Clear all the commands store had, then re-populate the list with them again so we can change the titles dynamically.
            ClearCommands();
            AddCommand(BuyOxen, StoreCommands.BuyOxen, $"Oxen              {_oxenAmount}");
            AddCommand(BuyFood, StoreCommands.BuyFood, $"Food              {_foodAmount}");
            AddCommand(BuyClothing, StoreCommands.BuyClothing, $"Clothing          {_clothingAmount}");
            AddCommand(BuyAmmunition, StoreCommands.BuyAmmunition, $"Ammunition        {_bulletsAmount}");
            AddCommand(BuySpareWheels, StoreCommands.BuySpareWheel, $"Vehicle wheels    {_wheelsAmount}");
            AddCommand(BuySpareAxles, StoreCommands.BuySpareAxles, $"Vehicle axles     {_axlesAmount}");
            AddCommand(BuySpareTongues, StoreCommands.BuySpareTongues, $"Vehicle tongues   {_tonguesAmount}");
            AddCommand(LeaveStore, StoreCommands.LeaveStore, "Leave store");

            // Footer text for below menu.
            var footerText = new StringBuilder();
            footerText.Append($"{Environment.NewLine}--------------------------------{Environment.NewLine}");

            // Calculate how much monies the player has and the total amount of monies owed to store for pending transaction receipt.
            var totalBill = StoreInfo.GetTransactionTotalCost();
            var amountPlayerHas = GameSimulationApp.Instance.Vehicle.Balance - totalBill;

            // If at first location we show the total cost of the bill so far the player has racked up.
            footerText.Append(
                GameSimulationApp.Instance.Trail.IsFirstPointOfInterest()
                    ? $"Total bill:            {totalBill.ToString("C2")}" +
                      $"\nAmount you have:       {amountPlayerHas.ToString("C2")}\n\nWhich item would you like to buy?"
                    : $"You have {GameSimulationApp.Instance.Vehicle.Balance.ToString("C2")} to spend.\n\nWhich number?");
            MenuFooter = footerText.ToString();

            // Trigger the store advice automatically on the first location and one time only.
            if (GameSimulationApp.Instance.Trail.IsFirstPointOfInterest() && StoreInfo.ShowStoreAdvice)
            {
                StoreInfo.ShowStoreAdvice = false;
                StoreAdvice();
            }
        }
    }
}