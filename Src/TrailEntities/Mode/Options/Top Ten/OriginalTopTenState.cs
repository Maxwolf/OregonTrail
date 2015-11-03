using System.Text;

namespace TrailEntities
{
    /// <summary>
    ///     Shows the player hard-coded top ten list as it is known internally in static list.
    /// </summary>
    public sealed class OriginalTopTenState : ModeState<OptionInfo>
    {
        /// <summary>
        ///     This constructor will be used by the other one
        /// </summary>
        public OriginalTopTenState(IMode gameMode, OptionInfo userData) : base(gameMode, userData)
        {
        }

        /// <summary>
        ///     Returns a text only representation of the current game mode state. Could be a statement, information, question
        ///     waiting input, etc.
        /// </summary>
        public override string GetStateTUI()
        {
            var sourceTopTen = new StringBuilder();

            // Text above the table to declare what this state is.
            sourceTopTen.Append("\nThe Oregon Top Ten\n\n");

            // Create text table representation of default high score list.
            var table = ScoreRegistry.TopTenDefaults.ToStringTable(
                u => u.Name,
                u => u.Points,
                u => u.Rating);
            sourceTopTen.AppendLine(table);

            // Question about viewing point distribution information.
            sourceTopTen.Append("Would you like to see how\n");
            sourceTopTen.Append("points are earned? Y/N");

            // Wait for user input...
            return sourceTopTen.ToString();
        }

        /// <summary>
        ///     Fired when the game mode current state is not null and input buffer does not match any known command.
        /// </summary>
        /// <param name="input">Contents of the input buffer which didn't match any known command in parent game mode.</param>
        public override void OnInputBufferReturned(string input)
        {
            switch (input.ToUpperInvariant())
            {
                case "Y":
                    // Show the user information about point distribution.
                    ParentMode.CurrentState = new PointsHealthState(ParentMode, UserData);
                    break;
                default:
                    // Go back to the options menu.
                    ParentMode.CurrentState = null;
                    break;
            }
        }
    }
}