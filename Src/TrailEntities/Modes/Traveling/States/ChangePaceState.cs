using System;
using TrailCommon;

namespace TrailEntities
{
    /// <summary>
    ///     Allows the player to alter how many 'miles' their vehicle will attempt to travel in a given day, this also changes
    ///     the rate at which random events that are considered bad will occur along with other factors in the simulation such
    ///     as making players more susceptible to disease and also making them hungry more often.
    /// </summary>
    public sealed class ChangePaceState : ModeState<TravelInfo>
    {
        /// <summary>
        ///     This constructor will be used by the other one
        /// </summary>
        public ChangePaceState(IMode gameMode, TravelInfo userData) : base(gameMode, userData)
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