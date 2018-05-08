// Created by Ron 'Maxwolf' McDowell (ron.mcdowell@gmail.com) 
// Timestamp 01/03/2016@1:50 AM

using System;
using System.Text;
using WolfCurses.Window;
using WolfCurses.Window.Form;
using WolfCurses.Window.Form.Input;

namespace OregonTrailDotNet.Window.Travel.Store.Help
{
    /// <summary>
    ///     If the player cannot afford to leave the store because they have attempted to purchase more items than they are
    ///     capable of carrying and or purchasing this will be displayed to inform the user they need to pay up.
    /// </summary>
    [ParentWindow(typeof(Travel))]
    public sealed class StoreDebtWarning : InputForm<TravelInfo>
    {
        /// <summary>
        ///     The store debt.
        /// </summary>
        private readonly StringBuilder _storeDebt;

        /// <summary>
        ///     Initializes a new instance of the <see cref="StoreDebtWarning" /> class.
        ///     This constructor will be used by the other one
        /// </summary>
        /// <param name="window">The window.</param>
        // ReSharper disable once UnusedMember.Global
        public StoreDebtWarning(IWindow window) : base(window)
        {
            _storeDebt = new StringBuilder();
        }

        /// <summary>
        ///     Fired when dialog prompt is attached to active game Windows and would like to have a string returned.
        /// </summary>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        protected override string OnDialogPrompt()
        {
            _storeDebt.Clear();
            _storeDebt.AppendLine($"{Environment.NewLine}Whoa there partner!");
            _storeDebt.AppendLine(
                $"I see you got {UserData.Store.Transactions.Count} items worth {UserData.Store.TotalTransactionCost:C2}.");
            _storeDebt.AppendLine($"You only got {GameSimulationApp.Instance.Vehicle.Balance:C2}!");
            _storeDebt.AppendLine($"Put some items back in order to leave the store...{Environment.NewLine}");
            return _storeDebt.ToString();
        }

        /// <summary>
        ///     Fired when the dialog receives favorable input and determines a response based on this. From this method it is
        ///     common to attach another state, or remove the current state based on the response.
        /// </summary>
        /// <param name="reponse">The response the dialog parsed from simulation input buffer.</param>
        protected override void OnDialogResponse(DialogResponse reponse)
        {
            SetForm(typeof(Store));
        }
    }
}