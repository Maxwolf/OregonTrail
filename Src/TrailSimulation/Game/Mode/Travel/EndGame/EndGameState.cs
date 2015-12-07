using System.Text;
using TrailSimulation.Core;

namespace TrailSimulation.Game
{
    /// <summary>
    ///     Attached when the party leader dies, or the vehicle reaches the end of the trail.
    /// </summary>
    [RequiredMode(Mode.Travel)]
    public sealed class EndGameState : StateProduct<TravelInfo>
    {
        /// <summary>
        ///     Holds reference to end game text that will be shown to the user.
        /// </summary>
        private StringBuilder _gameOver;

        /// <summary>
        ///     This constructor will be used by the other one
        /// </summary>
        public EndGameState(IModeProduct gameMode) : base(gameMode)
        {
            _gameOver = new StringBuilder();
        }

        /// <summary>
        ///     Returns a text only representation of the current game mode state. Could be a statement, information, question
        ///     waiting input, etc.
        /// </summary>
        public override string OnRenderState()
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        ///     Fired when the game mode current state is not null and input buffer does not match any known command.
        /// </summary>
        /// <param name="input">Contents of the input buffer which didn't match any known command in parent game mode.</param>
        public override void OnInputBufferReturned(string input)
        {
            throw new System.NotImplementedException();
        }
    }
}