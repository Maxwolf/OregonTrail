using System;
using System.Text;

namespace TrailEntities
{
    /// <summary>
    ///     Shows basic information about how the game works, how traveling works, rules for winning and losing.
    /// </summary>
    public sealed class InstructionsState : ModeState<MainMenuInfo>
    {
        /// <summary>
        ///     Determines if the player has read the instructions.
        /// </summary>
        private bool _hasReadInstructions;

        /// <summary>
        ///     Information about how to play the game is built in constructor and then referenced once.
        /// </summary>
        private StringBuilder _playInfo;

        /// <summary>
        ///     This constructor will be used by the other one
        /// </summary>
        public InstructionsState(IMode gameMode, MainMenuInfo userData) : base(gameMode, userData)
        {
            _playInfo = new StringBuilder();
            _playInfo.Append($"Your journey over the Oregon Trail takes place in 1847. Start{Environment.NewLine}");
            _playInfo.Append($"ing in Independence, Missouri, you plan to take your family of{Environment.NewLine}");
            _playInfo.Append($"five over {GameSimApp.TRAIL_LENGTH} tough miles to Oregon City.{Environment.NewLine}{Environment.NewLine}");

            _playInfo.Append($"Having saved for the trip, you bought a wagon and{Environment.NewLine}");
            _playInfo.Append($"now have to purchase the following items:{Environment.NewLine}{Environment.NewLine}");

            _playInfo.Append($" * Oxen (spending more will buy you a larger and better team which{Environment.NewLine}");
            _playInfo.Append(
                $" will be faster so you'll be on the trail for less time){Environment.NewLine}{Environment.NewLine}");

            _playInfo.Append(
                $" * Food (you'll need ample food to keep up your strength and health){Environment.NewLine}{Environment.NewLine}");

            _playInfo.Append($" * Ammunition ($1 buys a belt of 50 bullets. You'll need ammo for{Environment.NewLine}");
            _playInfo.Append(
                $" hunting and for fighting off attacks by bandits and animals){Environment.NewLine}{Environment.NewLine}");

            _playInfo.Append($" * Clothing (you'll need warm clothes, especially when you hit the{Environment.NewLine}");
            _playInfo.Append($" snow and freezing weather in the mountains){Environment.NewLine}{Environment.NewLine}");

            _playInfo.Append(
                $" * Other supplies (includes medicine, first-aid supplies, tools, and{Environment.NewLine}");
            _playInfo.Append($" wagon parts for unexpected emergencies){Environment.NewLine}{Environment.NewLine}");

            // Wait for user input...
            _playInfo.Append(GameSimApp.PRESS_ENTER);
        }

        /// <summary>
        ///     Determines if user input is currently allowed to be typed and filled into the input buffer.
        /// </summary>
        /// <remarks>Default is FALSE. Setting to TRUE allows characters and input buffer to be read when submitted.</remarks>
        public override bool AcceptsInput
        {
            get { return false; }
        }

        /// <summary>
        ///     Returns a text only representation of the current game mode state. Could be a statement, information, question
        ///     waiting input, etc.
        /// </summary>
        public override string OnRenderState()
        {
            return _playInfo.ToString();
        }

        /// <summary>
        ///     Fired when the game mode current state is not null and input buffer does not match any known command.
        /// </summary>
        /// <param name="input">Contents of the input buffer which didn't match any known command in parent game mode.</param>
        public override void OnInputBufferReturned(string input)
        {
            if (_hasReadInstructions)
                return;

            _hasReadInstructions = true;
            ParentMode.CurrentState = null;
        }
    }
}