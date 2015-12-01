using System;
using System.Text;
using TrailSimulation.Core;

namespace TrailSimulation.Game
{
    /// <summary>
    ///     Shown to the user the first time they cross a river, this way it can be explained to them they must cross it in
    ///     order to continue and there is no going around. We tell them how deep the water is and how many feed across the
    ///     river is they will need to travel.
    /// </summary>
    [RequiredMode(Mode.Travel)]
    public sealed class RiverPromptState : DialogState<TravelInfo>
    {
        /// <summary>
        ///     This constructor will be used by the other one
        /// </summary>
        public RiverPromptState(IModeProduct gameMode) : base(gameMode)
        {
        }

        /// <summary>
        ///     Fired when dialog prompt is attached to active game mode and would like to have a string returned.
        /// </summary>
        protected override string OnDialogPrompt()
        {
            // Generates a new river with randomized width and depth.
            UserData.RiverInfo = new RiverGenerator();

            var riverPrompt = new StringBuilder();
            riverPrompt.AppendLine($"{Environment.NewLine}You must cross the river in");
            riverPrompt.AppendLine("order to continue. The");
            riverPrompt.AppendLine("river at this point is");
            riverPrompt.AppendLine($"currently {UserData.RiverInfo.RiverWidth} feet across,");
            riverPrompt.AppendLine($"and {UserData.RiverInfo.RiverDepth} feet deep in the");
            riverPrompt.AppendLine($"middle.{Environment.NewLine}");
            return riverPrompt.ToString();
        }

        /// <summary>
        ///     Fired when the dialog receives favorable input and determines a response based on this. From this method it is
        ///     common to attach another state, or remove the current state based on the response.
        /// </summary>
        /// <param name="reponse">The response the dialog parsed from simulation input buffer.</param>
        protected override void OnDialogResponse(DialogResponse reponse)
        {
            SetState(typeof (RiverCrossState));
        }
    }
}