using System;
using System.Text;
using TrailCommon;

namespace TrailEntities
{
    public sealed class ConfirmPartyState : ModeState<NewGameInfo>
    {
        /// <summary>
        ///     This constructor will create new state taking values from old state
        /// </summary>
        public ConfirmPartyState(ModeState<NewGameInfo> state) : base(state)
        {
        }

        /// <summary>
        ///     This constructor will be used by the other one
        /// </summary>
        public ConfirmPartyState(IMode gameMode, NewGameInfo userData) : base(gameMode, userData)
        {
        }

        /// <summary>
        ///     Forces the current game mode state to update itself, this typically results in moving to the next state.
        /// </summary>
        public override void TickState()
        {
            // Move along, nothing to see here...
        }

        /// <summary>
        ///     Returns a text only representation of the current game mode state. Could be a statement, information, question
        ///     waiting input, etc.
        /// </summary>
        public override string GetStateTUI()
        {
            var confirmPartyText = new StringBuilder();
            confirmPartyText.Append("Does this look correct? Y/N");
            confirmPartyText.Append("Your Party Members:");
            var crewNumber = 1;
            foreach (var name in UserData.PlayerNames)
            {
                var isLeader = UserData.PlayerNames.IndexOf(name) == 0;
                if (isLeader)
                {
                    confirmPartyText.Append(crewNumber + ")." + name + " (leader)");
                }
                else
                {
                    confirmPartyText.Append(crewNumber + ")." + name);
                }
                crewNumber++;
            }

            return confirmPartyText.ToString();
        }

        /// <summary>
        ///     Fired when the game mode current state is not null and input buffer does not match any known command.
        /// </summary>
        /// <param name="input">Contents of the input buffer which didn't match any known command in parent game mode.</param>
        public override void OnInputBufferReturned(string input)
        {
            // Keep this state active until we have four names in the player list.
            if (UserData.PlayerNames.Count <= 4)
            {
                // Depending on which slot change the name of prefix we ask.
                switch (UserData.PlayerNames.Count)
                {
                    case 1:
                        UserData.PlayerNames[0] = input;
                        break;
                    case 2:
                        UserData.PlayerNames[1] = input;
                        break;
                    case 3:
                        UserData.PlayerNames[2] = input;
                        break;
                    case 4:
                        UserData.PlayerNames[3] = input;
                        break;
                    default:
                        throw new IndexOutOfRangeException("Attempted to tick choose name state out of bounds!");
                }
            }
            else if (UserData.PlayerNames.Count > 4)
            {

            }
        }
    }
}