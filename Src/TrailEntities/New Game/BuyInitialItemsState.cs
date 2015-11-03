using System.Text;

namespace TrailEntities
{
    /// <summary>
    ///     Spawns a new game mode in the game simulation while maintaining the state of previous one so when we bounce back we
    ///     can move from here to next state.
    /// </summary>
    public sealed class BuyInitialItemsState : ModeState<NewGameInfo>
    {
        /// <summary>
        ///     Keeps track if we have shown the information about what items the player should consider important before attaching
        ///     the actual store.
        /// </summary>
        private bool _hasAttachedStore;

        private StringBuilder _storeHelp;

        /// <summary>
        ///     This constructor will be used by the other one.
        /// </summary>
        public BuyInitialItemsState(IMode gameMode, NewGameInfo userData) : base(gameMode, userData)
        {
            // Pass the game data to the simulation for each new game mode state.
            GameSimulationApp.Instance.SetData(userData);
            _hasAttachedStore = false;

            // Create text we will display to user about the store before they actually load that game mode.
            _storeHelp = new StringBuilder();
            _storeHelp.Append("\nBefore leaving you should buy equipment and supplies.\n");
            _storeHelp.Append(
                $"You have {UserData.StartingMonies.ToString("C2")} in cash, but you don't have to spend it all now.\n\n");

            _storeHelp.Append("Press ENTER KEY to enter the store.\n");
        }

        /// <summary>
        ///     Disable input for the buy initial items since it's just telling the user something and only wants a key press event
        ///     to continue and not actual input.
        /// </summary>
        public override bool AcceptsInput
        {
            get { return false; }
        }

        /// <summary>
        ///     Returns a text only representation of the current game mode state. Could be a statement, information, question
        ///     waiting input, etc.
        /// </summary>
        public override string GetStateTUI()
        {
            return _storeHelp.ToString();
        }

        /// <summary>
        ///     Fired when the game mode current state is not null and input buffer does not match any known command.
        /// </summary>
        /// <param name="input">Contents of the input buffer which didn't match any known command in parent game mode.</param>
        public override void OnInputBufferReturned(string input)
        {
            // Make sure we never do this twice for the same instance.
            if (_hasAttachedStore)
                return;

            // Change the game mode to be a store which can work with this data.
            _hasAttachedStore = true;
            ParentMode.CurrentState = new IntroduceStoreState(ParentMode, UserData);
        }
    }
}