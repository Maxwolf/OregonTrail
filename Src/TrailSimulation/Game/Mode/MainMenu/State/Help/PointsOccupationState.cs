using System;
using System.Text;
using TrailSimulation.Core;

namespace TrailSimulation.Game
{
    /// <summary>
    ///     Third and final panel on point information, explains how players profession selection affects final scoring as a
    ///     multiplier since starting as a banker is a handicap.
    /// </summary>
    [RequiredMode(GameMode.MainMenu)]
    public sealed class PointsOccupationState : DialogState<MainMenuInfo>
    {
        /// <summary>
        ///     This constructor will be used by the other one
        /// </summary>
        public PointsOccupationState(IModeProduct gameMode) : base(gameMode)
        {
        }

        /// <summary>
        ///     Fired when dialog prompt is attached to active game mode and would like to have a string returned.
        /// </summary>
        protected override string OnDialogPrompt()
        {
            var _pointsProfession = new StringBuilder();
            _pointsProfession.Append(
                $"{Environment.NewLine}On Arriving in Oregon{Environment.NewLine}{Environment.NewLine}");
            _pointsProfession.AppendLine("You receive points for your");
            _pointsProfession.AppendLine("occupation in the new land.");
            _pointsProfession.AppendLine("Because more farmers and");
            _pointsProfession.AppendLine("carpenters were needed than");
            _pointsProfession.AppendLine("bankers, you receive double");
            _pointsProfession.AppendLine("points upon arriving in Oregon");
            _pointsProfession.AppendLine("as a carpenter, and triple");
            _pointsProfession.AppendLine($"points for arriving as a farmer.{Environment.NewLine}");
            return _pointsProfession.ToString();
        }

        /// <summary>
        ///     Fired when the dialog receives favorable input and determines a response based on this. From this method it is
        ///     common to attach another state, or remove the current state based on the response.
        /// </summary>
        /// <param name="reponse">The response the dialog parsed from simulation input buffer.</param>
        protected override void OnDialogResponse(DialogResponse reponse)
        {
            ClearState();
        }
    }
}