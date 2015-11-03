using System.Text;

namespace TrailEntities
{
    /// <summary>
    ///     Second panel on point information, shows how the number of resources you end the game with contribute to your final
    ///     score.
    /// </summary>
    public sealed class PointsResourcesState : ModeState<NewGameInfo>
    {
        /// <summary>
        ///     Represents the switch for knowing if the player is done reading this information state.
        /// </summary>
        private bool _hasShownResourceInfo;

        /// <summary>
        ///     Built up strong for information and table on resource point distribution that is only built once in constructor.
        /// </summary>
        private StringBuilder _pointsItems;

        /// <summary>
        ///     This constructor will be used by the other one
        /// </summary>
        public PointsResourcesState(IMode gameMode, NewGameInfo userData) : base(gameMode, userData)
        {
            _pointsItems = new StringBuilder();
            _pointsItems.Append("\nOn Arriving in Oregon\n\n");
            _pointsItems.Append("The resources you arrive with will\n");
            _pointsItems.Append("help you get started in the new\n");
            _pointsItems.Append("land. You receive points for each\n");
            _pointsItems.Append("item you bring safely to Oregon.\n\n");

            // Build up the table of resource points and how they work for player.
            var partyTable = ScoreRegistry.ResourcePoints.ToStringTable(
                new[] {"Resources of Party", "Points per Item"},
                u => u,
                u => u.PointsAwarded
                );

            // Print the table of how resources earn points.
            _pointsItems.AppendLine(partyTable);

            // Wait for player input before going to next information state...
            _pointsItems.Append("Press ENTER KEY to continue.\n");
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
            return _pointsItems.ToString();
        }

        /// <summary>
        ///     Fired when the game mode current state is not null and input buffer does not match any known command.
        /// </summary>
        /// <param name="input">Contents of the input buffer which didn't match any known command in parent game mode.</param>
        public override void OnInputBufferReturned(string input)
        {
            if (_hasShownResourceInfo)
                return;

            // The next state will explain information about profession selection acting as handicap for point multiplier.
            _hasShownResourceInfo = true;
            ParentMode.CurrentState = new PointsOccupationState(ParentMode, UserData);
        }
    }
}