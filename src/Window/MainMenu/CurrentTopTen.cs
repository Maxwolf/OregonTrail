// Created by Maxwolf (bigmaxwolf.com) 
// Timestamp 01/03/2016@1:50 AM

using System;
using System.Text;
using OregonTrailDotNet.Window.MainMenu.Help;
using WolfCurses.Window;
using WolfCurses.Window.Control;
using WolfCurses.Window.Form;
using WolfCurses.Window.Form.Input;

namespace OregonTrailDotNet.Window.MainMenu
{
    /// <summary>
    ///     References the top ten players in regards to final score they earned at the end of the game, this list is by
    ///     default hard-coded by players have the chance to save their own scores to the list if they beat the default values.
    /// </summary>
    [ParentWindow(typeof(MainMenu))]
    public sealed class CurrentTopTen : InputForm<NewGameInfo>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="CurrentTopTen" /> class.
        ///     This constructor will be used by the other one
        /// </summary>
        /// <param name="window">The window.</param>
        // ReSharper disable once UnusedMember.Global
        public CurrentTopTen(IWindow window) : base(window)
        {
        }

        /// <summary>
        ///     Defines what type of dialog this will act like depending on this enumeration value. Up to implementation to define
        ///     desired behavior.
        /// </summary>
        protected override DialogTypeEnum DialogType => DialogTypeEnum.YesNo;

        /// <summary>
        ///     Fired when dialog prompt is attached to active game Windows and would like to have a string returned.
        /// </summary>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        protected override string OnDialogPrompt()
        {
            var currentTopTen = new StringBuilder();

            // Text above the table to declare what this state is.
            currentTopTen.Append($"{Environment.NewLine}Current Top Ten List{Environment.NewLine}{Environment.NewLine}");

            // Create text table representation of default high score list.
            var table = GameSimulationApp.Instance.Scoring.TopTen.ToStringTable(
                u => u.Name,
                u => u.Points,
                u => u.Rating);
            currentTopTen.AppendLine(table);

            // For anybody wondering how high the list can possibly go.
            currentTopTen.Append(
                $"The highest possible score is {Module.Scoring.ScoringModule.MaxPossibleScore:N0}:{Environment.NewLine}");
            currentTopTen.Append($"a farmer party of five arriving in good{Environment.NewLine}");
            currentTopTen.Append($"health with a fully loaded wagon.{Environment.NewLine}{Environment.NewLine}");

            // Question about viewing point distribution information.
            currentTopTen.Append($"Would you like to see how{Environment.NewLine}");
            currentTopTen.Append("points are earned? Y/N");
            return currentTopTen.ToString();
        }

        /// <summary>
        ///     Sets a context-specific input prompt in place of the generic "What is your choice?", then renders as normal.
        /// </summary>
        public override string OnRenderForm()
        {
            ParentWindow.PromptText = "Yes or no?";
            return base.OnRenderForm();
        }

        /// <summary>
        ///     Fired when the dialog receives favorable input and determines a response based on this. From this method it is
        ///     common to attach another state, or remove the current state based on the response.
        /// </summary>
        /// <param name="reponse">The response the dialog parsed from simulation input buffer.</param>
        protected override void OnDialogResponse(DialogResponseEnum reponse)
        {
            switch (reponse)
            {
                case DialogResponseEnum.No:
                    ClearForm();
                    break;
                case DialogResponseEnum.Yes:
                    SetForm(typeof(PointsDistributionHelp));
                    break;
                case DialogResponseEnum.Custom:
                    ClearForm();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(reponse), reponse, null);
            }
        }
    }
}