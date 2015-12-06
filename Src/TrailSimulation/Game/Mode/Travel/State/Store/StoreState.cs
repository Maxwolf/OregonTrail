using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrailSimulation.Core;
using TrailSimulation.Entity;
using TrailSimulation.Utility;

namespace TrailSimulation.Game
{
    /// <summary>
    ///     Manages a general store where the player can buy food, clothes, bullets, and parts for their vehicle.
    /// </summary>
    [RequiredMode(Mode.Travel)]
    public sealed class StoreState : StateProduct<TravelInfo>
    {
        /// <summary>
        ///     String builder that will hold all the generated data about store inventory and selections for player to make.
        /// </summary>
        private StringBuilder _storePrompt;

        /// <summary>
        ///     This constructor will be used by the other one
        /// </summary>
        public StoreState(IModeProduct gameMode) : base(gameMode)
        {
        }

        /// <summary>
        ///     Fired after the state has been completely attached to the simulation letting the state know it can browse the user
        ///     data and other properties below it.
        /// </summary>
        public override void OnStatePostCreate()
        {
            base.OnStatePostCreate();

            // Will hold representation of this store for rendering.
            _storePrompt = new StringBuilder();

            // Builds up the store in the string builder we created above for rendering.
            UpdateStore();

            // Trigger the store advice automatically on the first location, deeper check is making sure we are in new game mode also (travel mode always there).
            if (GameSimulationApp.Instance.Trail.IsFirstLocation &&
                GameSimulationApp.Instance.ModeManager.ModeCount > 1 &&
                GameSimulationApp.Instance.Trail.CurrentLocation.Status == LocationStatus.Unreached)
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
        ///     Returns a text only representation of the current game mode state. Could be a statement, information, question
        ///     waiting input, etc.
        /// </summary>
        public override string OnRenderState()
        {
            return _storePrompt.ToString();
        }

        /// <summary>
        ///     Creates store from enumeration of simulation entities and ignoring the ones the player cannot purchase like
        ///     vehicle, people, and cash itself.
        /// </summary>
        private void UpdateStore()
        {
            // Skip if store has not been created yet.
            if (UserData.Store == null)
                return;

            // Clear previous prompt and rebuild it.
            _storePrompt.Clear();
            _storePrompt.AppendLine("--------------------------------");
            _storePrompt.AppendLine($"{GameSimulationApp.Instance.Trail.CurrentLocation?.Name} General Store");
            _storePrompt.AppendLine($"{GameSimulationApp.Instance.Time.Date}");
            _storePrompt.AppendLine("--------------------------------");

            // Loop through all the store assets commands and print them out for the state.
            var storeAssets = new List<SimEntity>(Enum.GetValues(typeof (SimEntity)).Cast<SimEntity>());
            for (var index = 0; index < storeAssets.Count; index++)
            {
                // Get the current entity enumeration value we casted into list.
                var storeItem = storeAssets[index];

                // Skip if store item is cash, person, or vehicle.
                if (storeItem == SimEntity.Cash ||
                    storeItem == SimEntity.Person ||
                    storeItem == SimEntity.Vehicle)
                    continue;

                // Creates a store price tag that shows the user how much the item is and or how much the store has.
                var storeTag = storeItem.ToDescriptionAttribute()
                    .Replace("@AMT@",
                        UserData.Store.Transactions[storeItem].ToString(
                            GameSimulationApp.Instance.Trail.IsFirstLocation &&
                            GameSimulationApp.Instance.Trail.CurrentLocation?.Status == LocationStatus.Unreached));

                // Last line should not print new line.
                if (index == (storeAssets.Count - 4))
                {
                    _storePrompt.AppendLine($"  {(int) storeItem}. {storeTag}");
                    _storePrompt.AppendLine($"  {storeAssets.Count - 2}. Leave store");
                }
                else
                {
                    _storePrompt.AppendLine($"  {(int) storeItem}. {storeTag}");
                }
            }

            // Footer text for below menu.
            _storePrompt.AppendLine("--------------------------------");

            // Calculate how much monies the player has and the total amount of monies owed to store for pending transaction receipt.
            var totalBill = UserData.Store.GetTransactionTotalCost;
            var amountPlayerHas = GameSimulationApp.Instance.Vehicle.Balance - totalBill;

            // If at first location we show the total cost of the bill so far the player has racked up.
            _storePrompt.Append(GameSimulationApp.Instance.Trail.IsFirstLocation &&
                GameSimulationApp.Instance.Trail.CurrentLocation?.Status == LocationStatus.Unreached
                ? $"Total bill:            {totalBill.ToString("C2")}" +
                  $"{Environment.NewLine}Amount you have:       {amountPlayerHas.ToString("C2")}"
                : $"You have {GameSimulationApp.Instance.Vehicle.Balance.ToString("C2")} to spend.");
        }

        /// <summary>
        ///     Fired when the game mode current state is not null and input buffer does not match any known command.
        /// </summary>
        /// <param name="input">Contents of the input buffer which didn't match any known command in parent game mode.</param>
        public override void OnInputBufferReturned(string input)
        {
            // Skip if the input is null or empty.
            if (string.IsNullOrEmpty(input) || string.IsNullOrWhiteSpace(input))
                return;

            // Attempt to cast string to enum value, can be characters or integer.
            SimEntity selectedItem;
            Enum.TryParse(input, out selectedItem);

            // Figure out what to do based on selection.
            switch (selectedItem)
            {
                case SimEntity.Animal:
                    BuyOxen();
                    break;
                case SimEntity.Food:
                    BuyFood();
                    break;
                case SimEntity.Clothes:
                    BuyClothing();
                    break;
                case SimEntity.Ammo:
                    BuyAmmunition();
                    break;
                case SimEntity.Wheel:
                    BuySpareWheels();
                    break;
                case SimEntity.Axle:
                    BuySpareAxles();
                    break;
                case SimEntity.Tongue:
                    BuySpareTongues();
                    break;
                case SimEntity.Vehicle:
                case SimEntity.Person:
                case SimEntity.Cash:
                    // The other options we just make them do the same as leaving store.
                    LeaveStore();
                    break;
                default:
                    LeaveStore();
                    break;
            }
        }

        /// <summary>
        ///     Attempts to leave the store state, if the player does not have enough oxen to pull the vehicle then it will
        ///     complain.
        /// </summary>
        private void LeaveStore()
        {
            // Complain if the player does not have any oxen to pull their vehicle.
            if (GameSimulationApp.Instance.Trail.IsFirstLocation &&
                GameSimulationApp.Instance.Trail.CurrentLocation?.Status == LocationStatus.Unreached &&
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

            // Modify the vehicles cash from purchases they made.
            var totalBill = UserData.Store.GetTransactionTotalCost;
            var amountPlayerHas = GameSimulationApp.Instance.Vehicle.Balance - totalBill;
            UserData.Store.Transactions[SimEntity.Cash] = new SimItem(UserData.Store.Transactions[SimEntity.Cash],
                (int) amountPlayerHas);

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
                GameSimulationApp.Instance.Trail.CurrentLocation?.Status == LocationStatus.Unreached)
            {
                // Sets up vehicle, location, and all other needed variables for simulation.
                GameSimulationApp.Instance.Trail.ArriveAtNextLocation();

                // Attach state that will ask if we want to check status or keep driving on trail.
                SetState(typeof (LookAroundState));
            }
            else
            {
                // Normal store operation just returns to travel mode menu.
                ClearState();
            }
        }
    }
}