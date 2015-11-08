using System;
using System.Text;

namespace TrailEntities.Mode
{
    /// <summary>
    ///     Allows the user to completely configure the simulation before they start off on the trail path. It will offer up
    ///     ability to choose names, professions, buy initial items, and starting month. The final thing it offers is ability
    ///     to change any of these values before actually starting the game as a final confirmation.
    /// </summary>
    [GameMode(ModeCategory.MainMenu)]
    public sealed class MainMenuMode : GameMode<MainMenuCommands>
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
        ///     Initializes a new instance of the <see cref="T:TrailEntities.Mode.MainMenuMode" /> class.
        /// </summary>
        public MainMenuMode() : base(false)
        {
            // Basic information to start a new simulation.
            MainMenuInfo = new MainMenuInfo();

            var headerText = new StringBuilder();
            headerText.Append($"{Environment.NewLine}The Oregon Trail{Environment.NewLine}{Environment.NewLine}");
            headerText.Append("You may:");
            MenuHeader = headerText.ToString();

            AddCommand(TravelTheTrail, MainMenuCommands.TravelTheTrail, "Travel the trail");
            AddCommand(LearnAboutTrail, MainMenuCommands.LearnAboutTheTrail, "Learn about the trail");
            AddCommand(SeeTopTen, MainMenuCommands.SeeTheOregonTopTen, "See the Oregon Top Ten");
            AddCommand(ChooseManagementOptions, MainMenuCommands.ChooseManagementOptions, "Choose Management Options");
            AddCommand(CloseSimulation, MainMenuCommands.CloseSimulation, "End");
        }

        /// <summary>
        ///     Defines the current game mode the inheriting class is going to take responsibility for when attached to the
        ///     simulation.
        /// </summary>
        public override ModeCategory ModeCategory
        {
            get { return ModeCategory.MainMenu; }
        }

        /// <summary>
        ///     Default values for new game.
        /// </summary>
        private MainMenuInfo MainMenuInfo { get; }

        /// <summary>
        ///     Does exactly what it says on the tin, closes the simulation and releases all memory.
        /// </summary>
        private static void CloseSimulation()
        {
            GameSimApp.Instance.Destroy();
        }

        /// <summary>
        ///     Glorified options menu, used to clear top ten, tombstone messages, and saved games.
        /// </summary>
        private static void ChooseManagementOptions()
        {
            GameSimApp.Instance.AddMode(ModeCategory.Options);
        }

        /// <summary>
        ///     High score list, defaults to hard-coded values if no custom ones present.
        /// </summary>
        private void SeeTopTen()
        {
            CurrentState = new CurrentTopTenState(this, MainMenuInfo);
        }

        /// <summary>
        ///     Instruction manual that explains how the game works and what is expected of the player.
        /// </summary>
        private void LearnAboutTrail()
        {
            CurrentState = new InstructionsState(this, MainMenuInfo);
        }

        /// <summary>
        ///     Start with choosing profession in the new game mode, the others are chained together after this one.
        /// </summary>
        private void TravelTheTrail()
        {
            CurrentState = new SelectProfessionState(this, MainMenuInfo);
        }
    }
}