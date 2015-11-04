using System;
using System.Text;

namespace TrailEntities
{
    /// <summary>
    ///     Allows the user to completely configure the simulation before they start off on the trail path. It will offer up
    ///     ability to choose names, professions, buy initial items, and starting month. The final thing it offers is ability
    ///     to change any of these values before actually starting the game as a final confirmation.
    /// </summary>
    public sealed class NewGameMode : GameMode<NewGameCommands>
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
        ///     Initializes a new instance of the <see cref="T:TrailEntities.NewGameMode" /> class.
        /// </summary>
        public NewGameMode() : base(false)
        {
            // Basic information to start a new simulation.
            NewGameInfo = new NewGameInfo();

            var headerText = new StringBuilder();
            headerText.Append($"{Environment.NewLine}The Oregon Trail{Environment.NewLine}{Environment.NewLine}");
            headerText.Append("You may:");
            MenuHeader = headerText.ToString();

            AddCommand(TravelTheTrail, NewGameCommands.TravelTheTrail, "Travel the trail");
            AddCommand(LearnAboutTrail, NewGameCommands.LearnAboutTheTrail, "Learn about the trail");
            AddCommand(SeeTopTen, NewGameCommands.SeeTheOregonTopTen, "See the Oregon Top Ten");
            AddCommand(ChooseManagementOptions, NewGameCommands.ChooseManagementOptions, "Choose Management Options");
            AddCommand(CloseSimulation, NewGameCommands.CloseSimulation, "End");
        }

        /// <summary>
        ///     Defines the current game mode the inheriting class is going to take responsibility for when attached to the
        ///     simulation.
        /// </summary>
        public override ModeType ModeType
        {
            get { return ModeType.NewGame; }
        }

        /// <summary>
        ///     Default values for new game.
        /// </summary>
        private NewGameInfo NewGameInfo { get; }

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
            GameSimulationApp.Instance.AddMode(ModeType.ManagementOptions);
        }

        /// <summary>
        ///     High score list, defaults to hard-coded values if no custom ones present.
        /// </summary>
        private void SeeTopTen()
        {
            CurrentState = new CurrentTopTenState(this, NewGameInfo);
        }

        /// <summary>
        ///     Instruction manual that explains how the game works and what is expected of the player.
        /// </summary>
        private void LearnAboutTrail()
        {
            CurrentState = new InstructionsState(this, NewGameInfo);
        }

        /// <summary>
        ///     Start with choosing profession in the new game mode, the others are chained together after this one.
        /// </summary>
        private void TravelTheTrail()
        {
            CurrentState = new SelectProfessionState(this, NewGameInfo);
        }
    }
}