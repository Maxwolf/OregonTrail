using System;
using System.Text;
using TrailSimulation.Core;
using TrailSimulation.Widget;

namespace TrailSimulation.Game
{
    /// <summary>
    ///     Shows the player hard-coded top ten list as it is known internally in static list.
    /// </summary>
    [RequiredMode(GameMode.Options)]
    public sealed class OriginalTopTenState : DialogState<OptionInfo>
    {
        /// <summary>
        ///     This constructor will be used by the other one
        /// </summary>
        public OriginalTopTenState(IModeProduct gameMode) : base(gameMode)
        {
        }

        /// <summary>
        ///     Fired when dialog prompt is attached to active game mode and would like to have a string returned.
        /// </summary>
        protected override string OnDialogPrompt()
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
            return sourceTopTen.ToString();
        }

        /// <summary>
        ///     Fired when the dialog receives favorable input and determines a response based on this. From this method it is
        ///     common to attach another state, or remove the current state based on the response.
        /// </summary>
        /// <param name="reponse">The response the dialog parsed from simulation input buffer.</param>
        protected override void OnDialogResponse(DialogResponse reponse)
        {
            //parentGameMode.CurrentState = null;
            ClearState();
        }
    }
}