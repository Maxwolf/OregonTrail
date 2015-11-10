using System.Text;
using TrailEntities.Mode;

namespace TrailEntities.Game
{
    /// <summary>
    ///     Runs the player over the river based on the crossing information. Depending on what happens a message will be
    ///     printed to the screen explaining what happened before defaulting back to travel game mode.
    /// </summary>
    public sealed class CrossingResultState : ModeState<RiverCrossInfo>
    {
        private StringBuilder _crossingResult;
        private bool _readCrossingResult;

        /// <summary>
        ///     This constructor will be used by the other one
        /// </summary>
        public CrossingResultState(IMode gameMode, RiverCrossInfo userData) : base(gameMode, userData)
        {
            _crossingResult = new StringBuilder();

            // TODO: Determine the outcome of the player crossing the river.
            // TODO: Depending on what happened we will add some text event.
        }

        /// <summary>
        ///     Determines if user input is currently allowed to be typed and filled into the input buffer.
        /// </summary>
        /// <remarks>Default is FALSE. Setting to TRUE allows characters and input buffer to be read when submitted.</remarks>
        public override bool AcceptsInput
        {
            get { return false; }
        }

        /// <summary>
        ///     Returns a text only representation of the current game mode state. Could be a statement, information, question
        ///     waiting input, etc.
        /// </summary>
        public override string OnRenderState()
        {
            return _crossingResult.ToString();
        }

        /// <summary>
        ///     Fired when the game mode current state is not null and input buffer does not match any known command.
        /// </summary>
        /// <param name="input">Contents of the input buffer which didn't match any known command in parent game mode.</param>
        public override void OnInputBufferReturned(string input)
        {
            if (_readCrossingResult)
                return;

            // Exits the river crossing mode and returns to travel game mode.
            _readCrossingResult = true;
            ParentMode.CurrentState = null;
            ParentMode.RemoveModeNextTick();
        }
    }
}