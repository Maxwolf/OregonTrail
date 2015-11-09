using System;
using System.Text;
using TrailEntities.Mode;
using TrailEntities.Simulation;

namespace TrailEntities.Game.Store
{
    /// <summary>
    ///     If the player cannot afford to leave the store because they have attempted to purchase more items than they are
    ///     capable of carrying and or purchasing this will be displayed to inform the user they need to pay up.
    /// </summary>
    public sealed class StoreDebtState : ModeStateProduct
    {
        /// <summary>
        ///     Determines if we have already told the player about their debt.
        /// </summary>
        private bool _hasComplainedAboutDebt;

        /// <summary>
        ///     This constructor will be used by the other one
        /// </summary>
        public StoreDebtState(ModeProduct gameMode, StoreInfo userData) : base(gameMode, userData)
        {
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
        public override string OnRenderState()
        {
            var storeDebt = new StringBuilder();
            storeDebt.Append(
                $"Whoa there partner! I see you got {UserData.Transactions.Count()} items to buy that are worth {UserData.GetTransactionTotalCost().ToString("C2")}.{Environment.NewLine}");
            storeDebt.Append(
                $"You only got {GameSimulationApp.Instance.Vehicle.Balance.ToString("C2")}! Put some items back in order to leave the store...{Environment.NewLine}");

            storeDebt.Append(GameSimulationApp.PRESS_ENTER);
            return storeDebt.ToString();
        }

        /// <summary>
        ///     Fired when the game mode current state is not null and input buffer does not match any known command.
        /// </summary>
        /// <param name="input">Contents of the input buffer which didn't match any known command in parent game mode.</param>
        public override void OnInputBufferReturned(string input)
        {
            if (_hasComplainedAboutDebt)
                return;

            _hasComplainedAboutDebt = true;
            ParentMode.RemoveState();
        }
    }
}