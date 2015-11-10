using System;
using System.Text;
using TrailEntities.Mode;

namespace TrailEntities.Game
{
    public sealed class CaulkRiverHelpState : ModeState<RiverCrossInfo>
    {
        private bool _seenPrompt;
        private StringBuilder _prompt;

        /// <summary>
        ///     This constructor will be used by the other one
        /// </summary>
        public CaulkRiverHelpState(IMode gameMode, RiverCrossInfo userData) : base(gameMode, userData)
        {
            _prompt = new StringBuilder();
            _prompt.Append("");
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
            return _prompt.ToString();
        }

        /// <summary>
        ///     Fired when the game mode current state is not null and input buffer does not match any known command.
        /// </summary>
        /// <param name="input">Contents of the input buffer which didn't match any known command in parent game mode.</param>
        public override void OnInputBufferReturned(string input)
        {
            if (_seenPrompt)
                return;

            _seenPrompt = true;
            ParentMode.CurrentState = new FerryHelpState(ParentMode, UserData);
        }
    }
}