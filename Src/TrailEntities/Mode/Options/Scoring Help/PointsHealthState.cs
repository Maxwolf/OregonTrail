using System.Text;

namespace TrailEntities
{
    /// <summary>
    ///     First panel on point information, shows how health of party members contributes to final score.
    /// </summary>
    public sealed class PointsHealthState : ModeState<OptionInfo>
    {
        /// <summary>
        ///     Reference to information about scoring based on party health.
        /// </summary>
        private StringBuilder _pointsHealth;

        /// <summary>
        ///     Determines if the player is done looking at this state.
        /// </summary>
        private bool _seenHealthPointsHelp;

        /// <summary>
        ///     This constructor will be used by the other one
        /// </summary>
        public PointsHealthState(IMode gameMode, OptionInfo userData) : base(gameMode, userData)
        {
            // Build up string of help about points for people.
            _pointsHealth = new StringBuilder();
            _pointsHealth.Append("\nOn Arriving in Oregon\n\n");
            _pointsHealth.Append("Your most important resource if the\n");
            _pointsHealth.Append("people you have with you. You\n");
            _pointsHealth.Append("receive points for each member of\n");
            _pointsHealth.Append("your party who arrives safely; you\n");
            _pointsHealth.Append("receive more points if they arrive\n");
            _pointsHealth.Append("in good health!\n\n");

            // Build a text table from people point distribution with custom headers.
            var partyTable = ScoreRegistry.PeoplePoints.ToStringTable(
                new[] {"Health of Party", "Points per Person"},
                u => u.PartyHealth,
                u => u.PointsPerPerson
                );

            // Print the table to the screen buffer.
            _pointsHealth.AppendLine(partyTable);

            // Wait for use input to goto next help screen...
            _pointsHealth.Append("Press ENTER KEY to continue.\n");
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
            return _pointsHealth.ToString();
        }

        /// <summary>
        ///     Fired when the game mode current state is not null and input buffer does not match any known command.
        /// </summary>
        /// <param name="input">Contents of the input buffer which didn't match any known command in parent game mode.</param>
        public override void OnInputBufferReturned(string input)
        {
            if (_seenHealthPointsHelp)
                return;

            // Onward to information about items!
            _seenHealthPointsHelp = true;
            ParentMode.CurrentState = new PointsResourcesState(ParentMode, UserData);
        }
    }
}