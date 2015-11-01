using System.Text;
using TrailCommon;

namespace TrailEntities
{
    /// <summary>
    ///     Keeps track of a set number of days and every time the game mode is ticked a day is simulated and days to rest
    ///     subtracted until we are at zero, then the player can close the window but until then input will not be accepted.
    /// </summary>
    public sealed class RestingState : ModeState<TravelInfo>
    {
        /// <summary>
        ///     Number of days the player and his party would like to rest, if zero we just close right away.
        /// </summary>
        private uint _daysToRest;

        /// <summary>
        ///     Keeps track if the player has rested the amount of time they wanted.
        /// </summary>
        private bool _hasRested;

        /// <summary>
        ///     This constructor will be used by the other one
        /// </summary>
        public RestingState(IMode gameMode, TravelInfo userData, uint daysToRest) : base(gameMode, userData)
        {
            _daysToRest = daysToRest;
        }

        /// <summary>
        ///     Returns a text only representation of the current game mode state. Could be a statement, information, question
        ///     waiting input, etc.
        /// </summary>
        public override string GetStateTUI()
        {
            var rest = new StringBuilder();
            rest.Append($"You rest for {_daysToRest}");
            rest.Append(_daysToRest > 1 ? "days\n" : "day\n");

            rest.Append("Press ENTER KEY to return to store.\n");
            return rest.ToString();
        }

        /// <summary>
        ///     Fired when the game mode current state is not null and input buffer does not match any known command.
        /// </summary>
        /// <param name="input">Contents of the input buffer which didn't match any known command in parent game mode.</param>
        public override void OnInputBufferReturned(string input)
        {
            // Not accepting user input when resting.
            if (_daysToRest > 1)
                return;

            // Can only actually stop resting once.
            if (!_hasRested)
            {
                _hasRested = true;
                ParentMode.CurrentState = null;
            }
        }
    }
}