using System;
using System.Text;
using TrailEntities.Mode;

namespace TrailEntities.Game
{
    /// <summary>
    ///     Erases all the saved JSON tombstone epitaphs on the disk so other players will not encounter them, new ones can be
    ///     created then.
    /// </summary>
    public sealed class EraseTombstoneState : ModeState<OptionInfo>
    {
        /// <summary>
        ///     This constructor will be used by the other one
        /// </summary>
        public EraseTombstoneState(IMode gameMode, OptionInfo userData) : base(gameMode, userData)
        {
        }

        /// <summary>
        ///     Returns a text only representation of the current game mode state. Could be a statement, information, question
        ///     waiting input, etc.
        /// </summary>
        public override string OnRenderState()
        {
            var eraseEpitaphs = new StringBuilder();

            // Text above the table to declare what this state is.
            eraseEpitaphs.Append(
                $"{Environment.NewLine}Erase tombstone messages{Environment.NewLine}{Environment.NewLine}");

            // Tell the user how tombstones work before destroying them.
            eraseEpitaphs.Append($"There may be one tombstone on{Environment.NewLine}");
            eraseEpitaphs.Append($"the first half of the trail and{Environment.NewLine}");
            eraseEpitaphs.Append($"one tombstone on the second{Environment.NewLine}");
            eraseEpitaphs.Append($"half. If you erase the{Environment.NewLine}");
            eraseEpitaphs.Append($"tombstone messages, they will{Environment.NewLine}");
            eraseEpitaphs.Append($"not be replaced until team{Environment.NewLine}");
            eraseEpitaphs.Append($"leaders die along the trail.{Environment.NewLine}{Environment.NewLine}");

            eraseEpitaphs.Append("Do you want to do this? Y/N");
            return eraseEpitaphs.ToString();
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
                    // TODO: Clear tombstone message list, delete file that was loaded from disk...
                    ParentMode.CurrentState = null;
                    break;
                default:
                    ParentMode.CurrentState = null;
                    break;
            }
        }
    }
}