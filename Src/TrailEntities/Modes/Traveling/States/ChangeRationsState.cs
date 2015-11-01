using System;
using TrailCommon;

namespace TrailEntities
{
    /// <summary>
    ///     Allows the player to change the amount of food their party members will have access to in a given day, the purpose
    ///     of which is to limit the amount they take in to slow the loss of food per pound. This has many affects on the
    ///     simulation such as disease, chance for breaking body parts, and or complete death from starvation.
    /// </summary>
    public sealed class ChangeRationsState : ModeState<TravelInfo>
    {
        /// <summary>
        ///     This constructor will be used by the other one
        /// </summary>
        public ChangeRationsState(IMode gameMode, TravelInfo userData) : base(gameMode, userData)
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