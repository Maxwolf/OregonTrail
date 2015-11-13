using System;
using System.Text;
using TrailEntities.Simulation;
using TrailEntities.Widget;

namespace TrailEntities.Game
{
    /// <summary>
    ///     References the top ten players in regards to final score they earned at the end of the game, this list is by
    ///     default hard-coded by players have the chance to save their own scores to the list if they beat the default values.
    /// </summary>
    public sealed class CurrentTopTenState : ModeState<MainMenuInfo>
    {
        /// <summary>
        ///     This constructor will be used by the other one
        /// </summary>
        public CurrentTopTenState(IModeProduct gameMode, MainMenuInfo userData) : base(gameMode, userData)
        {
        }

        /// <summary>
        ///     Returns a text only representation of the current game mode state. Could be a statement, information, question
        ///     waiting input, etc.
        /// </summary>
        public override string OnRenderState()
        {
            var currentTopTen = new StringBuilder();

            // Text above the table to declare what this state is.
            currentTopTen.Append($"{Environment.NewLine}Current Top Ten List{Environment.NewLine}{Environment.NewLine}");

            // Create text table representation of default high score list.
            var table = GameSimulationApp.Instance.ScoreTopTen.ToStringTable(
                u => u.Name,
                u => u.Points,
                u => u.Rating);
            currentTopTen.AppendLine(table);

            // Question about viewing point distribution information.
            currentTopTen.Append($"Would you like to see how{Environment.NewLine}");
            currentTopTen.Append("points are earned? Y/N");
            return currentTopTen.ToString();
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
                    //ParentMode.CurrentState = new PointsHealthState(ParentMode, UserData);
                    SetState(typeof (PointsHealthState));
                    break;
                default:
                    // Go back to the options menu.
                    //ParentMode.CurrentState = null;
                    ClearState();
                    break;
            }
        }
    }
}