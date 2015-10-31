using System.Linq;
using System.Text;
using TrailCommon;

namespace TrailEntities
{
    /// <summary>
    ///     Manages a general store where the player can buy food, clothes, bullets, and parts for their vehicle.
    /// </summary>
    public sealed class StoreMode : GameMode<StoreCommands>, IStoreMode
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
        public override string MenuHeader { get; protected set; }

        /// <summary>
        ///     Offers chance to purchase a special vehicle part that is also an animal that eats grass and can die if it starves.
        /// </summary>
        public void BuyOxen()
        {
            CurrentState = new BuyItemState("How many oxen?",
                CurrentPoint.StoreItems.First(item => item is OxenItem), this, StoreInfo);
        }

        /// <summary>
        ///     Offers the chance to buy some food for the players to eat everyday.
        /// </summary>
        public void BuyFood()
        {
            CurrentState = new BuyItemState("How many pounds of food?",
                CurrentPoint.StoreItems.First(item => item is FoodItem), this, StoreInfo);
        }

        /// <summary>
        ///     Offers chance to buy some clothing to protect the players party in harsh climates.
        /// </summary>
        public void BuyClothing()
        {
            CurrentState = new BuyItemState("How many clothing sets?",
                CurrentPoint.StoreItems.First(item => item is ClothingItem), this, StoreInfo);
        }

        /// <summary>
        ///     Offers chance to buy bullets for hunting animals and killing them for food.
        /// </summary>
        public void BuyAmmunition()
        {
            CurrentState = new BuyItemState("How many ammo boxes?",
                CurrentPoint.StoreItems.First(item => item is BulletsItem), this, StoreInfo);
        }

        /// <summary>
        ///     Offers a chance to purchase some spare wheels for the vehicle.
        /// </summary>
        public void BuySpareWheels()
        {
            CurrentState = new BuyItemState("How many spare wheels?",
                CurrentPoint.StoreItems.First(item => item is PartWheelItem), this, StoreInfo);
        }

        /// <summary>
        ///     Offers a chance to purchase some spare axles for the vehicle.
        /// </summary>
        public void BuySpareAxles()
        {
            CurrentState = new BuyItemState("How many spare axles?",
                CurrentPoint.StoreItems.First(item => item is PartAxleItem), this, StoreInfo);
        }

        /// <summary>
        ///     Offers a chance to purchase some spare vehicle tongues.
        /// </summary>
        public void BuySpareTongues()
        {
            CurrentState = new BuyItemState("How many spare tongues?",
                CurrentPoint.StoreItems.First(item => item is PartTongueItem), this, StoreInfo);
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
        public StoreInfo StoreInfo { get; }

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
        protected override void OnModeRemoved()
        {
            base.OnModeRemoved();

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
            // TODO: Remove LINQ calls and use a single loop instead.
            _oxenAmount = CurrentPoint.StoreItems.FirstOrDefault(t =>
                t is OxenItem)?.ToString() ?? ITEM_NOT_FOUND;

            _foodAmount = CurrentPoint.StoreItems.FirstOrDefault(t =>
                t is FoodItem)?.ToString() ?? ITEM_NOT_FOUND;

            _clothingAmount = CurrentPoint.StoreItems.FirstOrDefault(t =>
                t is ClothingItem)?.ToString() ?? ITEM_NOT_FOUND;

            _bulletsAmount = CurrentPoint.StoreItems.FirstOrDefault(t =>
                t is BulletsItem)?.ToString() ?? ITEM_NOT_FOUND;

            _wheelsAmount = CurrentPoint.StoreItems.FirstOrDefault(t =>
                t is PartWheelItem)?.ToString() ?? ITEM_NOT_FOUND;

            _axlesAmount = CurrentPoint.StoreItems.FirstOrDefault(t =>
                t is PartAxleItem)?.ToString() ?? ITEM_NOT_FOUND;

            _tonguesAmount = CurrentPoint.StoreItems.FirstOrDefault(t =>
                t is PartTongueItem)?.ToString() ?? ITEM_NOT_FOUND;

            // We will only modify store visualization of prices when at the first location on the trail.
            if (GameSimulationApp.Instance.TrailSim.VehicleLocation <= 0)
            {
                _oxenAmount = StoreInfo.Transactions.FirstOrDefault(t =>
                    t.Item is OxenItem)?.ToString() ?? ZERO_MONIES;

                _foodAmount = StoreInfo.Transactions.FirstOrDefault(t =>
                    t.Item is FoodItem)?.ToString() ?? ZERO_MONIES;

                _clothingAmount = StoreInfo.Transactions.FirstOrDefault(t =>
                    t.Item is ClothingItem)?.ToString() ?? ZERO_MONIES;

                _bulletsAmount = StoreInfo.Transactions.FirstOrDefault(t =>
                    t.Item is BulletsItem)?.ToString() ?? ZERO_MONIES;

                _wheelsAmount = StoreInfo.Transactions.FirstOrDefault(t =>
                    t.Item is PartWheelItem)?.ToString() ?? ZERO_MONIES;

                _axlesAmount = StoreInfo.Transactions.FirstOrDefault(t =>
                    t.Item is PartAxleItem)?.ToString() ?? ZERO_MONIES;

                _tonguesAmount = StoreInfo.Transactions.FirstOrDefault(t =>
                    t.Item is PartTongueItem)?.ToString() ?? ZERO_MONIES;
            }

            // Header text for above menu.
            var headerText = new StringBuilder();
            headerText.Append("--------------------------------\n");
            headerText.Append($"{CurrentPoint?.Name} General Store\n");
            headerText.Append($"{GameSimulationApp.Instance.Time.Date}\n");
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
            footerText.Append("\n--------------------------------\n");

            // Calculate how much monies the player has and the total amount of monies owed to store for pending transaction receipt.
            var totalBill = StoreInfo.GetTransactionTotalCost();
            var amountPlayerHas = GameSimulationApp.Instance.Vehicle.Balance - totalBill;

            // If at first location we show the total cost of the bill so far the player has racked up.
            footerText.Append(
                GameSimulationApp.Instance.TrailSim.VehicleLocation <= 0
                    ? $"Total bill:            {totalBill.ToString("C2")}" +
                      $"\nAmount you have:       {amountPlayerHas.ToString("C2")}\n\nWhich item would you like to buy?"
                    : $"You have {GameSimulationApp.Instance.Vehicle.Balance.ToString("C2")} to spend.\n\nWhich number?");
            MenuFooter = footerText.ToString();

            // Trigger the store advice automatically on the first location and one time only.
            if (GameSimulationApp.Instance.TrailSim.VehicleLocation <= 0 && StoreInfo.ShowStoreAdvice)
            {
                StoreInfo.ShowStoreAdvice = false;
                StoreAdvice();
            }
        }
    }
}