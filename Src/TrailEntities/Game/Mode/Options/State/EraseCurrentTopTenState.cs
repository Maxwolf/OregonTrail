using System;
using System.Text;
using TrailEntities.Simulation;

namespace TrailEntities.Game
{
    /// <summary>
    ///     Confirm the player wishes to the destroy the current top ten list and reset it back to the hard-coded default
    ///     values.
    /// </summary>
    public sealed class EraseCurrentTopTenState : StateProduct<OptionInfo>
    {
        /// <summary>
        ///     This constructor will be used by the other one
        /// </summary>
        public EraseCurrentTopTenState(IModeProduct gameMode) : base(gameMode)
        {
        }

        /// <summary>
        ///     Returns a text only representation of the current game mode state. Could be a statement, information, question
        ///     waiting input, etc.
        /// </summary>
        public override string OnRenderState()
        {
            var eraseTopTen = new StringBuilder();

            // Text above the table to declare what this state is.
            eraseTopTen.Append($"{Environment.NewLine}Erase Top Ten list{Environment.NewLine}{Environment.NewLine}");

            // Ask the user question if they really want to remove the top ten list.
            eraseTopTen.Append($"If you erase the current Top Ten{Environment.NewLine}");
            eraseTopTen.Append($"list, the names and scores will be{Environment.NewLine}");
            eraseTopTen.Append($"replaced by those on the original{Environment.NewLine}");
            eraseTopTen.Append($"list.{Environment.NewLine}{Environment.NewLine}");

            // Wait for use input...
            eraseTopTen.Append("Do you want to do this? Y/N");
            return eraseTopTen.ToString();
        }

        /// <summary>
        ///     Fired when the game mode current state is not null and input buffer does not match any known command.
        /// </summary>
        /// <param name="input">Contents of the input buffer which didn't match any known command in parent game mode.</param>
        public override void OnInputBufferReturned(string input)
        {
            switch (input.ToUpperInvariant())
            {
                case "Y":
                    // TODO: Clear the current top ten list, reset to defaults, delete the custom one, re-save with defaults...
                    //parentGameMode.CurrentState = null;
                    ClearState();
                    break;
                default:
                    //parentGameMode.CurrentState = null;
                    ClearState();
                    break;
            }
        }
    }
}