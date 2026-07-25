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
        ///     Matt's answer when the player asks for more than they can have, shown above the question until they
        ///     answer again. Null when the last answer was fine.
        /// </summary>
        private string _refusal;

        /// <summary>
        ///     How many units one answer buys at THIS counter. Captured when the screen opens so the arithmetic and the
        ///     order that follows cannot disagree about it.
        /// </summary>
        private int _lotSize = 1;

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
            //
            // Everything below counts in SALE LOTS, not single units: a yoke of two oxen, a box of twenty bullets, one
            // pound of food. How big a lot is depends on the counter rather than the goods — Matt's moves oxen by the
            // yoke, a fort sells them one at a time — so it is asked for once here and held for the whole screen. The
            // single conversion back to units happens when the order is placed, so the wagon and the scoring never see
            // a lot at all.
            var item = UserData.Store.SelectedItem;
            _lotSize = StoreCounter.LotSize(item);
            var lotCost = item.Cost*_lotSize;

            var pendingSameItem = UserData.Store.Transactions[item.Category];
            var currentBalance =
                GameSimulationApp.Instance.Vehicle.Balance - UserData.Store.TotalTransactionCost +
                pendingSameItem.TotalValue;
            _purchaseLimit = lotCost > 0 ? (int) (currentBalance/lotCost) : 0;

            // Prevent negative numbers and set credit limit to zero if it drops below that.
            if (_purchaseLimit < 0)
                _purchaseLimit = 0;

            // Cap the quote at the space left in the wagon for this item: the ceiling minus what the party already
            // owns, in whole lots. The pending receipt entry is NOT subtracted — the new order replaces it. Quoting the
            // full ceiling would offer goods that the inventory clamp discards at checkout.
            var alreadyOwned = GameSimulationApp.Instance.Vehicle.Inventory[item.Category].Quantity;
            var remainingCapacity = (item.MaxQuantity - alreadyOwned)/_lotSize;
            if (remainingCapacity < 0)
                remainingCapacity = 0;
            if (_purchaseLimit > remainingCapacity)
                _purchaseLimit = remainingCapacity;

            // The original's prompts were fixed-width character fields, so the field itself capped a single purchase:
            // one digit for oxen (nine yoke), two for ammunition (ninety-nine boxes). A player who wants more comes
            // back to the counter, which is exactly what they had to do in 1990.
            var fieldLimit = StoreCounter.MaxLots(item);
            if (_purchaseLimit > fieldLimit)
                _purchaseLimit = fieldLimit;

            // Add some information about how many you can buy and total amount you can carry.
            _itemBuyText = new StringBuilder();

            // Matt's own advice for this item, as the original gave it — how much a party this size needs, and what
            // it costs. Only at Independence: out on the trail the forts just quote a price.
            var advice = StoreAdvice.For(item);
            if (advice != null)
                _itemBuyText.AppendLine($"{Environment.NewLine}{advice}");

            // BOT CONTRACT: the literal "You can afford " prefix is scraped by the headless training bot
            // (ScreenRecognizer.AffordRx) to size its order. The number after it is now in SALE LOTS — boxes of
            // ammunition, yoke of oxen — so any change here needs the bot's StoreQuantity updated in tandem.
            _itemBuyText.AppendLine(
                $"{Environment.NewLine}You can afford {StoreCounter.ToLotString(item, _purchaseLimit)}.");

            // Wait for user input...
            _itemBuyText.Append($"How many {StoreCounter.LotPluralForm(item).ToLowerInvariant()} do you want?");

            // Set the SimItem to buy text.
            _itemToBuy = item;
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

            // A refusal sits above the question, and the running bill under it the way the original's counter did.
            var screen = new StringBuilder();
            if (_refusal != null)
                screen.AppendLine($"{Environment.NewLine}{_refusal}");

            screen.Append(_itemBuyText);

            var runningBill = StoreAdvice.RunningBill(UserData.Store);
            if (runningBill != null)
                screen.Append($"{Environment.NewLine}{Environment.NewLine}{runningBill}");

            return screen.ToString();
        }

        /// <summary>Fired when the game Windows current state is not null and input buffer does not match any known command.</summary>
        /// <param name="input">Contents of the input buffer which didn't match any known command in parent game Windows.</param>
        public override void OnInputBufferReturned(string input)
        {
            // Parse the user input buffer as a quantity of SALE LOTS. Anything that is not a number is simply not an
            // answer — re-ask rather than treating it as a cancellation.
            if (!int.TryParse(input, out var parsedLots))
                return;

            // Zero (or an outright negative) is the player backing out of this item. This is the ONLY path that
            // discards a pending order, because it is the only one where the player asked for that.
            if (parsedLots <= 0)
            {
                UserData.Store.RemoveItem(_itemToBuy);
                UserData.Store.SelectedItem = null;
                SetForm(typeof(Store));
                return;
            }

            // Too many. Say so and re-ask, keeping whatever was already on the receipt for this item: silently
            // bouncing back to the menu wiped a carefully chosen order and never told the player why.
            if (parsedLots > _purchaseLimit)
            {
                var field = StoreCounter.MaxLots(_itemToBuy);
                _refusal = parsedLots > field
                    ? $"I can only sell you {StoreCounter.ToLotString(_itemToBuy, field)} at a time."
                    : _purchaseLimit <= 0
                        ? "You cannot afford that."
                        : $"You cannot afford that many. I can do {StoreCounter.ToLotString(_itemToBuy, _purchaseLimit)}.";
                return;
            }

            // First location on the trail uses receipt to keep track of all the purchases player wants. This is the
            // one place lots become units — everything downstream counts single oxen, bullets and pounds.
            UserData.Store.AddItem(_itemToBuy, parsedLots*_lotSize);

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