using System;

namespace TrailEntities
{
    /// <summary>
    ///     Used when the party leader dies, no matter what happens this prevents the rest of the game from moving forward and
    ///     everybody dies. This state offers up the chance for the person to leave a personal epitaph of their existence as a
    ///     warning or really whatever. Part of the fun is not knowing what they will say!
    /// </summary>
    public sealed class TombstoneState : ModeState<TravelInfo>
    {
        /// <summary>
        ///     This constructor will be used by the other one
        /// </summary>
        public TombstoneState(IMode gameMode, TravelInfo userData) : base(gameMode, userData)
        {
        }

        /// <summary>
        ///     Returns a text only representation of the current game mode state. Could be a statement, information, question
        ///     waiting input, etc.
        /// </summary>
        public override string GetStateTUI()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Fired when the game mode current state is not null and input buffer does not match any known command.
        /// </summary>
        /// <param name="input">Contents of the input buffer which didn't match any known command in parent game mode.</param>
        public override void OnInputBufferReturned(string input)
        {
            throw new NotImplementedException();
        }
    }
}