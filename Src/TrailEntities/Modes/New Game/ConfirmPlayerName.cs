using System;
using TrailCommon;

namespace TrailEntities
{
    public sealed class ConfirmPlayerName : ModeState<NewGameInfo>
    {
        private readonly int _playerNameIndex;

        /// <summary>
        ///     This constructor will be used by the other one
        /// </summary>
        public ConfirmPlayerName(int playerNameIndex, IMode gameMode, NewGameInfo userData) : base(gameMode, userData)
        {
            _playerNameIndex = playerNameIndex;
        }

        /// <summary>
        ///     Returns a text only representation of the current game mode state. Could be a statement, information, question
        ///     waiting input, etc.
        /// </summary>
        public override string GetStateTUI()
        {
            return $"You entered {UserData.PlayerNames[_playerNameIndex]} for " +
                   $"player slot {_playerNameIndex + 1}.\n Does this look correct? Y/N";
        }

        /// <summary>
        ///     Fired when the game mode current state is not null and input buffer does not match any known command.
        /// </summary>
        /// <param name="input">Contents of the input buffer which didn't match any known command in parent game mode.</param>
        public override void OnInputBufferReturned(string input)
        {
            // ReSharper disable once ConvertIfStatementToSwitchStatement
            if (input.ToUpperInvariant() == "Y")
            {
                Mode.CurrentState = new ChooseProfessionState(Mode, UserData);
            }
            else if (input.ToUpperInvariant() == "N")
            {
                // If the user doesn't like what they entered then restart entry for that name.
                // ReSharper disable once SwitchStatementMissingSomeCases
                switch (_playerNameIndex)
                {
                    case 0:
                        Mode.CurrentState = new InputPlayerNameState(0, "Party leader name?", Mode, UserData);
                        break;
                    case 1:
                        Mode.CurrentState = new InputPlayerNameState(1, "Party member two name?", Mode, UserData);
                        break;
                    case 2:
                        Mode.CurrentState = new InputPlayerNameState(2, "Party member three name ?", Mode, UserData);
                        break;
                    case 3:
                        Mode.CurrentState = new InputPlayerNameState(3, "Party member four name?", Mode, UserData);
                        break;
                    default:
                        throw new IndexOutOfRangeException(
                            "Tried to return to player name input for index that exceeds design specifications!");
                }
            }
        }
    }
}