using System;
using System.Text;
using TrailCommon;

namespace TrailEntities
{
    /// <summary>
    ///     Allows the player to purchase a number of oxen to pull their vehicle.
    /// </summary>
    public sealed class BuyItemState : ModeState<StoreReceiptInfo>
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
        private double _purchaseLimit;

        public BuyItemState(string questionText, Item itemToBuy, IMode gameMode, StoreReceiptInfo userData)
            : base(gameMode, userData)
        {
            // Figure out how many of the item in question the player can purchase with the money they have left.
            _purchaseLimit = Math.Round(GameSimulationApp.Instance.Vehicle.Balance/itemToBuy.Cost,
                MidpointRounding.ToEven);

            // Append the question text for purchasing the given type of item.
            _itemBuyText = new StringBuilder();
            _itemBuyText.Append($"You can afford {_purchaseLimit} {itemToBuy.Name.ToLowerInvariant()}\n");
            _itemBuyText.Append(questionText);

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
            uint parsedInputNumber;
            if (!uint.TryParse(input, out parsedInputNumber))
                return;

            // If the number is zero remove the purchase state for this item and back to store menu.
            if (parsedInputNumber <= 0)
            {
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