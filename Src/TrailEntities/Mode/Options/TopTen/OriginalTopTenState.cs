using System;
using System.Text;

namespace TrailEntities
{
    /// <summary>
    ///     Shows the player hard-coded top ten list as it is known internally in static list.
    /// </summary>
    public sealed class OriginalTopTenState : ModeState<OptionInfo>
    {
        /// <summary>
        ///     This constructor will be used by the other one
        /// </summary>
        public OriginalTopTenState(IMode gameMode, OptionInfo userData) : base(gameMode, userData)
        {
        }

        /// <summary>
        ///     Returns a text only representation of the current game mode state. Could be a statement, information, question
        ///     waiting input, etc.
        /// </summary>
        public override string GetStateTUI()
        {
            var sourceTopTen = new StringBuilder();


            sourceTopTen.Append("Press ENTER KEY to continue.\n");
            return sourceTopTen.ToString();
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