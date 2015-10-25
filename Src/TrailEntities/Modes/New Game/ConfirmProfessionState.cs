using System.Text;
using TrailCommon;

namespace TrailEntities
{
    /// <summary>
    ///     Asks the user to confirm their selection for a given profession, if they select anything other than YES we will
    ///     restart the select profession state again.
    /// </summary>
    public sealed class ConfirmProfessionState : ModeState<NewGameInfo>
    {
        private StringBuilder confirmProfession;

        /// <summary>
        ///     This constructor will be used by the other one
        /// </summary>
        public ConfirmProfessionState(IMode gameMode, NewGameInfo userData) : base(gameMode, userData)
        {
            // Pass the game data to the simulation for each new game mode state.
            GameSimulationApp.Instance.StartGame(userData);

            confirmProfession = new StringBuilder();
            confirmProfession.Append(
                $"Selected profession {UserData.PlayerProfession} for party leader {UserData.PlayerNames[0]}.\n");
            confirmProfession.Append("Is this correct? Y/N\n");
        }

        /// <summary>
        ///     Determines if user input is currently allowed to be typed and filled into the input buffer.
        /// </summary>
        /// <remarks>Default is FALSE. Setting to TRUE allows characters and input buffer to be read when submitted.</remarks>
        public override bool AcceptsInput
        {
            get { return true; }
        }

        /// <summary>
        ///     Returns a text only representation of the current game mode state. Could be a statement, information, question
        ///     waiting input, etc.
        /// </summary>
        public override string GetStateTUI()
        {
            return confirmProfession.ToString();
        }

        /// <summary>
        ///     Fired when the game mode current state is not null and input buffer does not match any known command.
        /// </summary>
        /// <param name="input">Contents of the input buffer which didn't match any known command in parent game mode.</param>
        public override void OnInputBufferReturned(string input)
        {
            switch (input.ToUpperInvariant())
            {
                case "Y":
                    // User is happy with their profession, we will now add a store game mode so they can buy things with starting monies.
                    ParentMode.CurrentState = new BuyInitialItemsState(ParentMode, UserData);
                    break;
                default:
                    // User is not happy with profession choice and is going to reset the profession selector and try again.
                    ParentMode.CurrentState = new SelectProfessionState(ParentMode, UserData);
                    break;
            }
        }
    }
}