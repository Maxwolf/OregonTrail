using System.Text;
using TrailCommon;

namespace TrailEntities
{
    /// <summary>
    ///     Prints out every entered player name in the user data for simulation initialization. Confirms with the player they
    ///     would indeed like to use all the entered names they have provided or had randomly generated for them by just
    ///     pressing enter.
    /// </summary>
    public sealed class ConfirmGroupNamesState : ModeState<NewGameInfo>
    {
        /// <summary>
        ///     This constructor will be used by the other one
        /// </summary>
        public ConfirmGroupNamesState(IMode gameMode, NewGameInfo userData) : base(gameMode, userData)
        {
        }

        /// <summary>
        ///     Returns a text only representation of the current game mode state. Could be a statement, information, question
        ///     waiting input, etc.
        /// </summary>
        public override string GetStateTUI()
        {
            // Create string builder, counter, print info about party members.
            var confirmPartyText = new StringBuilder();
            confirmPartyText.Append("Your Party Members:\n");
            var crewNumber = 1;

            // Loop through every player and print their name.
            foreach (var name in UserData.PlayerNames)
            {
                var isLeader = UserData.PlayerNames.IndexOf(name) == 0 && crewNumber == 1;
                confirmPartyText.AppendFormat(isLeader ? "  {0} - {1} (leader)\n" : "  {0} - {1}\n", crewNumber, name);
                crewNumber++;
            }

            // Ask the user to check if the data we have looks correct to them, wait for input...
            confirmPartyText.Append("Are these names correct? Y/N");
            return confirmPartyText.ToString();
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
                    // Move along to confirming profession for party leader if user is happy with names.
                    Mode.CurrentState = new SelectProfessionState(Mode, UserData);
                    break;
                default:
                    // Clear all previous names we are going to try this again.
                    UserData.PlayerNames.Clear();

                    // Go back to the beginning of the player name selection chain.
                    Mode.CurrentState = new InputPlayerNameState(0, "Party leader name?", Mode, UserData);
                    break;
            }
        }
    }
}