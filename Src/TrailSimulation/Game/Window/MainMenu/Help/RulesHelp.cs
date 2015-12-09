using System;
using System.Text;
using TrailSimulation.Core;

namespace TrailSimulation.Game
{
    /// <summary>
    ///     Shows basic information about how the game works, how traveling works, rules for winning and losing.
    /// </summary>
    [ParentWindow(GameWindow.MainMenu)]
    public sealed class RulesHelp : InputForm<NewGameInfo>
    {
        /// <summary>
        ///     This constructor will be used by the other one
        /// </summary>
        public RulesHelp(IWindow gameMode) : base(gameMode)
        {
        }

        /// <summary>
        ///     Fired when dialog prompt is attached to active game Windows and would like to have a string returned.
        /// </summary>
        protected override string OnDialogPrompt()
        {
            var aboutTrail = new StringBuilder();
            aboutTrail.AppendLine($"{Environment.NewLine}Your journey over the Oregon Trail takes place in 1847. Start");
            aboutTrail.AppendLine($"ing in Independence, Missouri, you plan to take your family of");
            aboutTrail.AppendLine(
                $"five over {GameSimulationApp.Instance.Trail.TrailLength.ToString("N0")} tough miles to Oregon City.{Environment.NewLine}");

            aboutTrail.AppendLine($"Having saved for the trip, you bought a wagon and");
            aboutTrail.AppendLine($"now have to purchase the following items:{Environment.NewLine}");

            aboutTrail.AppendLine(
                $" * Oxen (spending more will buy you a larger and better team which");
            aboutTrail.AppendLine($" will be faster so you'll be on the trail for less time){Environment.NewLine}");

            aboutTrail.AppendLine(
                $" * Food (you'll need ample food to keep up your strength and health){Environment.NewLine}");

            aboutTrail.AppendLine($" * Ammunition ($1 buys a belt of 50 bullets. You'll need ammo for");
            aboutTrail.AppendLine($" hunting and for fighting off attacks by bandits and animals){Environment.NewLine}");

            aboutTrail.AppendLine($" * Clothing (you'll need warm clothes, especially when you hit the");
            aboutTrail.AppendLine($" snow and freezing weather in the mountains){Environment.NewLine}");

            aboutTrail.AppendLine($" * Other supplies (includes medicine, first-aid supplies, tools, and");
            aboutTrail.AppendLine($" wagon parts for unexpected emergencies){Environment.NewLine}");
            return aboutTrail.ToString();
        }

        /// <summary>
        ///     Fired when the dialog receives favorable input and determines a response based on this. From this method it is
        ///     common to attach another state, or remove the current state based on the response.
        /// </summary>
        /// <param name="reponse">The response the dialog parsed from simulation input buffer.</param>
        protected override void OnDialogResponse(DialogResponse reponse)
        {
            //parentGameMode.State = null;
            ClearForm();
        }
    }
}