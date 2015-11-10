using System;
using System.Text;
using TrailEntities.Simulation;
using TrailEntities.Simulation.Mode;

namespace TrailEntities.Game
{
    /// <summary>
    ///     Shows information about what the different pace settings mean in terms for the simulation and how they will affect
    ///     vehicle, party, and events.
    /// </summary>
    public sealed class PaceAdviceState : ModeState<TravelInfo>
    {
        /// <summary>
        ///     String builder to hold all the information for pace help so we only build it once and then reference it.
        /// </summary>
        private StringBuilder _paceHelp;

        /// <summary>
        ///     Determines if the player is done looking at information about travel pace.
        /// </summary>
        private bool _seenPaceHelp;

        /// <summary>
        ///     This constructor will be used by the other one
        /// </summary>
        public PaceAdviceState(IModeProduct gameMode, TravelInfo userData) : base(gameMode, userData)
        {
            // Steady
            _paceHelp = new StringBuilder();
            _paceHelp.Append($"{Environment.NewLine}steady - You travel about 8 hours a{Environment.NewLine}");
            _paceHelp.Append($"day, taking frequent rests. You take{Environment.NewLine}");
            _paceHelp.Append($"care not to get too tired.{Environment.NewLine}{Environment.NewLine}");

            // Strenuous
            _paceHelp.Append($"strenuous - You travel about 12 hours{Environment.NewLine}");
            _paceHelp.Append($"a day, starting just after sunrise{Environment.NewLine}");
            _paceHelp.Append($"and stopping shortly before sunset.{Environment.NewLine}");
            _paceHelp.Append($"You stop to rest only when necessary.{Environment.NewLine}");
            _paceHelp.Append($"You finish each day feeling very{Environment.NewLine}");
            _paceHelp.Append($"tired.{Environment.NewLine}{Environment.NewLine}");

            // Grueling
            _paceHelp.Append($"grueling - You travel about 16 hours{Environment.NewLine}");
            _paceHelp.Append($"a day, starting before sunrise and{Environment.NewLine}");
            _paceHelp.Append($"continuing until dark. You almost{Environment.NewLine}");
            _paceHelp.Append($"never stop to rest. You do not get{Environment.NewLine}");
            _paceHelp.Append($"enough sleep at night. You finish{Environment.NewLine}");
            _paceHelp.Append($"each day feeling absolutely{Environment.NewLine}");
            _paceHelp.Append($"exhausted, and your health suffers.{Environment.NewLine}{Environment.NewLine}");

            // Wait for user input...
            _paceHelp.Append(GameSimulationApp.PRESS_ENTER);
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
            return _paceHelp.ToString();
        }

        /// <summary>
        ///     Fired when the game mode current state is not null and input buffer does not match any known command.
        /// </summary>
        /// <param name="input">Contents of the input buffer which didn't match any known command in parent game mode.</param>
        public override void OnInputBufferReturned(string input)
        {
            if (_seenPaceHelp)
                return;

            // Go back to the travel menu.
            _seenPaceHelp = true;
            ParentMode.CurrentState = new ChangePaceState(ParentMode, UserData);
        }
    }
}