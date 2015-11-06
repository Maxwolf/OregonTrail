using System;
using System.Text;

namespace TrailEntities
{
    /// <summary>
    ///     Shows the player hard-coded top ten list as it is known internally in static list.
    /// </summary>
    public sealed class OriginalTopTenState : ModeState<OptionInfo>
    {
        /// <summary>
        ///     Determines if the player is done reading the original top ten list.
        /// </summary>
        private bool _doneSeeingOriginalTopTen;

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
            sourceTopTen.Append($"{Environment.NewLine}The Oregon Top Ten{Environment.NewLine}{Environment.NewLine}");

            // Create text table representation of default high score list.
            var table = ScoreRegistry.TopTenDefaults.ToStringTable(
                u => u.Name,
                u => u.Points,
                u => u.Rating);
            sourceTopTen.AppendLine(table);

            // Wait for user input...
            sourceTopTen.Append(GameSimApp.PRESS_ENTER);
            return sourceTopTen.ToString();
        }

        /// <summary>
        ///     Fired when the game mode current state is not null and input buffer does not match any known command.
        /// </summary>
        /// <param name="input">Contents of the input buffer which didn't match any known command in parent game mode.</param>
        public override void OnInputBufferReturned(string input)
        {
            if (_doneSeeingOriginalTopTen)
                return;

            _doneSeeingOriginalTopTen = true;
            ParentMode.CurrentState = null;
        }
    }
}