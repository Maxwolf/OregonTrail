namespace TrailEntities
{
    /// <summary>
    ///     Attaches a state that will ask the player how long they would like to rest in the number of days, zero is a valid
    ///     response and will not do anything. If greater than zero we will attach another state to tick that many days by in
    ///     the simulation.
    /// </summary>
    public sealed class RestQuestionState : ModeState<TravelInfo>
    {
        /// <summary>
        ///     This constructor will be used by the other one
        /// </summary>
        public RestQuestionState(IMode gameMode, TravelInfo userData) : base(gameMode, userData)
        {
        }

        /// <summary>
        ///     Returns a text only representation of the current game mode state. Could be a statement, information, question
        ///     waiting input, etc.
        /// </summary>
        public override string GetStateTUI()
        {
            return "How many days would you like to rest?";
        }

        /// <summary>
        ///     Fired when the game mode current state is not null and input buffer does not match any known command.
        /// </summary>
        /// <param name="input">Contents of the input buffer which didn't match any known command in parent game mode.</param>
        public override void OnInputBufferReturned(string input)
        {
            // Parse the user input buffer as a unsigned int.
            int parsedInputNumber;
            if (!int.TryParse(input, out parsedInputNumber))
                return;

            // If player rests for more than one day then set the resting state to that, otherwise just go back to travel menu.
            ParentMode.CurrentState = parsedInputNumber > 0
                ? new RestingState(ParentMode, UserData, parsedInputNumber)
                : null;
        }
    }
}