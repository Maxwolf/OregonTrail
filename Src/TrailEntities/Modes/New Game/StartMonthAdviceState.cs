using System.Text;
using TrailCommon;

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
            _startMonthHelp.Append("You need to decide when to set off on the trail.  If you leave too soon, there\n");
            _startMonthHelp.Append("won't be much grass for your oxen to eat.  You may encounter some very cold\n");
            _startMonthHelp.Append("weather and late spring snowstorms.\n\n");

            _startMonthHelp.Append("Starting in March will guarantee a rough start.  It will be cold in the\n");
            _startMonthHelp.Append("beginning, but you'll get a rather mild weathered journey from the middle on.\n\n");

            _startMonthHelp.Append("Starting in April/May is the easiest.  You'll get good weather throughout your\n");
            _startMonthHelp.Append("journey, until (possibly) the very end, if you rest a lot.\n\n");

            _startMonthHelp.Append("Starting in June-August ensures that you'll have to endure a tough winter in\n");
            _startMonthHelp.Append("the end (or middle, depending on how late you start) of your journey.\n\n");

            _startMonthHelp.Append("Press ENTER KEY to return to starting month selection.\n");
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