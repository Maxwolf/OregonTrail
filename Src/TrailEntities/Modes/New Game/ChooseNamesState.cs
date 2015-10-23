using System.Text;
using TrailCommon;

namespace TrailEntities
{
    public sealed class ChooseNamesState : ModeState<NewGameInfo>
    {
        /// <summary>
        ///     This constructor will be used by the other one
        /// </summary>
        public ChooseNamesState(IMode gameMode, NewGameInfo userData) : base(gameMode, userData)
        {

        }

        public override void TickState()
        {

        }

        public override string GetStateTUI()
        {
            var nameSelect = new StringBuilder();
            nameSelect.Append("Party leader name?");
            var playerName = GetPlayerName();
            UserData.PlayerNames.Add(playerName);
            nameSelect.Append("Added: " + playerName);

            nameSelect.Append("Party member two name?");
            playerName = GetPlayerName();
            UserData.PlayerNames.Add(playerName);
            nameSelect.Append("Added: " + playerName);

            nameSelect.Append("Party member three name?");
            playerName = GetPlayerName();
            UserData.PlayerNames.Add(playerName);
            nameSelect.Append("Added: " + playerName);

            nameSelect.Append("Party member four name?");
            playerName = GetPlayerName();
            UserData.PlayerNames.Add(playerName);
            nameSelect.Append("Added: " + playerName);

            nameSelect.Append("Your Party Members:");
            var crewNumber = 1;
            foreach (var name in UserData.PlayerNames)
            {
                var isLeader = UserData.PlayerNames.IndexOf(name) == 0;
                if (isLeader)
                {
                    nameSelect.Append(crewNumber + ")." + name + " (leader)");
                }
                else
                {
                    nameSelect.Append(crewNumber + ")." + name);
                }
                crewNumber++;
            }

            return nameSelect.ToString();
        }

        public override void ProcessInput(string input)
        {
            namesCorrectConfirmation.Append("Does this look correct? Y/N");
            if (!string.IsNullOrEmpty(namesCorrectResponse))
            {
                namesCorrectResponse = namesCorrectResponse.Trim().ToLowerInvariant();
                if (namesCorrectResponse.Equals("n"))
                {
                    UserData.PlayerNames.Clear();
                }
            }
            else
            {
                UserData.PlayerNames.Clear();
                ChooseNames();
            }
        }

        private string GetPlayerName()
        {
            if (readLine != null)
            {
                readLine = readLine.Trim();
                if (!string.IsNullOrEmpty(readLine) &&
                    !string.IsNullOrWhiteSpace(readLine))
                {
                    return readLine;
                }
            }

            // Just return a random name if there is invalid input.
            string[] names = { "Bob", "Joe", "Sally", "Tim", "Steve" };
            return names[GameSimulationApp.Instance.Random.Next(names.Length)];
        }
    }
}