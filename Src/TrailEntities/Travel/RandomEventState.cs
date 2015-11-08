using System;
using TrailEntities.Mode;

namespace TrailEntities.Travel
{
    /// <summary>
    ///     Used when the event director fires event that game simulation subscribes to which passes along events that should
    ///     be triggered when they occur so this state can be attached to the travel mode and also have the event data passed
    ///     into it so it may be executed and data shown in text user interface for this state.
    /// </summary>
    public sealed class RandomEventState : ModeState<TravelInfo>
    {
        /// <summary>
        ///     This constructor will be used by the other one
        /// </summary>
        public RandomEventState(IMode gameMode, TravelInfo userData) : base(gameMode, userData)
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