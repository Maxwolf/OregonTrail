using System;
using System.Text;
using TrailSimulation.Core;
using TrailSimulation.Widget;

namespace TrailSimulation.Game
{
    /// <summary>
    ///     References the top ten players in regards to final score they earned at the end of the game, this list is by
    ///     default hard-coded by players have the chance to save their own scores to the list if they beat the default values.
    /// </summary>
    [RequiredMode(GameMode.MainMenu)]
    public sealed class CurrentTopTenState : DialogState<MainMenuInfo>
    {
        /// <summary>
        ///     This constructor will be used by the other one
        /// </summary>
        public CurrentTopTenState(IModeProduct gameMode) : base(gameMode)
        {
        }

        /// <summary>
        ///     Defines what type of dialog this will act like depending on this enumeration value. Up to implementation to define
        ///     desired behavior.
        /// </summary>
        protected override DialogType DialogType
        {
            get { return DialogType.YesNo; }
        }

        /// <summary>
        ///     Fired when dialog prompt is attached to active game mode and would like to have a string returned.
        /// </summary>
        protected override string OnDialogPrompt()
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
        ///     Fired when the dialog receives favorable input and determines a response based on this. From this method it is
        ///     common to attach another state, or remove the current state based on the response.
        /// </summary>
        /// <param name="reponse">The response the dialog parsed from simulation input buffer.</param>
        protected override void OnDialogResponse(DialogResponse reponse)
        {
            switch (reponse)
            {
                case DialogResponse.No:
                    ClearState();
                    break;
                case DialogResponse.Yes:
                    // Show the user information about point distribution.
                    SetState(typeof (PointsHealthState));
                    break;
                case DialogResponse.Custom:
                    ClearState();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(reponse), reponse, null);
            }
        }
    }
}