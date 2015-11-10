using System.Text;
using TrailEntities.Simulation.Mode;

namespace TrailEntities.Game
{
    /// <summary>
    ///     Explains to the user how many monies and days they will be charged to cross the river using the ferry and to
    ///     confirm by saying yes. At this point the simulation will check if they have enough money or not and jump to the
    ///     next state accordingly.
    /// </summary>
    public sealed class UseFerryConfirmState : ModeState<RiverCrossInfo>
    {
        private StringBuilder _prompt;
        private bool _seenPrompt;

        /// <summary>
        ///     This constructor will be used by the other one
        /// </summary>
        public UseFerryConfirmState(IModeProduct gameMode, RiverCrossInfo userData) : base(gameMode, userData)
        {
            _prompt = new StringBuilder();
            _prompt.Append("");
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
            ParentMode.CurrentState = null;
        }
    }
}