using System;
using System.Text;
using TrailSimulation.Core;
using TrailSimulation.Entity;

namespace TrailSimulation.Game
{
    /// <summary>
    ///     Manages a general store where the player can buy food, clothes, bullets, and parts for their vehicle.
    /// </summary>
    [RequiredMode(Mode.Travel)]
    public sealed class StoreState : StateProduct<TravelInfo>
    {
        /// <summary>
        ///     This constructor will be used by the other one
        /// </summary>
        public StoreState(IModeProduct gameMode) : base(gameMode)
        {
            // Each instance of the store builds up a new instance of the class used to track purchases player would like to make.
            UserData.Store = new StoreReceipt();

            // Trigger the store advice automatically on the first location, deeper check is making sure we are in new game mode also (travel mode always there).
            if (GameSimulationApp.Instance.Trail.IsFirstLocation &&
                GameSimulationApp.Instance.ModeManager.ModeCount > 1)
            {
                StoreAdvice();
            }
        }

        /// <summary>
        ///     Offers chance to purchase a special vehicle part that is also an animal that eats grass and can die if it starves.
        /// </summary>
        private void BuyOxen()
        {
            UserData.Store.SelectedItem = Parts.Oxen;
            SetState(typeof (BuyItemState));
        }

        /// <summary>
        ///     Offers the chance to buy some food for the players to eat everyday.
        /// </summary>
        private void BuyFood()
        {
            UserData.Store.SelectedItem = Resources.Food;
            SetState(typeof (BuyItemState));
        }

        /// <summary>
        ///     Offers chance to buy some clothing to protect the players party in harsh climates.
        /// </summary>
        private void BuyClothing()
        {
            UserData.Store.SelectedItem = Resources.Clothing;
            SetState(typeof (BuyItemState));
        }

        /// <summary>
        ///     Offers chance to buy bullets for hunting animals and killing them for food.
        /// </summary>
        private void BuyAmmunition()
        {
            UserData.Store.SelectedItem = Resources.Bullets;
            SetState(typeof (BuyItemState));
        }

        /// <summary>
        ///     Offers a chance to purchase some spare wheels for the vehicle.
        /// </summary>
        private void BuySpareWheels()
        {
            UserData.Store.SelectedItem = Parts.Wheel;
            SetState(typeof (BuyItemState));
        }

        /// <summary>
        ///     Offers a chance to purchase some spare axles for the vehicle.
        /// </summary>
        private void BuySpareAxles()
        {
            UserData.Store.SelectedItem = Parts.Axle;
            SetState(typeof (BuyItemState));
        }

        /// <summary>
        ///     Offers a chance to purchase some spare vehicle tongues.
        /// </summary>
        private void BuySpareTongues()
        {
            UserData.Store.SelectedItem = Parts.Tongue;
            SetState(typeof (BuyItemState));
        }

        /// <summary>
        ///     Attaches a game mode state what will show the player some basic information about what the various items mean and
        ///     what their purpose is in the simulation.
        /// </summary>
        private void StoreAdvice()
        {
            SetState(typeof (StoreAdviceState));
        }

        /// <summary>
        ///     Detaches the store mode from the simulation and returns to the one previous.
        /// </summary>
        private void LeaveStore()
        {
            // Complain if the player does not have any oxen to pull their vehicle.
            if (GameSimulationApp.Instance.Trail.IsFirstLocation &&
                UserData.Store.Transactions[SimEntity.Animal].Quantity <= 0)
            {
                UserData.Store.SelectedItem = Parts.Oxen;
                SetState(typeof (MissingItemState));
                return;
            }

            // Check if player can afford the items they have selected.
            if (GameSimulationApp.Instance.Vehicle.Balance < UserData.Store.GetTransactionTotalCost)
            {
                SetState(typeof (StoreDebtState));
                return;
            }

            // Remove the store if we make this far!
            ClearState();
        }

