using System.Text;

namespace TrailEntities
{
    /// <summary>
    ///     Attached to the travel mode when the player requests to continue on the trail. This shows a ping-pong progress bar
    ///     moving back and fourth which lets the player know they are moving. Stats are also shown from the travel info
    ///     object, if any random events occur they will be selected from this state.
    /// </summary>
    public sealed class DriveState : ModeState<TravelInfo>
    {
        /// <summary>
        ///     Holds the current drive state, since we can size up the situation at any time.
        /// </summary>
        private StringBuilder _drive;

        /// <summary>
        ///     Determines if the driving mode should currently be ticking by everyday
        /// </summary>
        private bool _shouldTakeTickTurns;

        /// <summary>
        ///     This constructor will be used by the other one
        /// </summary>
        public DriveState(IMode gameMode, TravelInfo userData) : base(gameMode, userData)
        {
            // We don't create it in the constructor, will update with ticks.
            _drive = new StringBuilder();

            // When starting the mode we automatically begin linear progression of time.
            _shouldTakeTickTurns = true;
        }

        /// <summary>
        ///     Returns a text only representation of the current game mode state. Could be a statement, information, question
        ///     waiting input, etc.
        /// </summary>
        public override string GetStateTUI()
        {
            // Clear whatever was in the string builder last tick.
            _drive.Clear();

            // Ping-pong progress bar to show that we are moving.
            _drive.Append("CHANGE ME\n\n");

            // Basic information about simulation.
            _drive.Append(UserData.TravelStatus);

            // Wait for user input, event, or reaching next location...
            _drive.Append(GameSimulationApp.PRESS_ENTER);
            return _drive.ToString();
        }

        /// <summary>
        ///     Forces the current game mode state to update itself, this typically results in moving to the next state.
        /// </summary>
        public override void TickState()
        {
            base.TickState();

            // Check to see if we should be ticking by days with each simulation tick (defaults to every second).
            if (!_shouldTakeTickTurns)
                return;

            // Advances the simulation forward by a day every second (or each tick of simulation).
            GameSimulationApp.Instance.TakeTurn();
        }

        /// <summary>
        ///     Fired when the game mode current state is not null and input buffer does not match any known command.
        /// </summary>
        /// <param name="input">Contents of the input buffer which didn't match any known command in parent game mode.</param>
        public override void OnInputBufferReturned(string input)
        {
            // Check what state the drive state is currently in and change appropriately.
            if (string.IsNullOrEmpty(input) && _shouldTakeTickTurns)
            {
                _shouldTakeTickTurns = false;
            }
        }
    }
}