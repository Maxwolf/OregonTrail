using System;
using System.Text;

namespace TrailEntities
{
    /// <summary>
    ///     Informs the player they need to purchase at least a single one of the specified item in order to continue. This is
    ///     used in the new game mode to force the player to have at least one oxen to pull their vehicle in order to start the
    ///     simulation.
    /// </summary>
    public sealed class MissingItemState : ModeState<StoreInfo>
    {
        /// <summary>
        ///     Determines what item entity the player is actually missing.
        /// </summary>
        private readonly Item _missingItemEntity;

        /// <summary>
        ///     Determines if we have already told the player they need to purchase a particular item.
        /// </summary>
        private bool _informedAboutMissingItem;

        /// <summary>
        ///     This constructor will be used by the other one
        /// </summary>
        public MissingItemState(Item mustPurchaseEntity, IMode gameMode, StoreInfo userData) : base(gameMode, userData)
        {
            _missingItemEntity = mustPurchaseEntity;
        }

        /// <summary>
        ///     Determines if user input is currently allowed to be typed and filled into the input buffer.
        /// </summary>
        /// <remarks>Default is FALSE. Setting to TRUE allows characters and input buffer to be read when submitted.</remarks>
        public override bool AcceptsInput
        {
            get { return false; }
        }

        /// <summary>
        ///     Returns a text only representation of the current game mode state. Could be a statement, information, question
        ///     waiting input, etc.
        /// </summary>
        public override string GetStateTUI()
        {
            var missingItem = new StringBuilder();
            missingItem.Append(
                $"You need to purchase at least a single {_missingItemEntity.DelineatingUnit} in order to begin your trip!{Environment.NewLine}");

            missingItem.Append(GameSimulationApp.PRESS_ENTER);
            return missingItem.ToString();
        }

        /// <summary>
        ///     Fired when the game mode current state is not null and input buffer does not match any known command.
        /// </summary>
        /// <param name="input">Contents of the input buffer which didn't match any known command in parent game mode.</param>
        public override void OnInputBufferReturned(string input)
        {
            if (_informedAboutMissingItem)
                return;

            _informedAboutMissingItem = true;
            ParentMode.CurrentState = null;
        }
    }
}