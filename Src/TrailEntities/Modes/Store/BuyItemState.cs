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
        private StringBuilder _itemBuy;

        /// <summary>
        ///     Reference of the item the player wants to buy from the store.
        /// </summary>
        private Item _itemToBuy;

        public BuyItemState(string questionText, Item itemToBuy, IMode gameMode, StoreReceiptInfo userData)
            : base(gameMode, userData)
        {
            // Append the question text for purchasing the given type of item.
            _itemBuy = new StringBuilder();
            _itemBuy.Append(questionText + "\n");

            // Copy reference of the item the player wants to buy from the store.
            _itemToBuy = itemToBuy;
        }

        /// <summary>
        ///     Returns a text only representation of the current game mode state. Could be a statement, information, question
        ///     waiting input, etc.
        /// </summary>
        public override string GetStateTUI()
        {
            return _itemBuy.ToString();
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
        }
    }
}