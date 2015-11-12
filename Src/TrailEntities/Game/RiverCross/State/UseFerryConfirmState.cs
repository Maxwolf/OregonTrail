using System.Text;
using TrailEntities.Simulation.Mode;

namespace TrailEntities.Game
{
    /// <summary>
    ///     Explains to the user how many monies and days they will be charged to cross the river using the ferry and to
    ///     confirm by saying yes. At this point the simulation will check if they have enough money or not and jump to the
    ///     next state accordingly.
    /// </summary>
    public sealed class UseFerryConfirmState : DialogState<RiverCrossInfo>
    {
        /// <summary>
        ///     This constructor will be used by the other one
        /// </summary>
        public UseFerryConfirmState(IModeProduct gameMode, RiverCrossInfo userData) : base(gameMode, userData)
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
            var _prompt = new StringBuilder();
            _prompt.AppendLine("The ferry operator says that");
            _prompt.AppendLine($"he will charge you {UserData.FerryCost.ToString("C2")} and");
            _prompt.AppendLine($"that you will have to wait {UserData.FerryDelayInDays}");
            _prompt.AppendLine("days. Are you willing to do");
            _prompt.AppendLine("this?");
            return _prompt.ToString();
        }

        /// <summary>
        ///     Fired when the dialog receives favorable input and determines a response based on this. From this method it is
        ///     common to attach another state, or remove the current state based on the response.
        /// </summary>
        /// <param name="reponse">The response the dialog parsed from simulation input buffer.</param>
        protected override void OnDialogResponse(DialogResponse reponse)
        {
            // TODO: Check if you have enough monies to use the ferry.

            switch (reponse)
            {
                case DialogResponse.Yes:
                    SetState(typeof(CrossingResultState));
                    break;
                default:
                    ClearState();
                    break;
            }
        }
    }
}