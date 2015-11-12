using System;
using System.Text;
using TrailEntities.Simulation;
using TrailEntities.Simulation.Mode;

namespace TrailEntities.Game
{
    /// <summary>
    ///     Shows basic information about how the game works, how traveling works, rules for winning and losing.
    /// </summary>
    public sealed class InstructionsState : DialogState<MainMenuInfo>
    {
        /// <summary>
        ///     This constructor will be used by the other one
        /// </summary>
        public InstructionsState(IModeProduct gameMode, MainMenuInfo userData) : base(gameMode, userData)
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
            var aboutTrail = new StringBuilder();
            aboutTrail.Append($"Your journey over the Oregon Trail takes place in 1847. Start{Environment.NewLine}");
            aboutTrail.Append($"ing in Independence, Missouri, you plan to take your family of{Environment.NewLine}");
            aboutTrail.Append(
                $"five over {GameSimulationApp.TRAIL_LENGTH} tough miles to Oregon City.{Environment.NewLine}{Environment.NewLine}");

            aboutTrail.Append($"Having saved for the trip, you bought a wagon and{Environment.NewLine}");
            aboutTrail.Append($"now have to purchase the following items:{Environment.NewLine}{Environment.NewLine}");

            aboutTrail.Append(
                $" * Oxen (spending more will buy you a larger and better team which{Environment.NewLine}");
            aboutTrail.Append(
                $" will be faster so you'll be on the trail for less time){Environment.NewLine}{Environment.NewLine}");

            aboutTrail.Append(
                $" * Food (you'll need ample food to keep up your strength and health){Environment.NewLine}{Environment.NewLine}");

            aboutTrail.Append(
                $" * Ammunition ($1 buys a belt of 50 bullets. You'll need ammo for{Environment.NewLine}");
            aboutTrail.Append(
                $" hunting and for fighting off attacks by bandits and animals){Environment.NewLine}{Environment.NewLine}");

            aboutTrail.Append(
                $" * Clothing (you'll need warm clothes, especially when you hit the{Environment.NewLine}");
            aboutTrail.Append(
                $" snow and freezing weather in the mountains){Environment.NewLine}{Environment.NewLine}");

            aboutTrail.Append(
                $" * Other supplies (includes medicine, first-aid supplies, tools, and{Environment.NewLine}");
            aboutTrail.Append($" wagon parts for unexpected emergencies){Environment.NewLine}{Environment.NewLine}");
            return aboutTrail.ToString();
        }

        /// <summary>
        ///     Fired when the dialog receives favorable input and determines a response based on this. From this method it is
        ///     common to attach another state, or remove the current state based on the response.
        /// </summary>
        /// <param name="reponse">The response the dialog parsed from simulation input buffer.</param>
        protected override void OnDialogResponse(DialogResponse reponse)
        {
            //ParentMode.CurrentState = null;
            ClearState();
        }
    }
}