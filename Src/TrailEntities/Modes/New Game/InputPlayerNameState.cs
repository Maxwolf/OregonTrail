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
        ///     Question that is asked about the player on startup, easier to pass this along in constructor since there are four
        ///     names but slightly different questions being asked.
        /// </summary>
        private readonly string _questionText;

        /// <summary>
        ///     This constructor will be used by the other one
        /// </summary>
        public InputPlayerNameState(int playerNameIndex, string questionText, IMode gameMode, NewGameInfo userData)
            : base(gameMode, userData)
        {
            _playerNameIndex = playerNameIndex;
            _questionText = questionText;
        }

        /// <summary>
        ///     Returns a text only representation of the current game mode state. Could be a statement, information, question
        ///     waiting input, etc.
        /// </summary>
        public override string GetStateTUI()
        {
            return _questionText;
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
            Mode.CurrentState = new ConfirmPlayerNameState(_playerNameIndex, Mode, UserData);
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