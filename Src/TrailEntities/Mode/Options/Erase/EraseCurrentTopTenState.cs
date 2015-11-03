using System;
using System.Text;

namespace TrailEntities
{
    /// <summary>
    ///     Confirm the player wishes to the destroy the current top ten list and reset it back to the hard-coded default
    ///     values.
    /// </summary>
    public sealed class EraseCurrentTopTenState : ModeState<OptionInfo>
    {
        /// <summary>
        ///     This constructor will be used by the other one
        /// </summary>
        public EraseCurrentTopTenState(IMode gameMode, OptionInfo userData) : base(gameMode, userData)
        {
        }

        /// <summary>
        ///     Returns a text only representation of the current game mode state. Could be a statement, information, question
        ///     waiting input, etc.
        /// </summary>
        public override string GetStateTUI()
        {
            var eraseTopTen = new StringBuilder();


            eraseTopTen.Append("Press ENTER KEY to continue.\n");
            return eraseTopTen.ToString();
        }

        /// <summary>
        ///     Determines if user input is currently allowed to be typed and filled into the input buffer.
        /// </summary>
        /// <remarks>Default is FALSE. Setting to TRUE allows characters and input buffer to be read when submitted.</remarks>
        public override bool AcceptsInput
        {
            get { return false; }
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