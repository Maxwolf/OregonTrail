// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProfessionHelp.cs" company="Ron 'Maxwolf' McDowell">
//   ron.mcdowell@gmail.com
// </copyright>
// <summary>
//   Shows information about what the player leader professions mean and how it affects the party, vehicle, game
//   difficulty, and scoring at the end (if they make it).
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Text;
using TrailSimulation.Core;

namespace TrailSimulation.Game
{
    /// <summary>
    ///     Shows information about what the player leader professions mean and how it affects the party, vehicle, game
    ///     difficulty, and scoring at the end (if they make it).
    /// </summary>
    [ParentWindow(GameWindow.MainMenu)]
    public sealed class ProfessionHelp : InputForm<NewGameInfo>
    {
        /// <summary>Initializes a new instance of the <see cref="ProfessionHelp"/> class.</summary>
        /// <param name="window">The window.</param>
        public ProfessionHelp(IWindow window) : base(window)
        {
        }

        /// <summary>
        ///     Fired when dialog prompt is attached to active game Windows and would like to have a string returned.
        /// </summary>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        protected override string OnDialogPrompt()
        {
            // Information about professions and how they work.
            var _job = new StringBuilder();
            _job.Append($"{Environment.NewLine}Traveling to Oregon isn't easy!{Environment.NewLine}");
            _job.Append($"But if you're a banker, you'll{Environment.NewLine}");
            _job.Append($"have more money for supplies{Environment.NewLine}");
            _job.Append($"and services than a carpenter{Environment.NewLine}");
            _job.Append($"or a farmer.{Environment.NewLine}{Environment.NewLine}");
            _job.Append($"However, the harder you have{Environment.NewLine}");
            _job.Append($"to try, the more points you{Environment.NewLine}");
            _job.Append($"deserve! Therefore, the{Environment.NewLine}");
            _job.Append($"farmer earns the greatest{Environment.NewLine}");
            _job.Append($"number of points and the{Environment.NewLine}");
            _job.Append($"banker earns the least.{Environment.NewLine}{Environment.NewLine}");
            return _job.ToString();
        }

        /// <summary>Fired when the dialog receives favorable input and determines a response based on this. From this method it is
        ///     common to attach another state, or remove the current state based on the response.</summary>
        /// <param name="reponse">The response the dialog parsed from simulation input buffer.</param>
        protected override void OnDialogResponse(DialogResponse reponse)
        {
            // parentGameMode.State = new ProfessionSelector(parentGameMode, UserData);
            SetForm(typeof (ProfessionSelector));
        }
    }
}