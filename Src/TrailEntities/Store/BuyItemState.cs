using System;
using System.Text;

namespace TrailEntities
{
    /// <summary>
    ///     Allows the player to purchase a number of oxen to pull their vehicle.
    /// </summary>
    public sealed class BuyItemState : ModeState<StoreInfo>
    {
        /// <summary>
        ///     Help text to ask the player a question about how many of the particular item they would like to purchase.
        /// </summary>
        private StringBuilder _itemBuyText;

        /// <summary>
        ///     Reference to the item the player wishes to purchase from the store, it will be added to receipt list of it can.
        /// </summary>
        private Item _itemToBuy;

        /// <summary>
        ///     Reference to the total amount of items the player can purchase of item of this particular type from this store with
        ///     the money they have.
        /// </summary>
        private int _purchaseLimit;

        public BuyItemState(Item itemToBuy, IMode gameMode, StoreInfo userData)
            : base(gameMode, userData)
        {
            // Figure out what we owe already from other store items, then how many of the item we can afford.
            var _currentBalance =
                (int) (GameSimulationApp.Instance.Vehicle.Balance - userData.GetTransactionTotalCost());
            _purchaseLimit = (int) (_currentBalance/itemToBuy.Cost);

            // Prevent negative numbers and set credit limit to zero if it drops below that.
            if (_purchaseLimit < 0)
                _purchaseLimit = 0;

            // Set the credit limit to be the carry limit if they player has lots of monies and can buy many, we must limit them!
            if (_purchaseLimit > itemToBuy.CarryLimit)
                _purchaseLimit = itemToBuy.CarryLimit;

            // Add some information about how many you can buy and total amount you can carry.
            _itemBuyText = new StringBuilder();

            // Change up question asked if plural form matches the name of the item.
            var pluralMatchesName = itemToBuy.PluralForm.Equals(itemToBuy.Name,
                StringComparison.InvariantCultureIgnoreCase);

            _itemBuyText.Append(pluralMatchesName
                ? $"You can afford {_purchaseLimit} {itemToBuy.Name.ToLowerInvariant()}.{Environment.NewLine}"
                : $"You can afford {_purchaseLimit} {itemToBuy.PluralForm.ToLowerInvariant()} of {itemToBuy.Name.ToLowerInvariant()}.{Environment.NewLine}");

            _itemBuyText.Append($"How many {itemToBuy.PluralForm.ToLowerInvariant()} to buy?");

            // Set the item to buy text.
            _itemToBuy = itemToBuy;
        }

        /// <summary>
        ///     Returns a text only representation of the current game mode state. Could be a statement, information, question
        ///     waiting input, etc.
        /// </summary>
        public override string GetStateTUI()
        {
            return _itemBuyText.ToString();
        }

        /// <summary>
        ///     Fired when the game mode current state is not null and input buffer does not match any known command.
        /// </summary>
        /// <param name="input">Contents of the input buffer which didn't match any known command in parent game mode.</param>
        public override void OnInputBufferReturned(string input)
        {
            // Parse the user input buffer as a unsigned int.
            int parsedInputNumber;
            if (!int.TryParse(input, out parsedInputNumber))
                return;

            // If the number is zero remove the purchase state for this item and back to store menu.
            if (parsedInputNumber <= 0)
            {
                UserData.RemoveItem(_itemToBuy);
                ParentMode.CurrentState = null;
                return;
            }

            // Check that number is less than maximum quantity based on monies.
            if (parsedInputNumber > _purchaseLimit)
                return;

            // Check that number is less than or equal to limit that is hard-coded.
            if (parsedInputNumber > _itemToBuy.CarryLimit)
                return;

            // Add the item the player wants in given amount 
            UserData.AddItem(_itemToBuy, parsedInputNumber);

            // Return to the store menu.
            ParentMode.CurrentState = null;
        }
    }
}