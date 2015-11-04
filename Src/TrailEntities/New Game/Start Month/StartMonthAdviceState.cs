using System.Text;

namespace TrailEntities
{
    /// <summary>
    ///     Shows the player information about what the various starting months mean.
    /// </summary>
    public sealed class StartMonthAdviceState : ModeState<NewGameInfo>
    {
        /// <summary>
        ///     References if the user has pressed any key to get rid of the starting month advice.
        /// </summary>
        private bool _hasShownStartMonthAdvice;

        /// <summary>
        ///     References the string that represents the advice for starting months and what they mean in the simulation.
        /// </summary>
        private StringBuilder _startMonthHelp;

        public StartMonthAdviceState(IMode gameMode, NewGameInfo userData) : base(gameMode, userData)
        {
            // Have not shown this yet.
            _hasShownStartMonthAdvice = false;

            // Inform the user about a decision they need to make.
            _startMonthHelp = new StringBuilder();
            _startMonthHelp.Append("\nYou attend a public meeting held\n");
            _startMonthHelp.Append("for \"folks with the California -\n");
            _startMonthHelp.Append("Oregon fever.\" You're told:\n\n");
            _startMonthHelp.Append("If you leave too early, there\n");
            _startMonthHelp.Append("won't be any grass for your\n");
            _startMonthHelp.Append("oxen to eat. If you leave too\n");
            _startMonthHelp.Append("late, you may not get to Oregon\n");
            _startMonthHelp.Append("before winter comes. If you\n");
            _startMonthHelp.Append("leave at just the right time,\n");
            _startMonthHelp.Append("there will be green grass and\n");
            _startMonthHelp.Append("the weather will still be cool.\n\n");

            // Wait for user input...
            _startMonthHelp.Append("Press ENTER KEY to continue.\n");
        }

        public override bool AcceptsInput
        {
            get { return false; }
        }

        public override string GetStateTUI()
        {
            return _startMonthHelp.ToString();
        }

        public override void OnInputBufferReturned(string input)
        {
            // Ensure this only happens once per instance of advice.
            if (_hasShownStartMonthAdvice)
                return;

            // Return to starting month selection state.
            _hasShownStartMonthAdvice = true;
            ParentMode.CurrentState = new SelectStartingMonthState(ParentMode, UserData);
        }
    }
}