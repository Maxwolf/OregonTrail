using System;
using System.Text;
using TrailSimulation.Core;

namespace TrailSimulation.Game
{
    /// <summary>
    ///     Allows the user to completely configure the simulation before they start off on the trail path. It will offer up
    ///     ability to choose names, professions, buy initial items, and starting month. The final thing it offers is ability
    ///     to change any of these values before actually starting the game as a final confirmation.
    /// </summary>
    public sealed class MainMenuMode : ModeProduct<MainMenuCommands, MainMenuInfo>
    {
        /// <summary>
        ///     Asked for the first party member.
        /// </summary>
        public const string LEADER_QUESTION = "What is the first name of the wagon leader?";

        /// <summary>
        ///     Asked for every other party member name we want to collect.
        /// </summary>
        public const string MEMBERS_QUESTION = "What are the first names of the \nthree other members in your party?";

        /// <summary>
        ///     Defines the current game mode the inheriting class is going to take responsibility for when attached to the
        ///     simulation.
        /// </summary>
        public override Mode Mode
        {
            get { return Mode.MainMenu; }
        }

        /// <summary>
        ///     Called after the mode has been added to list of modes and made active.
        /// </summary>
        public override void OnModePostCreate()
        {
            var headerText = new StringBuilder();
            headerText.Append($"{Environment.NewLine}The Oregon Trail{Environment.NewLine}{Environment.NewLine}");
            headerText.Append("You may:");
            MenuHeader = headerText.ToString();

            AddCommand(TravelTheTrail, MainMenuCommands.TravelTheTrail);
            AddCommand(LearnAboutTrail, MainMenuCommands.LearnAboutTheTrail);
            AddCommand(SeeTopTen, MainMenuCommands.SeeTheOregonTopTen);
            AddCommand(ChooseManagementOptions, MainMenuCommands.ChooseManagementOptions);
            AddCommand(CloseSimulation, MainMenuCommands.CloseSimulation);
        }

        /// <summary>
        ///     Does exactly what it says on the tin, closes the simulation and releases all memory.
        /// </summary>
        private static void CloseSimulation()
        {
            GameSimulationApp.Instance.Destroy();
        }

        /// <summary>
        ///     Glorified options menu, used to clear top ten, tombstone messages, and saved games.
        /// </summary>
        private static void ChooseManagementOptions()
        {
            GameSimulationApp.Instance.ModeManager.AddMode(Mode.Options);
        }

        /// <summary>
        ///     High score list, defaults to hard-coded values if no custom ones present.
        /// </summary>
        private void SeeTopTen()
        {
            //CurrentState = new CurrentTopTenState(this, MainMenuInfo);
            SetState(typeof (CurrentTopTenState));
        }

        /// <summary>
        ///     Instruction manual that explains how the game works and what is expected of the player.
        /// </summary>
        private void LearnAboutTrail()
        {
            //CurrentState = new InstructionsState(this, MainMenuInfo);
            SetState(typeof (InstructionsState));
        }

        /// <summary>
        ///     Start with choosing profession in the new game mode, the others are chained together after this one.
        /// </summary>
        private void TravelTheTrail()
        {
            //CurrentState = new SelectProfessionState(this, MainMenuInfo);
            SetState(typeof (SelectProfessionState));
        }
    }
}