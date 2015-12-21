// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfirmPlayerNames.cs" company="Ron 'Maxwolf' McDowell">
//   ron.mcdowell@gmail.com
// </copyright>
// <summary>
//   Prints out every entered player name in the user data for simulation initialization. Confirms with the player they
//   would indeed like to use all the entered names they have provided or had randomly generated for them by just
//   pressing enter.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Text;
using TrailSimulation.Core;

namespace TrailSimulation.Game
{
    /// <summary>
    ///     Prints out every entered player name in the user data for simulation initialization. Confirms with the player they
    ///     would indeed like to use all the entered names they have provided or had randomly generated for them by just
    ///     pressing enter.
    /// </summary>
    [ParentWindow(GameWindow.MainMenu)]
    public sealed class ConfirmPlayerNames : InputForm<NewGameInfo>
    {
        /// <summary>Initializes a new instance of the <see cref="ConfirmPlayerNames"/> class.
        ///     This constructor will be used by the other one</summary>
        /// <param name="window">The window.</param>
        public ConfirmPlayerNames(IWindow window) : base(window)
        {
        }

        /// <summary>
        ///     Defines what type of dialog this will act like depending on this enumeration value. Up to implementation to define
        ///     desired behavior.
        /// </summary>
        protected override DialogType DialogType
        {
            get { return DialogType.Custom; }
        }

        /// <summary>
        ///     Fired when dialog prompt is attached to active game Windows and would like to have a string returned.
        /// </summary>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        protected override string OnDialogPrompt()
        {
            // Pass the game data to the simulation for each new game Windows state.
            GameSimulationApp.Instance.SetStartInfo(UserData);

            // Create string builder, counter, print info about party members.
            var _confirmPartyText = new StringBuilder();
            _confirmPartyText.AppendLine(
                $"{Environment.NewLine}Are these names correct? Y/N{Environment.NewLine}");
            var crewNumber = 1;

            // Loop through every player and print their name.
            for (var index = 0; index < UserData.PlayerNames.Count; index++)
            {
                // First name in list is always the leader.
                var name = UserData.PlayerNames[index];
                var isLeader = UserData.PlayerNames.IndexOf(name) == 0 && crewNumber == 1;

                // Only append new line when not printing last line.
                if (index < (UserData.PlayerNames.Count - 1))
                {
                    _confirmPartyText.AppendLine(isLeader
                        ? $"  {crewNumber} - {name} (leader)"
                        : $"  {crewNumber} - {name}");
                }
                else
                {
                    _confirmPartyText.Append($"  {crewNumber} - {name}");
                }

                crewNumber++;
            }

            return _confirmPartyText.ToString();
        }

        /// <summary>Fired when the dialog receives favorable input and determines a response based on this. From this method it is
        ///     common to attach another state, or remove the current state based on the response.</summary>
        /// <param name="reponse">The response the dialog parsed from simulation input buffer.</param>
        protected override void OnDialogResponse(DialogResponse reponse)
        {
            switch (reponse)
            {
                case DialogResponse.No:
                    RestartNameInput();
                    break;
                case DialogResponse.Yes:


// Move along to confirming profession for party leader if user is happy with names.
                    UserData.PlayerNameIndex = 0;
                    SetForm(typeof (SelectStartingMonthState));
                    break;
                case DialogResponse.Custom:
                    RestartNameInput();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(reponse), reponse, null);
            }
        }

        /// <summary>
        ///     Restarts the player name selection.
        /// </summary>
        private void RestartNameInput()
        {
            UserData.PlayerNames.Clear();
            UserData.PlayerNameIndex = 0;
            SetForm(typeof (InputPlayerNames));
        }
    }
}