// Created by Ron 'Maxwolf' McDowell (ron.mcdowell@gmail.com) 
// Timestamp 01/03/2016@1:50 AM

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OregonTrailDotNet.Entity;
using OregonTrailDotNet.Entity.Item;
using OregonTrailDotNet.Entity.Location;
using OregonTrailDotNet.Window.Travel.Dialog;
using OregonTrailDotNet.Window.Travel.Store.Help;
using WolfCurses.Utility;
using WolfCurses.Window;
using WolfCurses.Window.Form;

namespace OregonTrailDotNet.Window.Travel.Store
{
    /// <summary>
    ///     Manages a general store where the player can buy food, clothes, bullets, and parts for their vehicle.
    /// </summary>
    [ParentWindow(typeof(Travel))]
    public sealed class Store : Form<TravelInfo>
    {
        /// <summary>
        ///     String builder that will hold all the generated data about store inventory and selections for player to make.
        /// </summary>
        private StringBuilder _storePrompt;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Store" /> class.
        ///     This constructor will be used by the other one
        /// </summary>
        /// <param name="window">The window.</param>
        public Store(IWindow window) : base(window)
        {
        }

        /// <summary>
        ///     Fired after the state has been completely attached to the simulation letting the state know it can browse the user
        ///     data and other properties below it.
        /// </summary>
        public override void OnFormPostCreate()
        {
            base.OnFormPostCreate();

            // Will hold representation of this store for rendering.
            _storePrompt = new StringBuilder();

            // Builds up the store in the string builder we created above for rendering.
            UpdateStore();
        }

        /// <summary>
        ///     Offers chance to purchase a special vehicle part that is also an animal that eats grass and can die if it starves.
        /// </summary>
        private void BuyOxen()
        {
            UserData.Store.SelectedItem = Parts.Oxen;
            SetForm(typeof(StorePurchase));
        }

        /// <summary>
        ///     Offers the chance to buy some food for the players to eat everyday.
        /// </summary>
        private void BuyFood()
        {
            UserData.Store.SelectedItem = Resources.Food;
            SetForm(typeof(StorePurchase));
        }

        /// <summary>
        ///     Offers chance to buy some clothing to protect the players party in harsh climates.
        /// </summary>
        private void BuyClothing()
        {
            UserData.Store.SelectedItem = Resources.Clothing;
            SetForm(typeof(StorePurchase));
        }

        /// <summary>
        ///     Offers chance to buy bullets for hunting animals and killing them for food.
        /// </summary>
        private void BuyAmmunition()
        {
            UserData.Store.SelectedItem = Resources.Bullets;
            SetForm(typeof(StorePurchase));
        }

        /// <summary>
        ///     Offers a chance to purchase some spare wheels for the vehicle.
        /// </summary>
        private void BuySpareWheels()
        {
            UserData.Store.SelectedItem = Parts.Wheel;
            SetForm(typeof(StorePurchase));
        }

        /// <summary>
        ///     Offers a chance to purchase some spare axles for the vehicle.
        /// </summary>
        private void BuySpareAxles()
        {
            UserData.Store.SelectedItem = Parts.Axle;
            SetForm(typeof(StorePurchase));
        }

        /// <summary>
        ///     Offers a chance to purchase some spare vehicle tongues.
        /// </summary>
        private void BuySpareTongues()
        {
            UserData.Store.SelectedItem = Parts.Tongue;
            SetForm(typeof(StorePurchase));
        }

        /// <summary>
        ///     Returns a text only representation of the current game Windows state. Could be a statement, information, question
        ///     waiting input, etc.
        /// </summary>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        public override string OnRenderForm()
        {
            return _storePrompt.ToString();
        }

