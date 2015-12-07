using System;
using TrailSimulation.Core;

namespace TrailSimulation.Game
{
    /// <summary>
    ///     Used when the party leader dies, no matter what happens this prevents the rest of the game from moving forward and
    ///     everybody dies. This state offers up the chance for the person to leave a personal epitaph of their existence as a
    ///     warning or really whatever. Wheel of the fun is not knowing what they will say!
    /// </summary>
    [RequiredWindow(Windows.Travel)]
    public sealed class TombstoneState : Form<TravelInfo>
    {
        /// <summary>
        ///     This constructor will be used by the other one
        /// </summary>
        public TombstoneState(IWindow gameMode) : base(gameMode)
        {
        }

        /// <summary>
        ///     Returns a text only representation of the current game Windows state. Could be a statement, information, question
        ///     waiting input, etc.
        /// </summary>
        public override string OnRenderForm()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Fired when the game Windows current state is not null and input buffer does not match any known command.
        /// </summary>
        /// <param name="input">Contents of the input buffer which didn't match any known command in parent game Windows.</param>
        public override void OnInputBufferReturned(string input)
        {
            throw new NotImplementedException();
        }
    }
}