// Created by Maxwolf (bigmaxwolf.com) 
// Timestamp 01/03/2016@1:50 AM

using System;
using System.Text;
using OregonTrailDotNet.Presentation;
using OregonTrailDotNet.Presentation.Audio;
using OregonTrailDotNet.Window.MainMenu.Help;
using OregonTrailDotNet.Window.MainMenu.Options;
using OregonTrailDotNet.Window.MainMenu.Profession;
using WolfCurses;
using WolfCurses.Core;
using WolfCurses.Window;

namespace OregonTrailDotNet.Window.MainMenu
{
    /// <summary>
    ///     Allows the configuration of party names, player profession, and purchasing initial items for trip.
    /// </summary>
    public sealed class MainMenu : Window<MainMenuCommandsEnum, NewGameInfo>
    {
        /// <summary>
        ///     Asked for the first party member.
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public const string LEADER_QUESTION = "What is the first name of the wagon leader?";

        /// <summary>
        ///     Asked for every other party member name we want to collect.
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public static readonly string MEMBERS_QUESTION =
            $"What are the first names of the{Environment.NewLine}four other members in your party?";

        /// <summary>
        ///     Initializes a new instance of the <see cref="Window{TCommands,TData}" /> class.
        /// </summary>
        /// <param name="simUnit">Core simulation which is controlling the form factory.</param>
        // ReSharper disable once UnusedMember.Global
        public MainMenu(SimulationApp simUnit) : base(simUnit)
        {
        }

        /// <summary>
        ///     Resets the input prompt so a context-specific prompt set by one form does not leak into the next,
        ///     and rebuilds the header so the title banner is re-measured for the console whenever a form clears
        ///     back to the menu.
        /// </summary>
        protected override void OnFormChange()
        {
            base.OnFormChange();
            PromptText = SceneGraph.PROMPT_TEXT_DEFAULT;
            BuildHeader();
        }

        /// <summary>
        ///     Called after the Windows has been added to list of modes and made active.
        /// </summary>
        public override void OnWindowPostCreate()
        {
            BuildHeader();

            AddCommand(TravelTheTrail, MainMenuCommandsEnum.TravelTheTrail);
            AddCommand(LearnAboutTrail, MainMenuCommandsEnum.LearnAboutTheTrail);
            AddCommand(SeeTopTen, MainMenuCommandsEnum.SeeTheOregonTopTen);
            AddCommand(ChooseManagementOptions, MainMenuCommandsEnum.ChooseManagementOptions);
            AddCommand(CloseSimulation, MainMenuCommandsEnum.CloseSimulation);

            // The original's "Turn sound off", offered only where sound exists: headless hosts run without the
            // audio stack and their menu text must stay exactly as the bot has always read it.
            if (GameSimulationApp.PresentationEnabled)
                AddCommand(ToggleSound, MainMenuCommandsEnum.ToggleSound);
        }

        /// <summary>
        ///     Flips the one process-wide mute and rebuilds the header so the menu reads the new state back —
        ///     the original relabelled its menu entry, but command labels are fixed, so the masthead answers.
        /// </summary>
        private void ToggleSound()
        {
            Music.ToggleMute();
            BuildHeader();
        }

        /// <summary>
        ///     The menu's masthead. The original draws its title lettering and scroll flourish over the menu; with
        ///     presentation on the same picture replaces the plain text title (headless hosts keep the text).
        /// </summary>
        private void BuildHeader()
        {
            var headerText = new StringBuilder();
            if (GameSimulationApp.PresentationEnabled)
            {
                headerText.Append($"{Banners.Title(reservedRows: 12)}{Environment.NewLine}");
                if (Music.Muted)
                    headerText.Append($"(sound is off){Environment.NewLine}{Environment.NewLine}");
            }
            else
            {
                headerText.Append($"{Environment.NewLine}The Oregon Trail{Environment.NewLine}{Environment.NewLine}");
            }

            headerText.Append("You may:");
            MenuHeader = headerText.ToString();
        }

        /// <summary>
        ///     Does exactly what it says on the tin, closes the simulation and releases all memory.
        /// </summary>
        private static void CloseSimulation()
        {
            GameSimulationApp.Instance.Destroy();
        }

        /// <summary>
        ///     Glorified options menu, used to clear top ten, Tombstone messages, and saved games.
        /// </summary>
        private void ChooseManagementOptions()
        {
            SetForm(typeof(ManagementOptions));
        }

        /// <summary>
        ///     High score list, defaults to hard-coded values if no custom ones present.
        /// </summary>
        private void SeeTopTen()
        {
            SetForm(typeof(CurrentTopTen));
        }

        /// <summary>
        ///     Instruction manual that explains how the game works and what is expected of the player.
        /// </summary>
        private void LearnAboutTrail()
        {
            SetForm(typeof(RulesHelp));
        }

        /// <summary>
        ///     Start with choosing profession in the new game Windows, the others are chained together after this one.
        /// </summary>
        private void TravelTheTrail()
        {
            SetForm(typeof(ProfessionSelector));
        }
    }
}