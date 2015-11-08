using System;

namespace TrailEntities.Mode
{
    /// <summary>
    ///     Attached if the player decides to rest by the river for a specified amount of time. Events will still tick here as
    ///     the number of days is moved through.
    /// </summary>
    public sealed class CampByRiverState : ModeState<RiverCrossInfo>
    {
        /// <summary>
        ///     This constructor will be used by the other one
        /// </summary>
        public CampByRiverState(IMode gameMode, RiverCrossInfo userData) : base(gameMode, userData)
        {
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
            throw new NotImplementedException();
        }
    }
}