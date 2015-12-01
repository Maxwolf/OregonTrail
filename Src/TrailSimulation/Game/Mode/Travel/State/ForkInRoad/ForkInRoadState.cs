using System;
using TrailSimulation.Core;

namespace TrailSimulation.Game
{
    /// <summary>
    ///     Defines a location that has the player make a choice about the next location they want to travel to, it is not a
    ///     linear choice and depends on the player telling the simulation which way to fork down the path. The decisions are
    ///     pear shaped in the sense any fork will eventually lead back to the same path.
    /// </summary>
    [RequiredMode(Mode.Travel)]
    public sealed class ForkInRoadState : StateProduct<TravelInfo>
    {
        /// <summary>
        ///     This constructor will be used by the other one
        /// </summary>
        public ForkInRoadState(IModeProduct gameMode) : base(gameMode)
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