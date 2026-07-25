// Created by Maxwolf (bigmaxwolf.com) 
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
        ///     Offers the chance to buy medical supplies used to cure serious illness among the party.
        /// </summary>
        private void BuyMedicine()
        {
            UserData.Store.SelectedItem = Resources.Medicine;
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
        ///     TRUE while the party is still outfitting at Matt's in Independence, before the journey has begun. The
        ///     location index alone is not enough: the party passes back through Independence's index once it departs.
        /// </summary>
        private static bool AtOpeningStore =>
            GameSimulationApp.Instance.Trail.IsFirstLocation &&
            GameSimulationApp.Instance.Trail.CurrentLocation?.Status == LocationStatusEnum.Unreached;

        /// <summary>
        ///     Creates store from enumeration of simulation entities and ignoring the ones the player cannot purchase like
        ///     vehicle, people, and cash itself.
        /// </summary>
        private void UpdateStore()
        {
            // Clear previous prompt and rebuild it. The store name and date title a framed header panel above the menu.
            // The opening store is Matt's, by name, in Independence — the original titled it that and only that, and
            // it is the one shop in the game with a shopkeeper. The forts on the trail get their own name instead.
            _storePrompt.Clear();
            _storePrompt.AppendLine(AtOpeningStore
                ? FramedPanel.Render("Matt's General Store",
                    $"Independence, Missouri{Environment.NewLine}{GameSimulationApp.Instance.Time.Date}")
                : FramedPanel.Render($"{GameSimulationApp.Instance.Trail.CurrentLocation?.Name} General Store",
                    $"{GameSimulationApp.Instance.Time.Date}"));

            // Loop through all the store assets commands and print them out for the state.
            var storeAssets = new List<EntitiesEnum>(Enum.GetValues(typeof(EntitiesEnum)).Cast<EntitiesEnum>());
            for (var index = 0; index < storeAssets.Count; index++)
            {
                // Get the current entity enumeration value we casted into list.
                var storeItem = storeAssets[index];

                // Skip if store item is cash, person, or vehicle.
                if ((storeItem == EntitiesEnum.Cash) ||
                    (storeItem == EntitiesEnum.Person) ||
                    (storeItem == EntitiesEnum.Vehicle) ||
                    (storeItem == EntitiesEnum.Location))
                    continue;

                // Creates a store price tag that shows the user how much the item is and or how much the store has.
                var storeTag = storeItem.ToDescriptionAttribute()
                    .Replace("@AMT@",
                        UserData.Store.Transactions[storeItem].ToString(
                            GameSimulationApp.Instance.Trail.IsFirstLocation &&
                            (GameSimulationApp.Instance.Trail.CurrentLocation?.Status == LocationStatusEnum.Unreached)));

                _storePrompt.AppendLine($"  {(int) storeItem}. {storeTag}");

                // "Leave store" closes the list, under the last thing that is actually for sale. Keyed to the item and
                // to the exit's own enum value rather than counted backwards from the end of the enumeration: the old
                // `Count - 5` / `Count - 3` arithmetic silently mis-numbered the exit the moment anything was added to
                // EntitiesEnum, and the printed number IS what the bot's trained policies send.
                if (storeItem == EntitiesEnum.Medicine)
                    _storePrompt.AppendLine($"  {(int) EntitiesEnum.Vehicle}. Leave store");
            }

            // Footer text for below menu.
            _storePrompt.AppendLine("--------------------------------");

            // Calculate the total owed to the store for the pending receipt. "Amount you have" is the money in the
            // party's pocket, NOT what is left after the bill — the original printed the two side by side precisely so
            // the player could compare them ("Total bill: $120.00" against "Amount you have: $400.00"), and netting one
            // off the other threw that comparison away and made the running bill look like it cost nothing.
            var totalBill = UserData.Store.TotalTransactionCost;

            // If at first location we show the total cost of the bill so far the player has racked up.
            _storePrompt.Append(AtOpeningStore
                ? $"Total bill:            {totalBill:C2}" +
                  $"{Environment.NewLine}Amount you have:       {GameSimulationApp.Instance.Vehicle.Balance:C2}"
                : $"You have {GameSimulationApp.Instance.Vehicle.Balance:C2} to spend.");
        }

        /// <summary>Fired when the game Windows current state is not null and input buffer does not match any known command.</summary>
        /// <param name="input">Contents of the input buffer which didn't match any known command in parent game Windows.</param>
        public override void OnInputBufferReturned(string input)
        {
            // Skip if the input is null or empty.
            if (string.IsNullOrEmpty(input) || string.IsNullOrWhiteSpace(input))
                return;

            // Attempt to cast string to enum value, can be characters or integer. A fat-fingered answer must not be
            // mistaken for "leave": Enum.TryParse happily accepts any number in range of the underlying int (so "0",
            // "10" and "99" all parsed) as well as enum NAMES, and the old default arm walked every one of them out of
            // the store — which at Independence bought the receipt and departed for Oregon with no way back. An answer
            // that is not on the menu is not an answer; re-ask, exactly as the original's masked field did.
            if (!Enum.TryParse(input, out EntitiesEnum selectedItem) ||
                !Enum.IsDefined(typeof(EntitiesEnum), selectedItem))
                return;

            // Figure out what to do based on selection.
            switch (selectedItem)
            {
                case EntitiesEnum.Animal:
                    BuyOxen();
                    break;
                case EntitiesEnum.Food:
                    BuyFood();
                    break;
                case EntitiesEnum.Clothes:
                    BuyClothing();
                    break;
                case EntitiesEnum.Ammo:
                    BuyAmmunition();
                    break;
                case EntitiesEnum.Medicine:
                    BuyMedicine();
                    break;
                case EntitiesEnum.Wheel:
                    BuySpareWheels();
                    break;
                case EntitiesEnum.Axle:
                    BuySpareAxles();
                    break;
                case EntitiesEnum.Tongue:
                    BuySpareTongues();
                    break;
                // The one way out, and the number the printed menu shows against "Leave store" — which is also the
                // number the bot's trained policies send, so it is frozen.
                case EntitiesEnum.Vehicle:
                    LeaveStore();
                    break;

                // Everything else on the entity enumeration is not for sale and is not the exit either: Person, Cash
                // and Location are simulation bookkeeping that the menu never prints. Ignore and re-ask.
                default:
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
                (GameSimulationApp.Instance.Trail.CurrentLocation?.Status == LocationStatusEnum.Unreached))
            {
                // First location and store prompt buys items when you leave the store.
                UserData.Store.PurchaseItems();

                // Sets up vehicle, location, and all other needed variables for simulation.
                GameSimulationApp.Instance.Trail.ArriveAtNextLocation();

                // Attach state that will ask if we want to check status or keep driving on trail. This is the
                // "Going back to 1848" moment, and with presentation on it is the opening card (Independence art +
                // the opening tune) — arriving above may already have attached it via the window re-add, but this
                // explicit set must name the same form or it clobbers the card with the text prompt mid-tune.
                SetForm(Scene.OpeningCard.ShouldShow ? typeof(Scene.OpeningCard) : typeof(LocationArrive));
            }
            else
            {
                // Normal store operation just returns to travel Windows menu.
                ClearForm();
            }
        }
    }
}