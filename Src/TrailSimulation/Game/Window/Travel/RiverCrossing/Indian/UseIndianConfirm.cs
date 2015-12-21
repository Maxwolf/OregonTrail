using System;
using System.Text;
using TrailSimulation.Core;
using TrailSimulation.Entity;

namespace TrailSimulation.Game
{
    [ParentWindow(GameWindow.Travel)]
    public sealed class UseIndianConfirm : InputForm<TravelInfo>
    {
        /// <summary>
        ///     This constructor will be used by the other one
        /// </summary>
        public UseIndianConfirm(IWindow window) : base(window)
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
        ///     Fired when dialog prompt is attached to active game Windows and would like to have a string returned.
        /// </summary>
        protected override string OnDialogPrompt()
        {
            var _prompt = new StringBuilder();
            _prompt.AppendLine($"{Environment.NewLine}A Shoshoni guide says that he");
            _prompt.AppendLine("will take your wagon across");
            _prompt.AppendLine($"the river in exchange for {UserData.River.IndianCost.ToString("N0")}");
            _prompt.AppendLine("sets of clothing. Will you");
            _prompt.AppendLine("accept this offer? Y/N");
            return _prompt.ToString();
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
                case DialogResponse.Yes:
                    // Check if you have enough clothing sets to trade Indian guide for his services.
                    if (UserData.River.IndianCost >
                        GameSimulationApp.Instance.Vehicle.Inventory[Entities.Clothes].Quantity)
                    {
                        // Tell the player they do not have enough money to cross the river using the ferry.
                        SetForm(typeof(FerryNoMonies));
                        return;
                    }

                    SetForm(typeof(CrossingTick));
                    break;
                case DialogResponse.No:
                case DialogResponse.Custom:
                    UserData.River.CrossingType = RiverCrossChoice.None;
                    SetForm(typeof(RiverCross));
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(reponse), reponse, null);
            }
        }
    }
}
