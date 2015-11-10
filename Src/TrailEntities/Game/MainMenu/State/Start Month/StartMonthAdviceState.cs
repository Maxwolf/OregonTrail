using System;
using System.Text;
using TrailEntities.Simulation;
using TrailEntities.Simulation.Mode;

namespace TrailEntities.Game
{
    /// <summary>
    ///     Shows the player information about what the various starting months mean.
    /// </summary>
    public sealed class StartMonthAdviceState : ModeState<MainMenuInfo>
    {
        /// <summary>
        ///     References if the user has pressed any key to get rid of the starting month advice.
        /// </summary>
        private bool _hasShownStartMonthAdvice;

        /// <summary>
        ///     References the string that represents the advice for starting months and what they mean in the simulation.
        /// </summary>
        private StringBuilder _startMonthHelp;

        public StartMonthAdviceState(IModeProduct gameMode, MainMenuInfo userData) : base(gameMode, userData)
        {
            // Have not shown this yet.
            _hasShownStartMonthAdvice = false;

            // Inform the user about a decision they need to make.
            _startMonthHelp = new StringBuilder();
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

            // Wait for user input...
            _startMonthHelp.Append(GameSimulationApp.PRESS_ENTER);
        }

        public override bool AcceptsInput
        {
            get { return false; }
        }

        public override string OnRenderState()
        {
            return _startMonthHelp.ToString();
        }

        public override void OnInputBufferReturned(string input)
        {
            // Ensure this only happens once per instance of advice.
            if (_hasShownStartMonthAdvice)
                return;

            // Return to starting month selection state.
            _hasShownStartMonthAdvice = true;
            ParentMode.CurrentState = new SelectStartingMonthState(ParentMode, UserData);
        }
    }
}