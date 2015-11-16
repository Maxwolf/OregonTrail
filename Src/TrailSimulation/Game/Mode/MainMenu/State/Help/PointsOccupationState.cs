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
        ///     Defines what type of dialog this will act like depending on this enumeration value. Up to implementation to define
        ///     desired behavior.
        /// </summary>
        protected override DialogType DialogType
        {
            get { return DialogType.Prompt; }
        }

        /// <summary>
        ///     Fired when dialog prompt is attached to active game mode and would like to have a string returned.
        /// </summary>
        protected override string OnDialogPrompt()
        {
            var _pointsProfession = new StringBuilder();
            _pointsProfession.Append(
                $"{Environment.NewLine}On Arriving in Oregon{Environment.NewLine}{Environment.NewLine}");
            _pointsProfession.Append($"You receive points for your{Environment.NewLine}");
            _pointsProfession.Append($"occupation in the new land.{Environment.NewLine}");
            _pointsProfession.Append($"Because more farmers and{Environment.NewLine}");
            _pointsProfession.Append($"carpenters were needed than{Environment.NewLine}");
            _pointsProfession.Append($"bankers, you receive double{Environment.NewLine}");
            _pointsProfession.Append($"points upon arriving in Oregon{Environment.NewLine}");
            _pointsProfession.Append($"as a carpenter, and triple{Environment.NewLine}");
            _pointsProfession.Append($"points for arriving as a farmer.{Environment.NewLine}");
            return _pointsProfession.ToString();
        }

        /// <summary>
        ///     Fired when the dialog receives favorable input and determines a response based on this. From this method it is
        ///     common to attach another state, or remove the current state based on the response.
        /// </summary>
        /// <param name="reponse">The response the dialog parsed from simulation input buffer.</param>
        protected override void OnDialogResponse(DialogResponse reponse)
        {
            //parentGameMode.CurrentState = null;
            RemoveState();
        }
    }
}