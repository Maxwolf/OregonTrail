using System;
using System.Text;
using TrailSimulation.Core;

namespace TrailSimulation.Game
{
    /// <summary>
    ///     Gets the name of a player for a particular index in the player name user data object. This will also offer the user
    ///     a chance to confirm their selection in another state, reset if they don't like it, and also generate a random user
    ///     name if they just press enter at the prompt for a name.
    /// </summary>
    [RequiredMode(GameMode.MainMenu)]
    public sealed class InputPlayerNameState : StateProduct<MainMenuInfo>
    {
        /// <summary>
        ///     References the string that makes up the question about player names and also showing previous ones that have been
        ///     entered for continuity sake.
        /// </summary>
        private StringBuilder _inputNamesHelp;

        /// <summary>
        ///     This constructor will be used by the other one
        /// </summary>
        public InputPlayerNameState(IModeProduct gameMode) : base(gameMode)
        {
            // Pass the game data to the simulation for each new game mode state.
            GameSimulationApp.Instance.SetData(UserData);

            // Create string builder so we only build up this data once.
            _inputNamesHelp = new StringBuilder();

            // Add the question text from constructor parameter.
            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (UserData.PlayerNameIndex)
            {
                case 0:
                    _inputNamesHelp.Append(Environment.NewLine +
                                           $"{MainMenuMode.LEADER_QUESTION}");
                    break;
                case 1:
                    _inputNamesHelp.Append(Environment.NewLine +
                                           $"{MainMenuMode.MEMBERS_QUESTION}" +
                                           $"{Environment.NewLine}{Environment.NewLine}");
                    break;
                case 2:
                    _inputNamesHelp.Append(Environment.NewLine +
                                           $"{MainMenuMode.MEMBERS_QUESTION}" +
                                           $"{Environment.NewLine}{Environment.NewLine}");
                    break;
                case 3:
                    _inputNamesHelp.Append(Environment.NewLine +
                                           $"{MainMenuMode.MEMBERS_QUESTION}" +
                                           $"{Environment.NewLine}{Environment.NewLine}");
                    break;
            }

            // Only print player names if we have some to actually print.
            if (UserData.PlayerNames.Count > 0)
            {
                // Loop through all the player names and get their current state.
                var crewNumber = 1;

                // Loop through every player and print their name.
                for (var index = 0; index < GameSimulationApp.MAX_PLAYERS; index++)
                {
                    var name = string.Empty;
                    if (index < UserData.PlayerNames.Count)
                        name = UserData.PlayerNames[index];

                    // First name in list is always the leader.
                    var isLeader = UserData.PlayerNames.IndexOf(name) == 0 && crewNumber == 1;
                    _inputNamesHelp.AppendFormat(isLeader
                        ? $"  {crewNumber} - {name} (leader){Environment.NewLine}"
                        : $"  {crewNumber} - {name}{Environment.NewLine}");
                    crewNumber++;
                }

                // Wait for user input...
                _inputNamesHelp.Append("\n(Enter names or press Enter)");
            }
        }

        /// <summary>
        ///     Returns a text only representation of the current game mode state. Could be a statement, information, question
        ///     waiting input, etc.
        /// </summary>
        public override string OnRenderState()
        {
            return _inputNamesHelp.ToString();
        }

        /// <summary>
        ///     Fired when the game mode current state is not null and input buffer does not match any known command.
        /// </summary>
        /// <param name="input">Contents of the input buffer which didn't match any known command in parent game mode.</param>
        public override void OnInputBufferReturned(string input)
        {
            // If player enters empty name fill out all the slots with random ones.
            if (string.IsNullOrEmpty(input) || string.IsNullOrWhiteSpace(input))
            {
                // Only fill out names for slots that are empty.
                for (var i = 0; i < (GameSimulationApp.MAX_PLAYERS - UserData.PlayerNameIndex); i++)
                {
                    UserData.PlayerNames.Insert(UserData.PlayerNameIndex, GetPlayerName());
                }

                // Attach state to confirm randomized name selection, skipping manual entry with the return.
                SetState(typeof (ConfirmPlayerNamesState));
                return;
            }

            // Add the name to list since we will have something at this point even if randomly generated.
            UserData.PlayerNames.Insert(UserData.PlayerNameIndex, input);

            // Change the state to either confirm or input the next name based on index of name we are entering.
            SetState(UserData.PlayerNameIndex + 1 < GameSimulationApp.MAX_PLAYERS
                ? typeof (InputPlayerNameState)
                : typeof (ConfirmPlayerNamesState));
        }

        /// <summary>
        ///     Returns a random name if there is an empty name returned, we assume the player doesn't care and just give him one.
        /// </summary>
        private static string GetPlayerName()
        {
            string[] names =
            {
                "Bob", "Joe", "Sally", "Tim", "Steve", "Zeke", "Suzan", "Rebekah", "Young", "Marquitta",
                "Kristy", "Sharice", "Joanna", "Chrystal", "Genevie", "Angela", "Ruthann", "Viva", "Iris", "Anderson",
                "Siobhan", "Karey", "Jolie", "Carlene", "Lekisha", "Buck"
            };
            return names[GameSimulationApp.Instance.Random.Next(names.Length)];
        }
    }
}