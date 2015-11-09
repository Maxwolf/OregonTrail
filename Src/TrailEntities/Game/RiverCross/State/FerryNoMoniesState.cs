using System;
using TrailEntities.Mode;

namespace TrailEntities.Game.RiverCross
{
    /// <summary>
    ///     Attached when the user attempts to cross the river using the ferry, confirms they would like to but does not have
    ///     enough money at this point this state will be attached and explain to the user they cannot use the ferry and must
    ///     pick one of the other two options.
    /// </summary>
    public sealed class FerryNoMoniesState : StateProduct
    {
        /// <summary>
        ///     This constructor will be used by the other one
        /// </summary>
        public FerryNoMoniesState(ModeProduct gameMode, RiverCrossInfo userData) : base(gameMode, userData)
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