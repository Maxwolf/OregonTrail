using System;
using System.Text;
using TrailSimulation.Core;

namespace TrailSimulation.Game
{
    /// <summary>
    ///     Shows the player information about what the various starting months mean.
    /// </summary>
    [RequiredMode(Mode.MainMenu)]
    public sealed class StartMonthAdviceState : DialogState<MainMenuInfo>
    {
        public StartMonthAdviceState(IModeProduct gameMode) : base(gameMode)
        {
        }

        /// <summary>
        ///     Fired when dialog prompt is attached to active game mode and would like to have a string returned.
        /// </summary>
        protected override string OnDialogPrompt()
        {
            // Inform the user about a decision they need to make.
            var _startMonthHelp = new StringBuilder();
            _startMonthHelp.Append($"{Environment.NewLine}You attend a public meeting held{Environment.NewLine}");
            _startMonthHelp.Append($"for \"folks with the California -{Environment.NewLine}");
            _startMonthHelp.Append($"Oregon fever.\" You're told:{Environment.NewLine}{Environment.NewLine}");
            _startMonthHelp.Append($"If you leave too early, there{Environment.NewLine}");
            _startMonthHelp.Append($"won't be any grass for your{Environment.NewLine}");
            _startMonthHelp.Append($"oxen to eat. If you leave too{Environment.NewLine}");
            _startMonthHelp.Append($"late, you may not get to Oregon{Environment.NewLine}");
            _startMonthHelp.Append($"before winter comes. If you{Environment.NewLine}");
            _startMonthHelp.Append($"leave at just the right time,{Environment.NewLine}");
            _startMonthHelp.Append($"there will be green grass and{Environment.NewLine}");
            _startMonthHelp.Append($"the weather will still be cool.{Environment.NewLine}{Environment.NewLine}");
            return _startMonthHelp.ToString();
        }

        /// <summary>
        ///     Fired when the dialog receives favorable input and determines a response based on this. From this method it is
        ///     common to attach another state, or remove the current state based on the response.
        /// </summary>
        /// <param name="reponse">The response the dialog parsed from simulation input buffer.</param>
        protected override void OnDialogResponse(DialogResponse reponse)
        {
            //parentGameMode.State = new SelectStartingMonthState(parentGameMode, UserData);
            SetState(typeof (SelectStartingMonthState));
        }
    }
}