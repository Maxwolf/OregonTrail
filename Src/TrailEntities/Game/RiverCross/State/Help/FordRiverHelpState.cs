using System;
using System.Text;
using TrailEntities.Mode;

namespace TrailEntities.Game
{
    /// <summary>
    ///     Information about what fording a river means and how it works for the player vehicle and their party members.
    /// </summary>
    public sealed class FordRiverHelpState : ModeState<RiverCrossInfo>
    {
        private StringBuilder _fordRiverHelp;
        private bool _hasReadFordRiverHelp;

        /// <summary>
        ///     This constructor will be used by the other one
        /// </summary>
        public FordRiverHelpState(IMode gameMode, RiverCrossInfo userData) : base(gameMode, userData)
        {
            _fordRiverHelp = new StringBuilder();
        }

        /// <summary>
        ///     Returns a text only representation of the current game mode state. Could be a statement, information, question
        ///     waiting input, etc.
        /// </summary>
        public override string OnRenderState()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Fired when the game mode current state is not null and input buffer does not match any known command.
        /// </summary>
        /// <param name="input">Contents of the input buffer which didn't match any known command in parent game mode.</param>
        public override void OnInputBufferReturned(string input)
        {
            if (_hasReadFordRiverHelp)
                return;

            _hasReadFordRiverHelp = true;
            ParentMode.CurrentState = new CaulkRiverHelpState(ParentMode, UserData);
        }
    }
}