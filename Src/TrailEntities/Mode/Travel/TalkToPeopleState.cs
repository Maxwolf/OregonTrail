using System;

namespace TrailEntities.Mode
{
    /// <summary>
    ///     Attaches a game state that will loop through random advice that is associated with the given point of interest.
    ///     This is not a huge list and players will eventually see the same advice if they keep coming back, only one piece of
    ///     advice should be shown and one day will advance in the simulation to prevent the player from just spamming it.
    /// </summary>
    public sealed class TalkToPeopleState : ModeState<TravelInfo>
    {
        /// <summary>
        ///     This constructor will be used by the other one
        /// </summary>
        public TalkToPeopleState(IMode gameMode, TravelInfo userData) : base(gameMode, userData)
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