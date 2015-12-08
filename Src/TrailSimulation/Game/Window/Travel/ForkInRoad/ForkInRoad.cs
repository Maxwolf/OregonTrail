using System;
using TrailSimulation.Core;

namespace TrailSimulation.Game
{
    /// <summary>
    ///     Defines a location that has the player make a choice about the next location they want to travel to, it is not a
    ///     linear choice and depends on the player telling the simulation which way to fork down the path. The decisions are
    ///     pear shaped in the sense any fork will eventually lead back to the same path.
    /// </summary>
    [ParentWindow(SimulationModule.Travel)]
    public sealed class ForkInRoad : Form<TravelInfo>
    {
        /// <summary>
        ///     This constructor will be used by the other one
        /// </summary>
        public ForkInRoad(IWindow gameMode) : base(gameMode)
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
        /// <param name="input">Contents of the input buffer which didn't match any known command in parent game mode.</param>
        public override void OnInputBufferReturned(string input)
        {
            throw new NotImplementedException();
        }
    }
}