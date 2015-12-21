using System;
using System.Text;
using TrailSimulation.Core;

namespace TrailSimulation.Game
{
    /// <summary>
    ///     Attached if the player wants to use the river crossing services an Indian guide provides, however they don't enough
    ///     clothing sets to trade him. The player at this point will have to refuse his services since they cannot trade at
    ///     the river crossing unless they looked around ahead of time. The amount of clothing sets the Indian also asks for
    ///     changes based on the number of animals the player hunts and kills, the more they kill the more he wants.
    /// </summary>
    [ParentWindow(GameWindow.Travel)]
    public sealed class IndianNoClothes : InputForm<TravelInfo>
    {
        /// <summary>
        ///     This constructor will be used by the other one
        /// </summary>
        public IndianNoClothes(IWindow window) : base(window)
        {
        }

        /// <summary>
        ///     Fired when dialog prompt is attached to active game Windows and would like to have a string returned.
        /// </summary>
        protected override string OnDialogPrompt()
        {
            var _prompt = new StringBuilder();
            _prompt.AppendLine($"{Environment.NewLine}You do not have enough");
            _prompt.AppendLine("monies to take the");
            _prompt.AppendLine($"ferry.{Environment.NewLine}");
            return _prompt.ToString();
        }

        /// <summary>
        ///     Fired when the dialog receives favorable input and determines a response based on this. From this method it is
        ///     common to attach another state, or remove the current state based on the response.
        /// </summary>
        /// <param name="reponse">The response the dialog parsed from simulation input buffer.</param>
        protected override void OnDialogResponse(DialogResponse reponse)
        {
            SetForm(typeof(RiverCross));
        }
    }
}