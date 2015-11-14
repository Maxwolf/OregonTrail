using System;
using System.Linq;
using System.Text;
using TrailEntities.Simulation;

namespace TrailEntities.Game
{
    /// <summary>
    ///     If the player cannot afford to leave the store because they have attempted to purchase more items than they are
    ///     capable of carrying and or purchasing this will be displayed to inform the user they need to pay up.
    /// </summary>
    [RequiredMode(GameMode.Store)]
    public sealed class StoreDebtState : DialogState<StoreInfo>
    {
        /// <summary>
        ///     This constructor will be used by the other one
        /// </summary>
        public StoreDebtState(IModeProduct gameMode) : base(gameMode)
        {
        }

        /// <summary>
        ///     Fired when dialog prompt is attached to active game mode and would like to have a string returned.
        /// </summary>
        protected override string OnDialogPrompt()
        {
            var storeDebt = new StringBuilder();
            storeDebt.Append(
                $"Whoa there partner! I see you got {UserData.Transactions.Count()} items to buy that are worth {UserData.GetTransactionTotalCost().ToString("C2")}.{Environment.NewLine}");
            storeDebt.Append(
                $"You only got {GameSimulationApp.Instance.Vehicle.Balance.ToString("C2")}! Put some items back in order to leave the store...{Environment.NewLine}");
            return storeDebt.ToString();
        }

        /// <summary>
        ///     Fired when the dialog receives favorable input and determines a response based on this. From this method it is
        ///     common to attach another state, or remove the current state based on the response.
        /// </summary>
        /// <param name="reponse">The response the dialog parsed from simulation input buffer.</param>
        protected override void OnDialogResponse(DialogResponse reponse)
        {
            //parentGameMode.CurrentState = null;
            ClearState();
        }
    }
}