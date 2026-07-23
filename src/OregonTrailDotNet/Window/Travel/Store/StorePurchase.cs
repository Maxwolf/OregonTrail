// Created by Maxwolf (bigmaxwolf.com) 
// Timestamp 01/03/2016@1:50 AM

using System;
using System.Text;
using OregonTrailDotNet.Entity.Item;
using OregonTrailDotNet.Entity.Location;
using OregonTrailDotNet.Window.Travel.Store.Help;
using WolfCurses.Window;
using WolfCurses.Window.Form;

namespace OregonTrailDotNet.Window.Travel.Store
{
    /// <summary>
    ///     Allows the player to purchase a number of oxen to pull their vehicle.
    /// </summary>
    [ParentWindow(typeof(Travel))]
    public sealed class StorePurchase : Form<TravelInfo>
    {
        /// <summary>
        ///     Help text to ask the player a question about how many of the particular SimItem they would like to purchase.
        /// </summary>
        private StringBuilder _itemBuyText;

        /// <summary>
        ///     Reference to the SimItem the player wishes to purchase from the store, it will be added to receipt list of
        ///     it can.
        /// </summary>
        private SimItem _itemToBuy;

        /// <summary>
        ///     Reference to the total amount of items the player can purchase of SimItem of this particular type from this
        ///     store
        ///     with
        ///     the money they have.
        /// </summary>
        private int _purchaseLimit;

        /// <summary>
        ///     Initializes a new instance of the <see cref="StorePurchase" /> class.
        ///     Attaches a state that will allow the player to purchase a certain number of a particular SimItem.
        /// </summary>
        /// <param name="window">Current game Windows that requested this.</param>
        // ReSharper disable once UnusedMember.Global
        public StorePurchase(IWindow window) : base(window)
        {
        }

        /// <summary>
        ///     Fired after the state has been completely attached to the simulation letting the state know it can browse the user
        ///     data and other properties below it.
        /// </summary>
        public override void OnFormPostCreate()
        {
            base.OnFormPostCreate();

            // Figure out what we owe already from other store items, then how many of the SimItem we can afford. A new
            // entry REPLACES any pending order for this same item (StoreGenerator.AddItem), so the item's own pending
            // cost is money this purchase frees up again — count it back in, or re-opening an item would quote against
            // its own reservation.
            var pendingSameItem = UserData.Store.Transactions[UserData.Store.SelectedItem.Category];
            var currentBalance =
                (int) (GameSimulationApp.Instance.Vehicle.Balance - UserData.Store.TotalTransactionCost +
                       pendingSameItem.TotalValue);
            _purchaseLimit = (int) (currentBalance/UserData.Store.SelectedItem.Cost);

            // Prevent negative numbers and set credit limit to zero if it drops below that.
            if (_purchaseLimit < 0)
                _purchaseLimit = 0;

            // Cap the quote at the space left in the wagon for this item: the ceiling minus what the party already
            // owns. The pending receipt entry is NOT subtracted — the new order replaces it. Quoting the full ceiling
            // would offer goods that the inventory clamp discards at checkout.
            var alreadyOwned = GameSimulationApp.Instance.Vehicle.Inventory[UserData.Store.SelectedItem.Category].Quantity;
            var remainingCapacity = UserData.Store.SelectedItem.MaxQuantity - alreadyOwned;
            if (remainingCapacity < 0)
                remainingCapacity = 0;
            if (_purchaseLimit > remainingCapacity)
                _purchaseLimit = remainingCapacity;

            // Items sold in a minimum lot (e.g. a 20-round box of ammunition) charge for the whole lot even when fewer are
            // requested, because the item enforces a minimum quantity that a purchase is silently clamped up to. If the
            // player cannot afford one full lot they can afford none of this item; otherwise the quote below would offer a
            // quantity that becomes an unaffordable purchase once clamped.
            if (_purchaseLimit < UserData.Store.SelectedItem.MinQuantity)
                _purchaseLimit = 0;

            // Add some information about how many you can buy and total amount you can carry.
            _itemBuyText = new StringBuilder();

            // Print text about purchasing the selected item, using natural singular/plural phrasing so vehicle parts
            // read as "3 wheels" rather than "3 wheels of vehicle wheel" while bulk goods stay "300 pounds of food".
            _itemBuyText.AppendLine(
                $"{Environment.NewLine}You can afford {UserData.Store.SelectedItem.ToQuantityString(_purchaseLimit)}.");

            // Wait for user input...
            _itemBuyText.Append($"How many {UserData.Store.SelectedItem.PluralForm.ToLowerInvariant()} to buy?");

            // Set the SimItem to buy text.
            _itemToBuy = UserData.Store.SelectedItem;
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
            ParentWindow.PromptText = "Enter a quantity:";
            return _itemBuyText.ToString();
        }

