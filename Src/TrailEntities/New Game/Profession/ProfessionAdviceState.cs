using System.Text;

namespace TrailEntities
{
    /// <summary>
    ///     Shows information about what the player leader professions mean and how it affects the party, vehicle, game
    ///     difficulty, and scoring at the end (if they make it).
    /// </summary>
    public sealed class ProfessionAdviceState : ModeState<NewGameInfo>
    {
        /// <summary>
        ///     Determines if the player is done reading the profession advice.
        /// </summary>
        private bool _hasReadProfessionAdvice;

        /// <summary>
        ///     Holds reference to advice string that is built in constructor.
        /// </summary>
        private StringBuilder _job;

        public ProfessionAdviceState(IMode gameMode, NewGameInfo userData) : base(gameMode, userData)
        {
            // Information about professions and how they work.
            _job = new StringBuilder();
            _job.Append("\nTraveling to Oregon isn't easy!\n");
            _job.Append("But if you're a banker, you'll\n");
            _job.Append("have more money for supplies\n");
            _job.Append("and services than a carpenter\n");
            _job.Append("or a farmer.\n\n");
            _job.Append("However, the harder you have\n");
            _job.Append("to try, the more points you\n");
            _job.Append("deserve! Therefore, the\n");
            _job.Append("farmer earns the greatest\n");
            _job.Append("number of points and the\n");
            _job.Append("banker earns the least.\n\n");

            _job.Append(GameSimulationApp.PRESS_ENTER);
        }

        public override bool AcceptsInput
        {
            get { return false; }
        }

        public override string GetStateTUI()
        {
            return _job.ToString();
        }

        public override void OnInputBufferReturned(string input)
        {
            if (_hasReadProfessionAdvice)
                return;

            // Return the select profession state if we are ready for that.
            _hasReadProfessionAdvice = true;
            ParentMode.CurrentState = new SelectProfessionState(ParentMode, UserData);
        }
    }
}