using System;
using System.Collections.Generic;
using System.Text;
using TrailSimulation.Core;

namespace TrailSimulation.Game
{
    /// <summary>
    ///     Shows the player hard-coded top ten list as it is known internally in static list.
    /// </summary>
    [ParentWindow(GameWindow.MainMenu)]
    public sealed class OriginalTopTen : InputForm<NewGameInfo>
    {
        /// <summary>
        ///     This constructor will be used by the other one
        /// </summary>
        public OriginalTopTen(IWindow window) : base(window)
        {
        }

        /// <summary>
        ///     Original high scores from Apple II version of the game.
        /// </summary>
        public static IEnumerable<Highscore> DefaultTopTen
        {
            get
            {
                return new List<Highscore>
                {
                    new Highscore("Stephen Meek", 7650, Performance.TrailGuide),
                    new Highscore("Celinda Hines", 5694, Performance.Adventurer),
                    new Highscore("Andrew Sublette", 4138, Performance.Adventurer),
                    new Highscore("David Hastings", 2945, Performance.Greenhorn),
                    new Highscore("Ezra Meeker", 2052, Performance.Greenhorn),
                    new Highscore("Willian Vaughn", 1401, Performance.Greenhorn),
                    new Highscore("Mary Bartlett", 937, Performance.Greenhorn),
                    new Highscore("Willian Wiggins", 615, Performance.Greenhorn),
                    new Highscore("Charles Hopper", 396, Performance.Greenhorn),
                    new Highscore("Elijah White", 250, Performance.Greenhorn)
                };
            }
        }

        /// <summary>
        ///     Fired when dialog prompt is attached to active game Windows and would like to have a string returned.
        /// </summary>
        protected override string OnDialogPrompt()
        {
            var sourceTopTen = new StringBuilder();

            // Text above the table to declare what this state is.
            sourceTopTen.Append($"{Environment.NewLine}The Oregon Top Ten{Environment.NewLine}{Environment.NewLine}");

            // Create text table representation of default high score list.
            var table = DefaultTopTen.ToStringTable(
                u => u.Name,
                u => u.Points,
                u => u.Rating);
            sourceTopTen.AppendLine(table);
            return sourceTopTen.ToString();
        }

        /// <summary>
        ///     Fired when the dialog receives favorable input and determines a response based on this. From this method it is
        ///     common to attach another state, or remove the current state based on the response.
        /// </summary>
        /// <param name="reponse">The response the dialog parsed from simulation input buffer.</param>
        protected override void OnDialogResponse(DialogResponse reponse)
        {
            SetForm(typeof (ManagementOptions));
        }
    }
}