using System.Text;
using TrailEntities.Simulation.Mode;

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
        protected override string OnDialogPrompt(StringBuilder dialogPrompt)
        {
            dialogPrompt.AppendLine("You must cross the river in");
            dialogPrompt.AppendLine("order to continue. The");
            dialogPrompt.AppendLine("river at this point is");
            dialogPrompt.AppendLine("currently 618 feed across,");
            dialogPrompt.AppendLine("and 3.5 feet deep in the");
            dialogPrompt.AppendLine("middle.");
            return dialogPrompt.ToString();
        }

        /// <summary>
        ///     Fired when the dialog receives favorable input and determines a response based on this. From this method it is
        ///     common to attach another state, or remove the current state based on the response.
        /// </summary>
        /// <param name="promptType">References what type of dialog prompt this was configured to be before response was triggered.</param>
        /// <param name="reponse">The response the dialog parsed from simulation input buffer.</param>
        protected override void OnDialogResponse(DialogType promptType, DialogResponse reponse)
        {
            if (reponse == DialogResponse.Continue)
                ParentMode.CurrentState = new FordRiverHelpState(ParentMode, UserData);
        }
    }
}