using System;
using System.Text;
using TrailCommon;

namespace TrailEntities
{
    public sealed class ChooseNamesState : ModeState<NewGameInfo>
    {
        /// <summary>
        /// Holds the text that will be shown to the user when the TUI information is got.
        /// </summary>
        private string _questionName;

        /// <summary>
        ///     This constructor will be used by the other one
        /// </summary>
        public ChooseNamesState(IMode gameMode, NewGameInfo userData) : base(gameMode, userData)
        {

        }

        /// <summary>
        ///     Forces the current game mode state to update itself, this typically results in moving to the next state.
        /// </summary>
        public override void TickState()
        {
            // Nothing to see here, move along...
        }

        public override string GetStateTUI()
        {
            return _questionName;

            var playerName = GetPlayerName();
            UserData.PlayerNames.Add(playerName);
            nameSelect.Append("Added: " + playerName);

            playerName = GetPlayerName();
            UserData.PlayerNames.Add(playerName);
            nameSelect.Append("Added: " + playerName);

            playerName = GetPlayerName();
            UserData.PlayerNames.Add(playerName);
            nameSelect.Append("Added: " + playerName);

            playerName = GetPlayerName();
            UserData.PlayerNames.Add(playerName);
            nameSelect.Append("Added: " + playerName);


        }

        public override void ProcessInput(string input)
        {

        }


    }
}