        /// <summary>Fired when the game Windows current state is not null and input buffer does not match any known command.</summary>
        /// <param name="input">Contents of the input buffer which didn't match any known command in parent game Windows.</param>
        public override void OnInputBufferReturned(string input)
        {
            // Parse the user input buffer as a unsigned int.
            if (!int.TryParse(input, out var parsedInputNumber))
                return;

            // If the number is zero remove the purchase state for this SimItem and back to store menu.
            if (parsedInputNumber <= 0)
            {
                UserData.Store.RemoveItem(_itemToBuy);
                UserData.Store.SelectedItem = null;
                SetForm(typeof(Store));
                return;
            }

            // Check that number is less than maximum quantity based on monies.
            if (parsedInputNumber > _purchaseLimit)
            {
                UserData.Store.RemoveItem(_itemToBuy);
                UserData.Store.SelectedItem = null;
                SetForm(typeof(Store));
                return;
            }

            // Check that number is less than or equal to limit that is hard-coded.
            if (parsedInputNumber > _itemToBuy.MaxQuantity)
            {
                UserData.Store.RemoveItem(_itemToBuy);
                UserData.Store.SelectedItem = null;
                SetForm(typeof(Store));
                return;
            }

            // Check that the player can actually afford this purchase. A purchase is charged for at least the item's
            // minimum lot (e.g. a 20-round box of ammunition), so validate against the quantity that will really be added
            // and against the balance left after any other items already on the pending receipt. The previous check used
            // _itemToBuy.TotalValue, which was always zero (SelectedItem's quantity is zero), so it never caught anything.
            var chargedQuantity = Math.Max(parsedInputNumber, _itemToBuy.MinQuantity);
            var balanceAfterPending = GameSimulationApp.Instance.Vehicle.Balance - UserData.Store.TotalTransactionCost;
            if (balanceAfterPending < _itemToBuy.Cost*chargedQuantity)
            {
                UserData.Store.RemoveItem(_itemToBuy);
                UserData.Store.SelectedItem = null;
                SetForm(typeof(Store));
                return;
            }

            // First location on the trail uses receipt to keep track of all the purchases player wants.
            UserData.Store.AddItem(_itemToBuy, parsedInputNumber);

            // If we are not on the first location we will add the item right away.
            if (GameSimulationApp.Instance.Trail.CurrentLocation?.Status == LocationStatusEnum.Arrived)
            {
                // Normal store operation while on the trail.
                UserData.Store.PurchaseItems();
            }
            else
            {
                // Check if player can afford the items they have selected.
                var totalBill = UserData.Store.TotalTransactionCost;
                if (GameSimulationApp.Instance.Vehicle.Balance < totalBill)
                {
                    SetForm(typeof(StoreDebtWarning));
                    return;
                }
            }

            // Clear the selection for the type of item the player was purchasing.
            UserData.Store.SelectedItem = null;

            // Return to the store menu.
            SetForm(typeof(Store));
        }
    }
}