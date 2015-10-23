using System;
using TrailCommon;

namespace TrailEntities
{
    public sealed class InputPlayerOneState : InputPlayerNameState
    {
        /// <summary>
        ///     This constructor will create new state taking values from old state
        /// </summary>
        public InputPlayerOneState(ModeState<NewGameInfo> state) : base(state)
        {
        }

        /// <summary>
        ///     This constructor will be used by the other one
        /// </summary>
        public InputPlayerOneState(IMode gameMode, NewGameInfo userData) : base(gameMode, userData)
        {
        }

        /// <summary>
        ///     Forces the current game mode state to update itself, this typically results in moving to the next state.
        /// </summary>
        public override void TickState()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Returns a text only representation of the current game mode state. Could be a statement, information, question
        ///     waiting input, etc.
        /// </summary>
        public override string GetStateTUI()
        {
            return "Party leader name?";
        }

        /// <summary>
        ///     Fired when the game mode current state is not null and input buffer does not match any known command.
        /// </summary>
        /// <param name="input">Contents of the input buffer which didn't match any known command in parent game mode.</param>
        public override void ProcessInput(string input)
        {
            throw new NotImplementedException();
        }
    }
}