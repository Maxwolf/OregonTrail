using System;
using System.Text;
using TrailEntities.Entity;
using TrailEntities.Simulation;

namespace TrailEntities.Mode
{
    /// <summary>
    ///     Allows the player to purchase a number of oxen to pull their vehicle.
    /// </summary>
    public sealed class BuyItemState : ModeStateProduct
    {
        /// <summary>
        ///     Help text to ask the player a question about how many of the particular SimItem they would like to purchase.
        /// </summary>
        private StringBuilder _itemBuyText;

        /// <summary>
        ///     Reference to the total amount of items the player can purchase of SimItem of this particular type from this store
        ///     with
        ///     the money they have.
        /// </summary>
        private int _purchaseLimit;

        /// <summary>
        ///     Reference to the SimItem the player wishes to purchase from the store, it will be added to receipt list of it can.
        /// </summary>
        private SimItem _simItemToBuy;

        /// <summary>
        ///     Attaches a state that will allow the player to purchase a certain number of a particular SimItem.
        /// </summary>
        /// <param name="simItemToBuy">SimItem to purchase.</param>
        /// <param name="gameMode">Current game mode that requested this.</param>
        /// <param name="userData">Any special user data associated with this state and mode.</param>
        public BuyItemState(SimItem simItemToBuy, ModeProduct gameMode, StoreInfo userData)
            : base(gameMode, userData)
        {
            // Figure out what we owe already from other store items, then how many of the SimItem we can afford.
            var _currentBalance =
                (int) (GameSimApp.Instance.Vehicle.Balance - userData.GetTransactionTotalCost());
            _purchaseLimit = (int) (_currentBalance/simItemToBuy.Cost);

            // Prevent negative numbers and set credit limit to zero if it drops below that.
            if (_purchaseLimit < 0)
                _purchaseLimit = 0;

            // Set the credit limit to be the carry limit if they player has lots of monies and can buy many, we must limit them!
            if (_purchaseLimit > simItemToBuy.MaxQuantity)
                _purchaseLimit = simItemToBuy.MaxQuantity;

            // Add some information about how many you can buy and total amount you can carry.
            _itemBuyText = new StringBuilder();

            // Change up question asked if plural form matches the name of the SimItem.
            var pluralMatchesName = simItemToBuy.PluralForm.Equals(simItemToBuy.Name,
                StringComparison.InvariantCultureIgnoreCase);

            _itemBuyText.Append(pluralMatchesName
                ? $"You can afford {_purchaseLimit} {simItemToBuy.Name.ToLowerInvariant()}.{Environment.NewLine}"
                : $"You can afford {_purchaseLimit} {simItemToBuy.PluralForm.ToLowerInvariant()} of {simItemToBuy.Name.ToLowerInvariant()}.{Environment.NewLine}");

            _itemBuyText.Append($"How many {simItemToBuy.PluralForm.ToLowerInvariant()} to buy?");

            // Set the SimItem to buy text.
            _simItemToBuy = simItemToBuy;
        }

        /// <summary>
        ///     Returns a text only representation of the current game mode state. Could be a statement, information, question
        ///     waiting input, etc.
        /// </summary>
        public override string OnRenderState()
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

            // If the number is zero remove the purchase state for this SimItem and back to store menu.
            if (parsedInputNumber <= 0)
            {
                UserData.RemoveItem(_simItemToBuy);
                ParentMode.RemoveState();
                return;
            }

            // Check that number is less than maximum quantity based on monies.
            if (parsedInputNumber > _purchaseLimit)
                return;

            // Check that number is less than or equal to limit that is hard-coded.
            if (parsedInputNumber > _simItemToBuy.MaxQuantity)
                return;

            // Add the SimItem the player wants in given amount 
            UserData.AddItem(_simItemToBuy, parsedInputNumber);

            // Return to the store menu.
            ParentMode.RemoveState();
        }
    }
}