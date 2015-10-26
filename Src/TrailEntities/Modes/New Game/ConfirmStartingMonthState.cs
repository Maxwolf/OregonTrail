using System.Text;
using TrailCommon;

namespace TrailEntities
{
    /// <summary>
    ///     Confirms the users selection for the starting month of the simulation. If they don't like the selection they will
    ///     be offered a chance to restart the selection process.
    /// </summary>
    public sealed class ConfirmStartingMonthState : ModeState<NewGameInfo>
    {
        private StringBuilder confirmStartMonth;

        /// <summary>
        ///     This constructor will be used by the other one
        /// </summary>
        public ConfirmStartingMonthState(IMode gameMode, NewGameInfo userData) : base(gameMode, userData)
        {
            // Pass the game data to the simulation for each new game mode state.
            GameSimulationApp.Instance.SetData(userData);

            confirmStartMonth = new StringBuilder();
            confirmStartMonth.Append(
                $"Selected starting month of {UserData.StartingMonth}.\n");
            confirmStartMonth.Append("Is this correct? Y/N\n");
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
            return confirmStartMonth.ToString();
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
                    // Remove this state and flip back to main menu primary game mode with no attached states.
                    ParentMode.CurrentState = null;
                    break;
                default:
                    // User didn't like that starting month so start this section over again.
                    ParentMode.CurrentState = new SelectStartingMonthState(ParentMode, UserData);
                    break;
            }
        }
    }
}