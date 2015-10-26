using System.Text;
using TrailCommon;

namespace TrailEntities
{
    /// <summary>
    ///     Gets the name of a player for a particular index in the player name user data object. This will also offer the user
    ///     a chance to confirm their selection in another state, reset if they don't like it, and also generate a random user
    ///     name if they just press enter at the prompt for a name.
    /// </summary>
    public sealed class InputPlayerNameState : ModeState<NewGameInfo>
    {
        /// <summary>
        ///     Index in the list of player names we are going to be inserting into.
        /// </summary>
        private readonly int _playerNameIndex;

        /// <summary>
        ///     References the string that makes up the question about player names and also showing previous ones that have been
        ///     entered for continuity sake.
        /// </summary>
        private StringBuilder _inputNamesHelp;

        /// <summary>
        ///     This constructor will be used by the other one
        /// </summary>
        public InputPlayerNameState(int playerNameIndex, string questionText, IMode gameMode, NewGameInfo userData)
            : base(gameMode, userData)
        {
            // Copy over current name index and question text to ask.
            _playerNameIndex = playerNameIndex;

            // Only print player names if we have some to actually print.
            _inputNamesHelp = new StringBuilder();
            if (UserData.PlayerNames.Count > 0)
            {
                // Loop through all the player names and get their current state.
                _inputNamesHelp.Append("Your Party Members:\n\n");
                var crewNumber = 1;

                // Loop through every player and print their name.
                foreach (var name in UserData.PlayerNames)
                {
                    // First name in list is always the leader.
                    var isLeader = UserData.PlayerNames.IndexOf(name) == 0 && crewNumber == 1;
                    _inputNamesHelp.AppendFormat(isLeader ? "  {0} - {1} (leader)\n" : "  {0} - {1}\n", crewNumber, name);
                    crewNumber++;
                }

                // Add a blank line after the player names print out.
                _inputNamesHelp.Append("\n");
            }

            // Add the question text from constructor parameter.
            _inputNamesHelp.Append(questionText);
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
            return _inputNamesHelp.ToString();
        }

        /// <summary>
        ///     Fired when the game mode current state is not null and input buffer does not match any known command.
        /// </summary>
        /// <param name="input">Contents of the input buffer which didn't match any known command in parent game mode.</param>
        public override void OnInputBufferReturned(string input)
        {
            // Check if the incoming name is empty, if so fill it with a random one.
            if (string.IsNullOrEmpty(input) || string.IsNullOrWhiteSpace(input))
                input = GetPlayerName();

            // Add the name to list since we will have something at this point even if randomly generated.
            UserData.PlayerNames.Insert(_playerNameIndex, input);

            // Change the state of the game mode to confirm the name we just entered.
            ParentMode.CurrentState = new ConfirmPlayerNameState(_playerNameIndex, ParentMode, UserData);
        }

        /// <summary>
        ///     Returns a random name if there is an empty name returned, we assume the player doesn't care and just give him one.
        /// </summary>
        internal string GetPlayerName()
        {
            string[] names = {"Bob", "Joe", "Sally", "Tim", "Steve"};
            return names[GameSimulationApp.Instance.Random.Next(names.Length)];
        }
    }
}