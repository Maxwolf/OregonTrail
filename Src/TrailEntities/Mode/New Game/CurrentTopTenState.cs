using System;
using System.Text;

namespace TrailEntities
{
    /// <summary>
    ///     References the top ten players in regards to final score they earned at the end of the game, this list is by
    ///     default hard-coded by players have the chance to save their own scores to the list if they beat the default values.
    /// </summary>
    public sealed class CurrentTopTenState : ModeState<NewGameInfo>
    {
        /// <summary>
        ///     This constructor will be used by the other one
        /// </summary>
        public CurrentTopTenState(IMode gameMode, NewGameInfo userData) : base(gameMode, userData)
        {
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
        ///     Returns a text only representation of the current game mode state. Could be a statement, information, question
        ///     waiting input, etc.
        /// </summary>
        public override string GetStateTUI()
        {
            var currentTopTen = new StringBuilder();

            // TODO: Load custom list from JSON with user high scores altered from defaults.
            //// Create text table representation of current high score list.
            //var table = users.ToStringTable(
            //u => u.FirstName,
            //u => u.LastName,

            //u => u.DateTime,
            //u => u.NullableDateTime,

            //u => u.IntValue,
            //u => u.NullableIntValue);

            //currentTopTen.Append(table);

            currentTopTen.Append("Press ENTER KEY to continue.\n");
            return currentTopTen.ToString();
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