using System;
using TrailCommon;

namespace TrailEntities
{
    /// <summary>
    ///     Confirms if a particular index in the user data player name list is what the player desires. Since we offer them
    ///     the ability to input this data we also offer them a chance to confirm it, if they say no it will bounce the state
    ///     back to the name entry for that index.
    /// </summary>
    public sealed class ConfirmPlayerNameState : ModeState<NewGameInfo>
    {
        private readonly int _playerNameIndex;

        /// <summary>
        ///     This constructor will be used by the other one
        /// </summary>
        public ConfirmPlayerNameState(int playerNameIndex, IMode gameMode, NewGameInfo userData)
            : base(gameMode, userData)
        {
            _playerNameIndex = playerNameIndex;
        }

        /// <summary>
        ///     Determines if user input is currently allowed to be typed and filled into the input buffer.
        /// </summary>
        /// <remarks>Default is FALSE. Setting to TRUE allows characters and input buffer to be read when submitted.</remarks>
        public override bool AcceptsInput
        {
            get { return true; }
        }

        /// <summary>
        ///     Returns a text only representation of the current game mode state. Could be a statement, information, question
        ///     waiting input, etc.
        /// </summary>
        public override string GetStateTUI()
        {
            return $"You entered {UserData.PlayerNames[_playerNameIndex]} for " +
                   $"player slot {_playerNameIndex + 1}.\n Is this correct? Y/N";
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
                    if (_playerNameIndex < 3)
                    {
                        // Depending on what index we are confirming for we might move onto party names confirmation, or just the next name in the list.
                        SetNameSelectionStateByIndex(false);
                    }
                    else if (_playerNameIndex >= 3)
                    {
                        // Instead of throwing an exception when out of bounds we just goto confirm the party since you must have filled them all out.
                        ParentMode.CurrentState = new ConfirmGroupNamesState(ParentMode, UserData);
                    }
                    break;
                default:
                    SetNameSelectionStateByIndex(true);
                    break;
            }
        }

        /// <summary>
        ///     Allows the user to change what they entered then restart entry for that name.
        /// </summary>
        /// <param name="retrying">Determines if this selection state change is a retry or progression to next state.</param>
        private void SetNameSelectionStateByIndex(bool retrying)
        {
            // Add one to the selection state if we are advancing the state forward and not retrying.
            var correctedPlayerNameIndex = _playerNameIndex;
            if (!retrying)
            {
                correctedPlayerNameIndex++;
            }
            else
            {
                // Remove the previous entry if we are retrying.
                UserData.PlayerNames.RemoveAt(correctedPlayerNameIndex);
            }

            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (correctedPlayerNameIndex)
            {
                case 0:
                    ParentMode.CurrentState = new InputPlayerNameState(correctedPlayerNameIndex, "Party leader name?", ParentMode,
                        UserData);
                    break;
                case 1:
                    ParentMode.CurrentState = new InputPlayerNameState(correctedPlayerNameIndex, "Party member two name?",
                        ParentMode, UserData);
                    break;
                case 2:
                    ParentMode.CurrentState = new InputPlayerNameState(correctedPlayerNameIndex, "Party member three name ?",
                        ParentMode, UserData);
                    break;
                case 3:
                    ParentMode.CurrentState = new InputPlayerNameState(correctedPlayerNameIndex, "Party member four name?",
                        ParentMode, UserData);
                    break;
                default:
                    if (!retrying)
                    {
                        ParentMode.CurrentState = new ConfirmGroupNamesState(ParentMode, UserData);
                    }
                    else
                    {
                        throw new ArgumentException(
                            "Attempted to set game mode state party by index but was set to retry and higher than total player count!");
                    }
                    break;
            }
        }
    }
}