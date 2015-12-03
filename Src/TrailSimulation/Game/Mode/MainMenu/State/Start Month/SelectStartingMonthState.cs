using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrailSimulation.Core;

namespace TrailSimulation.Game
{
    /// <summary>
    ///     Offers the player the ability to change the starting month of the simulation, this affects how many resources will
    ///     be available to them and the severity of the random events they encounter along the trail.
    /// </summary>
    [RequiredMode(Mode.MainMenu)]
    public sealed class SelectStartingMonthState : StateProduct<MainMenuInfo>
    {
        /// <summary>
        ///     References the string representing the question about starting month, only builds it once and holds in memory while
        ///     state is active.
        /// </summary>
        private StringBuilder _startMonthQuestion;

        /// <summary>
        ///     This constructor will be used by the other one
        /// </summary>
        public SelectStartingMonthState(IModeProduct gameMode) : base(gameMode)
        {
            // Pass the game data to the simulation for each new game mode state.
            GameSimulationApp.Instance.SetData(UserData);

            // Tell the user they need to make a decision.
            _startMonthQuestion = new StringBuilder();
            _startMonthQuestion.AppendLine($"{Environment.NewLine}It is 1848. Your jumping off place");
            _startMonthQuestion.AppendLine("for Oregon is Independence, Missouri.");
            _startMonthQuestion.AppendLine("You must decide which month");
            _startMonthQuestion.AppendLine($"to leave Independence{Environment.NewLine}");

            // Loop through every possible starting month and list them out by their enumeration integer values along with description attribute.
            var choices = new List<StartingMonth>(Enum.GetValues(typeof (StartingMonth)).Cast<StartingMonth>());
            for (var index = 0; index < choices.Count; index++)
            {
                // Get the current river choice enumeration value we casted into list.
                var monthValue = choices[index];

                // Last line should not print new line.
                if (index == (choices.Count - 1))
                {
                    _startMonthQuestion.AppendLine($"  {(int) monthValue}. {monthValue}");
                    _startMonthQuestion.Append($"  {choices.Count + 1}. Ask for advice");
                }
                else
                {
                    _startMonthQuestion.AppendLine($"  {(int) monthValue}. {monthValue}");
                }
            }
        }

        /// <summary>
        ///     Determines if user input is currently allowed to be typed and filled into the input buffer.
        /// </summary>
        /// <remarks>Default is FALSE. Setting to TRUE allows characters and input buffer to be read when submitted.</remarks>
        public override bool InputFillsBuffer
        {
            get { return true; }
        }

        /// <summary>
        ///     Returns a text only representation of the current game mode state. Could be a statement, information, question
        ///     waiting input, etc.
        /// </summary>
        public override string OnRenderState()
        {
            return _startMonthQuestion.ToString();
        }

        /// <summary>
        ///     Fired when the game mode current state is not null and input buffer does not match any known command.
        /// </summary>
        /// <param name="input">Contents of the input buffer which didn't match any known command in parent game mode.</param>
        public override void OnInputBufferReturned(string input)
        {
            // Skip if the input is null or empty.
            if (string.IsNullOrEmpty(input) || string.IsNullOrWhiteSpace(input))
                return;

            // Attempt to cast string to enum value, can be characters or integer.
            StartingMonth startMonth;
            Enum.TryParse(input, out startMonth);

            // Depending on what was selected we will set starting month to correct one in full listing, or show advice to player.
            switch (startMonth)
            {
                case StartingMonth.March:
                    UserData.StartingMonth = Month.March;
                    SetState(typeof (BuyInitialItemsState));
                    break;
                case StartingMonth.April:
                    UserData.StartingMonth = Month.April;
                    SetState(typeof (BuyInitialItemsState));
                    break;
                case StartingMonth.May:
                    UserData.StartingMonth = Month.May;
                    SetState(typeof (BuyInitialItemsState));
                    break;
                case StartingMonth.June:
                    UserData.StartingMonth = Month.June;
                    SetState(typeof (BuyInitialItemsState));
                    break;
                case StartingMonth.July:
                    UserData.StartingMonth = Month.July;
                    SetState(typeof (BuyInitialItemsState));
                    break;
                default:
                    // Shows information about what the different starting months mean.
                    SetState(typeof (StartMonthAdviceState));
                    break;
            }
        }
    }
}