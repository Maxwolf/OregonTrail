// Created by Maxwolf (bigmaxwolf.com) 
// Timestamp 01/03/2016@1:50 AM

using System;
using System.Text;
using WolfCurses.Window;
using WolfCurses.Window.Form;
using WolfCurses.Window.Form.Input;

namespace OregonTrailDotNet.Window.Travel.Command
{
    /// <summary>
    ///     Shows all the players supplies that they currently have in their vehicle inventory, along with the amount of money
    ///     they have. This screen is not for looking at group stats, only items which are normally not shown unlike the travel
    ///     menu that shows basic party stats at all times.
    /// </summary>
    [ParentWindow(typeof(Travel))]
    public sealed class CheckSupplies : InputForm<TravelInfo>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="CheckSupplies" /> class.
        ///     This constructor will be used by the other one
        /// </summary>
        /// <param name="window">The window.</param>
        // ReSharper disable once UnusedMember.Global
        public CheckSupplies(IWindow window) : base(window)
        {
        }

        /// <summary>
        ///     Fired when dialog prompt is attached to active game Windows and would like to have a string returned.
        /// </summary>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        protected override string OnDialogPrompt()
        {
            // Tick the people, but not the trail or the day.
            GameSimulationApp.Instance.TakeTurn(true);

            // Show the shared supplies table (with the party's remaining money), under a heading.
            var supplies = new StringBuilder();
            supplies.AppendLine($"{Environment.NewLine}Your Supplies{Environment.NewLine}");
            supplies.AppendLine(SupplyPanel.Build(includeCash: true));

            return supplies.ToString();
        }

        /// <summary>
        ///     Fired when the dialog receives favorable input and determines a response based on this. From this method it is
        ///     common to attach another state, or remove the current state based on the response.
        /// </summary>
        /// <param name="reponse">The response the dialog parsed from simulation input buffer.</param>
        protected override void OnDialogResponse(DialogResponse reponse)
        {
            // parentGameMode.State = null;
            ClearForm();
        }
    }
}