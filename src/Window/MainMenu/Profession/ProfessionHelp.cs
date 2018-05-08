// Created by Ron 'Maxwolf' McDowell (ron.mcdowell@gmail.com) 
// Timestamp 01/03/2016@1:50 AM

using System;
using System.Text;
using WolfCurses.Window;
using WolfCurses.Window.Form;
using WolfCurses.Window.Form.Input;

namespace OregonTrailDotNet.Window.MainMenu.Profession
{
    /// <summary>
    ///     Shows information about what the player leader professions mean and how it affects the party, vehicle, game
    ///     difficulty, and scoring at the end (if they make it).
    /// </summary>
    [ParentWindow(typeof(MainMenu))]
    public sealed class ProfessionHelp : InputForm<NewGameInfo>
    {
        /// <summary>Initializes a new instance of the <see cref="ProfessionHelp" /> class.</summary>
        /// <param name="window">The window.</param>
        // ReSharper disable once UnusedMember.Global
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
            var job = new StringBuilder();
            job.Append($"{Environment.NewLine}Traveling to Oregon isn't easy!{Environment.NewLine}");
            job.Append($"But if you're a banker, you'll{Environment.NewLine}");
            job.Append($"have more money for supplies{Environment.NewLine}");
            job.Append($"and services than a carpenter{Environment.NewLine}");
            job.Append($"or a farmer.{Environment.NewLine}{Environment.NewLine}");
            job.Append($"However, the harder you have{Environment.NewLine}");
            job.Append($"to try, the more points you{Environment.NewLine}");
            job.Append($"deserve! Therefore, the{Environment.NewLine}");
            job.Append($"farmer earns the greatest{Environment.NewLine}");
            job.Append($"number of points and the{Environment.NewLine}");
            job.Append($"banker earns the least.{Environment.NewLine}{Environment.NewLine}");
            return job.ToString();
        }

        /// <summary>
        ///     Fired when the dialog receives favorable input and determines a response based on this. From this method it is
        ///     common to attach another state, or remove the current state based on the response.
        /// </summary>
        /// <param name="reponse">The response the dialog parsed from simulation input buffer.</param>
        protected override void OnDialogResponse(DialogResponse reponse)
        {
            // parentGameMode.State = new ProfessionSelector(parentGameMode, UserData);
            SetForm(typeof(ProfessionSelector));
        }
    }
}