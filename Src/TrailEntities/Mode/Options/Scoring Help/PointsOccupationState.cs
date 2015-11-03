using System.Text;

namespace TrailEntities
{
    /// <summary>
    ///     Third and final panel on point information, explains how players profession selection affects final scoring as a
    ///     multiplier since starting as a banker is a handicap.
    /// </summary>
    public sealed class PointsOccupationState : ModeState<OptionInfo>
    {
        /// <summary>
        ///     Determines if the player is done looking at information on profession scoring.
        /// </summary>
        private bool _hasSeenProfessionHelp;

        /// <summary>
        ///     Built up string that will be representation of this state, only built during constructor.
        /// </summary>
        private StringBuilder _pointsProfession;

        /// <summary>
        ///     This constructor will be used by the other one
        /// </summary>
        public PointsOccupationState(IMode gameMode, OptionInfo userData) : base(gameMode, userData)
        {
            _pointsProfession = new StringBuilder();
            _pointsProfession.Append("\nOn Arriving in Oregon\n\n");
            _pointsProfession.Append("You receive points for your\n");
            _pointsProfession.Append("occupation in the new land.\n");
            _pointsProfession.Append("Because more farmers and\n");
            _pointsProfession.Append("carpenters were needed than\n");
            _pointsProfession.Append("bankers, you receive double\n");
            _pointsProfession.Append("points upon arriving in Oregon\n");
            _pointsProfession.Append("as a carpenter, and triple\n");
            _pointsProfession.Append("points for arriving as a farmer.\n\n");

            _pointsProfession.Append("Press ENTER KEY to continue.\n");
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
        public override string GetStateTUI()
        {
            return _pointsProfession.ToString();
        }

        /// <summary>
        ///     Fired when the game mode current state is not null and input buffer does not match any known command.
        /// </summary>
        /// <param name="input">Contents of the input buffer which didn't match any known command in parent game mode.</param>
        public override void OnInputBufferReturned(string input)
        {
            if (_hasSeenProfessionHelp)
                return;

            // Return to options menu.
            _hasSeenProfessionHelp = true;
            ParentMode.CurrentState = null;
        }
    }
}