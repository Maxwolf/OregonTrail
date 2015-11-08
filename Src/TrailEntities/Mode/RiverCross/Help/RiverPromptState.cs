using System;

namespace TrailEntities.Mode
{
    /// <summary>
    ///     Shown to the user the first time they cross a river, this way it can be explained to them they must cross it in
    ///     order to continue and there is no going around. We tell them how deep the water is and how many feed across the
    ///     river is they will need to travel.
    /// </summary>
    public sealed class RiverPromptState : ModeState<RiverCrossInfo>
    {
        /// <summary>
        ///     This constructor will be used by the other one
        /// </summary>
        public RiverPromptState(GameMode gameMode, RiverCrossInfo userData) : base(gameMode, userData)
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