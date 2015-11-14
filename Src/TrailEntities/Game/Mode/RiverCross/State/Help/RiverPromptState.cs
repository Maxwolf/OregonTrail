using System.Text;
using TrailEntities.Simulation;

namespace TrailEntities.Game
{
    /// <summary>
    ///     Shown to the user the first time they cross a river, this way it can be explained to them they must cross it in
    ///     order to continue and there is no going around. We tell them how deep the water is and how many feed across the
    ///     river is they will need to travel.
    /// </summary>
    public sealed class RiverPromptState : DialogState<RiverCrossInfo>
    {
        /// <summary>
        ///     This constructor will be used by the other one
        /// </summary>
        public RiverPromptState(IModeProduct gameMode, RiverCrossInfo userData) : base(gameMode, userData)
        {
        }

        /// <summary>
        ///     Fired when dialog prompt is attached to active game mode and would like to have a string returned.
        /// </summary>
        protected override string OnDialogPrompt()
        {
            var riverPrompt = new StringBuilder();
            riverPrompt.AppendLine("You must cross the river in");
            riverPrompt.AppendLine("order to continue. The");
            riverPrompt.AppendLine("river at this point is");
            riverPrompt.AppendLine($"currently {UserData.RiverWidth} feet across,");
            riverPrompt.AppendLine($"and {UserData.Depth} feet deep in the");
            riverPrompt.AppendLine("middle.");
            return riverPrompt.ToString();
        }

        /// <summary>
        ///     Fired when the dialog receives favorable input and determines a response based on this. From this method it is
        ///     common to attach another state, or remove the current state based on the response.
        /// </summary>
        /// <param name="reponse">The response the dialog parsed from simulation input buffer.</param>
        protected override void OnDialogResponse(DialogResponse reponse)
        {
            //parentGameMode.CurrentState = new FordRiverHelpState(parentGameMode, UserData);
            SetState(typeof (FordRiverHelpState));
        }
    }
}