        /// <summary>
        ///     Creates store from enumeration of simulation entities and ignoring the ones the player cannot purchase like
        ///     vehicle, people, and cash itself.
        /// </summary>
        private void UpdateStore()
        {
            // Clear previous prompt and rebuild it.
            _storePrompt.Clear();
            _storePrompt.AppendLine("--------------------------------");
            _storePrompt.AppendLine($"{GameSimulationApp.Instance.Trail.CurrentLocation?.Name} General Store");
            _storePrompt.AppendLine($"{GameSimulationApp.Instance.Time.Date}");
            _storePrompt.AppendLine("--------------------------------");

            // Loop through all the store assets commands and print them out for the state.
            var storeAssets = new List<Entities>(Enum.GetValues(typeof(Entities)).Cast<Entities>());
            for (var index = 0; index < storeAssets.Count; index++)
            {
                // Get the current entity enumeration value we casted into list.
                var storeItem = storeAssets[index];

                // Skip if store item is cash, person, or vehicle.
                if ((storeItem == Entities.Cash) ||
                    (storeItem == Entities.Person) ||
                    (storeItem == Entities.Vehicle) ||
                    (storeItem == Entities.Location))
                    continue;

                // Creates a store price tag that shows the user how much the item is and or how much the store has.
                var storeTag = storeItem.ToDescriptionAttribute()
                    .Replace("@AMT@",
                        UserData.Store.Transactions[storeItem].ToString(
                            GameSimulationApp.Instance.Trail.IsFirstLocation &&
                            (GameSimulationApp.Instance.Trail.CurrentLocation?.Status == LocationStatus.Unreached)));

                // Last line should not print new line.
                if (index == storeAssets.Count - 5)
                {
                    _storePrompt.AppendLine($"  {(int) storeItem}. {storeTag}");
                    _storePrompt.AppendLine($"  {storeAssets.Count - 3}. Leave store");
                }
                else
                {
                    _storePrompt.AppendLine($"  {(int) storeItem}. {storeTag}");
                }
            }

            // Footer text for below menu.
            _storePrompt.AppendLine("--------------------------------");

            // Calculate how much monies the player has and the total amount of monies owed to store for pending transaction receipt.
            var totalBill = UserData.Store.TotalTransactionCost;
            var amountPlayerHas = GameSimulationApp.Instance.Vehicle.Balance - totalBill;

            // If at first location we show the total cost of the bill so far the player has racked up.
            _storePrompt.Append(GameSimulationApp.Instance.Trail.IsFirstLocation &&
                                (GameSimulationApp.Instance.Trail.CurrentLocation?.Status == LocationStatus.Unreached)
                ? $"Total bill:            {totalBill:C2}" +
                  $"{Environment.NewLine}Amount you have:       {amountPlayerHas:C2}"
                : $"You have {GameSimulationApp.Instance.Vehicle.Balance:C2} to spend.");
        }

        /// <summary>Fired when the game Windows current state is not null and input buffer does not match any known command.</summary>
        /// <param name="input">Contents of the input buffer which didn't match any known command in parent game Windows.</param>
        public override void OnInputBufferReturned(string input)
        {
            // Skip if the input is null or empty.
            if (string.IsNullOrEmpty(input) || string.IsNullOrWhiteSpace(input))
                return;

            // Attempt to cast string to enum value, can be characters or integer.
            Enum.TryParse(input, out Entities selectedItem);

            // Figure out what to do based on selection.
            switch (selectedItem)
            {
                case Entities.Animal:
                    BuyOxen();
                    break;
                case Entities.Food:
                    BuyFood();
                    break;
                case Entities.Clothes:
                    BuyClothing();
                    break;
                case Entities.Ammo:
                    BuyAmmunition();
                    break;
                case Entities.Wheel:
                    BuySpareWheels();
                    break;
                case Entities.Axle:
                    BuySpareAxles();
                    break;
                case Entities.Tongue:
                    BuySpareTongues();
                    break;
                case Entities.Vehicle:
                case Entities.Person:
                case Entities.Cash:
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
            // Complain if user doesn't have enough animals to pull their vehicle.
            if (UserData.Store.MissingImportantItems)
            {
                UserData.Store.SelectedItem = Parts.Oxen;
                SetForm(typeof(RequiredItem));
                return;
            }

            // Check if player can afford the items they have selected.
            var totalBill = UserData.Store.TotalTransactionCost;
            if (GameSimulationApp.Instance.Vehicle.Balance < totalBill)
            {
                SetForm(typeof(StoreDebtWarning));
                return;
            }

            // Travel Windows waits until it is by itself on first location and first turn.
            if (GameSimulationApp.Instance.Trail.IsFirstLocation &&
                (GameSimulationApp.Instance.Trail.CurrentLocation?.Status == LocationStatus.Unreached))
            {
                // First location and store prompt buys items when you leave the store.
                UserData.Store.PurchaseItems();

                // Sets up vehicle, location, and all other needed variables for simulation.
                GameSimulationApp.Instance.Trail.ArriveAtNextLocation();

                // Attach state that will ask if we want to check status or keep driving on trail.
                SetForm(typeof(LocationArrive));
            }
            else
            {
                // Normal store operation just returns to travel Windows menu.
                ClearForm();
            }
        }
    }
}