        /// <summary>
        ///     Returns a text only representation of the current game mode state. Could be a statement, information, question
        ///     waiting input, etc.
        /// </summary>
        public override string OnRenderState()
        {
            // Skip if store has not been created yet.
            if (UserData.Store == null)
                return "Loading store...";

            // Header text for above menu.
            var headerText = new StringBuilder();
            headerText.AppendLine("--------------------------------");
            headerText.AppendLine($"{GameSimulationApp.Instance.Trail.CurrentLocation?.Name} General Store");
            headerText.AppendLine($"{GameSimulationApp.Instance.Time.Date}");
            headerText.Append("--------------------------------");

            // Keep track if this is the first point of interest, it will alter how the store shows values.
            var isFirstPoint = GameSimulationApp.Instance.Trail.IsFirstLocation;

            // Animals
            AddCommand(BuyOxen, StoreCommand.BuyOxen,
                $"Oxen              {UserData.Store.Transactions[SimEntity.Animal].ToString(isFirstPoint)}");

            // Food
            AddCommand(BuyFood, StoreCommand.BuyFood,
                $"Food              {UserData.Store.Transactions[SimEntity.Food].ToString(isFirstPoint)}");

            // Clothes
            AddCommand(BuyClothing, StoreCommand.BuyClothing,
                $"Clothing          {UserData.Store.Transactions[SimEntity.Clothes].ToString(isFirstPoint)}");

            // Bullets
            AddCommand(BuyAmmunition, StoreCommand.BuyAmmunition,
                $"Ammunition        {UserData.Store.Transactions[SimEntity.Ammo].ToString(isFirstPoint)}");

            // Wheel
            AddCommand(BuySpareWheels, StoreCommand.BuySpareWheel,
                $"Vehicle wheels    {UserData.Store.Transactions[SimEntity.Wheel].ToString(isFirstPoint)}");

            // Axle
            AddCommand(BuySpareAxles, StoreCommand.BuySpareAxles,
                $"Vehicle axles     {UserData.Store.Transactions[SimEntity.Axle].ToString(isFirstPoint)}");

            // Tongue
            AddCommand(BuySpareTongues, StoreCommand.BuySpareTongues,
                $"Vehicle tongues   {UserData.Store.Transactions[SimEntity.Tongue].ToString(isFirstPoint)}");

            // Exit store
            //AddCommand(LeaveStore, StoreCommands.LeaveStore, );

            // Footer text for below menu.
            headerText.Append($"{Environment.NewLine}--------------------------------{Environment.NewLine}");

            // Calculate how much monies the player has and the total amount of monies owed to store for pending transaction receipt.
            var totalBill = UserData.Store.GetTransactionTotalCost;
            var amountPlayerHas = GameSimulationApp.Instance.Vehicle.Balance - totalBill;

            // If at first location we show the total cost of the bill so far the player has racked up.
            headerText.Append(isFirstPoint
                ? $"Total bill:            {totalBill.ToString("C2")}" +
                  $"{Environment.NewLine}Amount you have:       {amountPlayerHas.ToString("C2")}"
                : $"You have {GameSimulationApp.Instance.Vehicle.Balance.ToString("C2")} to spend.");
            return headerText.ToString();
        }

        /// <summary>
        ///     Fired when the game mode current state is not null and input buffer does not match any known command.
        /// </summary>
        /// <param name="input">Contents of the input buffer which didn't match any known command in parent game mode.</param>
        public override void OnInputBufferReturned(string input)
        {
            // TODO: Store needs to keep existing items if not first turn and add to them.

            // Modify the vehicles cash from purchases they made.
            var totalBill = UserData.Store.GetTransactionTotalCost;
            var amountPlayerHas = GameSimulationApp.Instance.Vehicle.Balance - totalBill;
            UserData.Store.Transactions[SimEntity.Cash] =
                new SimItem(UserData.Store.Transactions[SimEntity.Cash],
                    (int)amountPlayerHas);

            // Process all of the pending transactions in the store receipt info object.
            foreach (var transaction in UserData.Store.Transactions)
            {
                GameSimulationApp.Instance.Vehicle.BuyItem(transaction.Value);
            }

            // Remove all the transactions now that we have processed them.
            UserData.Store.ClearTransactions();
            UserData.Store = null;

            // Travel mode waits until it is by itself on first location and first turn.
            if (GameSimulationApp.Instance.Trail.IsFirstLocation &&
                GameSimulationApp.Instance.ModeManager.ModeCount >= 3)
            {
                // Establishes configured vehicle onto running simulation, sets first point on trail as visited.
                // NOTE: Also calculates initial distance to next point!
                GameSimulationApp.Instance.Trail.ArriveAtNextLocation();
            }
        }
    }
}