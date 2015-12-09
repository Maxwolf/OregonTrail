using System;
using System.Text;
using TrailSimulation.Core;

namespace TrailSimulation.Game
{
    /// <summary>
    ///     If the player cannot afford to leave the store because they have attempted to purchase more items than they are
    ///     capable of carrying and or purchasing this will be displayed to inform the user they need to pay up.
    /// </summary>
    [ParentWindow(GameWindow.Travel)]
    public sealed class StoreDebtWarning : InputForm<TravelInfo>
    {
        private StringBuilder storeDebt;

        /// <summary>
        ///     This constructor will be used by the other one
        /// </summary>
        public StoreDebtWarning(IWindow window) : base(window)
        {
            storeDebt = new StringBuilder();
        }

        /// <summary>
        ///     Fired when dialog prompt is attached to active game Windows and would like to have a string returned.
        /// </summary>
        protected override string OnDialogPrompt()
        {
            storeDebt.Clear();
            storeDebt.AppendLine($"{Environment.NewLine}Whoa there partner!");
            storeDebt.AppendLine(
                $"I see you got {UserData.Store.Transactions.Count} items worth {UserData.Store.TotalTransactionCost.ToString("C2")}.");
            storeDebt.AppendLine($"You only got {GameSimulationApp.Instance.Vehicle.Balance.ToString("C2")}!");
            storeDebt.AppendLine($"Put some items back in order to leave the store...{Environment.NewLine}");
            return storeDebt.ToString();
        }

        /// <summary>
        ///     Fired when the dialog receives favorable input and determines a response based on this. From this method it is
        ///     common to attach another state, or remove the current state based on the response.
        /// </summary>
        /// <param name="reponse">The response the dialog parsed from simulation input buffer.</param>
        protected override void OnDialogResponse(DialogResponse reponse)
        {
            SetForm(typeof (Store));
        }
    }
}