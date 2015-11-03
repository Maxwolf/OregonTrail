using System;

namespace TrailEntities
{
    /// <summary>
    ///     Shows the player their vehicle and list of all the points in the trail they could possibly travel to. It marks the
    ///     spot they are on and all the spots they have visited, shows percentage for completion and some other basic
    ///     statistics about the journey that could only be seen from this state.
    /// </summary>
    public sealed class LookAtMapState : ModeState<TravelInfo>
    {
        /// <summary>
        ///     This constructor will be used by the other one
        /// </summary>
        public LookAtMapState(IMode gameMode, TravelInfo userData) : base(gameMode, userData)
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