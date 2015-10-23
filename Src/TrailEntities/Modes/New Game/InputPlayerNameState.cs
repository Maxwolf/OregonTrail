using TrailCommon;

namespace TrailEntities
{
    public sealed class InputPlayerNameState : ModeState<NewGameInfo>
    {
        private readonly int _playerNameIndex;
        private readonly string _questionText;

        /// <summary>
        ///     This constructor will be used by the other one
        /// </summary>
        public InputPlayerNameState(int playerNameIndex, string questionText, IMode gameMode, NewGameInfo userData) : base(gameMode, userData)
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

            UserData.PlayerNames.Insert(_playerNameIndex, input);
            Mode.CurrentState = new ConfirmPlayerName(_playerNameIndex, Mode, UserData);
        }

        internal string GetPlayerName()
        {
            // Just return a random name if there is invalid input.
            string[] names = { "Bob", "Joe", "Sally", "Tim", "Steve" };
            return names[GameSimulationApp.Instance.Random.Next(names.Length)];
        }
    }
}