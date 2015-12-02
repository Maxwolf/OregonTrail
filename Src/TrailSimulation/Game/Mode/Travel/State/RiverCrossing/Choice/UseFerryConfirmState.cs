using System.Text;
using TrailSimulation.Core;
using TrailSimulation.Entity;

namespace TrailSimulation.Game
{
    /// <summary>
    ///     Explains to the user how many monies and days they will be charged to cross the river using the ferry and to
    ///     confirm by saying yes. At this point the simulation will check if they have enough money or not and jump to the
    ///     next state accordingly.
    /// </summary>
    [RequiredMode(Mode.Travel)]
    public sealed class UseFerryConfirmState : DialogState<TravelInfo>
    {
        /// <summary>
        ///     This constructor will be used by the other one
        /// </summary>
        public UseFerryConfirmState(IModeProduct gameMode) : base(gameMode)
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
            _prompt.AppendLine($"he will charge you {UserData.RiverInfo.FerryCost.ToString("C2")} and");
            _prompt.AppendLine($"that you will have to wait {UserData.RiverInfo.FerryDelayInDays}");
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
            // Check if you have enough monies to use the ferry.
            var cashAmount = GameSimulationApp.Instance.Vehicle.Inventory[SimEntity.Cash].TotalValue;
            if (UserData.RiverInfo.FerryCost > cashAmount)
            {
                // Tell the player they do not have enough money to cross the river using the ferry.
                SetState(typeof(FerryNoMoniesState));
                return;
            }

            // Check if the ferry operator wants player to wait a certain amount of days before they can cross.
            if (UserData.DaysToRest > 0)
            {
                SetState(typeof(RestingState));
                return;
            }

            // Player has enough money for ferry operator, and there is no more delay we can cross now.
            switch (reponse)
            {
                case DialogResponse.Yes:
                    SetState(typeof (CrossingResultState));
                    break;
                default:
                    SetState(typeof(RiverCrossState));
                    break;
            }
        }
    }
}