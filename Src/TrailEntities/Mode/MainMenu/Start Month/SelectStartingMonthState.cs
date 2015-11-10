using System;
using System.Text;
using TrailEntities.Simulation;

namespace TrailEntities.Mode
{
    /// <summary>
    ///     Offers the player the ability to change the starting month of the simulation, this affects how many resources will
    ///     be available to them and the severity of the random events they encounter along the trail.
    /// </summary>
    public sealed class SelectStartingMonthState : ModeState<MainMenuInfo>
    {
        /// <summary>
        ///     References the string representing the question about starting month, only builds it once and holds in memory while
        ///     state is active.
        /// </summary>
        private StringBuilder _startMonthQuestion;

        /// <summary>
        ///     This constructor will be used by the other one
        /// </summary>
        public SelectStartingMonthState(IMode gameMode, MainMenuInfo userData) : base(gameMode, userData)
        {
            // Pass the game data to the simulation for each new game mode state.
            GameSimulationApp.Instance.SetData(userData);

            // Tell the user they need to make a decision.
            _startMonthQuestion = new StringBuilder();
            _startMonthQuestion.Append($"{Environment.NewLine}It is 1848. Your jumping off place{Environment.NewLine}");
            _startMonthQuestion.Append($"for Oregon is Independence, Missouri.{Environment.NewLine}");
            _startMonthQuestion.Append($"You must decide which month{Environment.NewLine}");
            _startMonthQuestion.Append($"to leave Independence{Environment.NewLine}{Environment.NewLine}");
            _startMonthQuestion.Append($"  1. March{Environment.NewLine}");
            _startMonthQuestion.Append($"  2. April{Environment.NewLine}");
            _startMonthQuestion.Append($"  3. May{Environment.NewLine}");
            _startMonthQuestion.Append($"  4. June{Environment.NewLine}");
            _startMonthQuestion.Append($"  5. July{Environment.NewLine}");
            _startMonthQuestion.Append($"  6. Ask for advice");
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
            switch (input.ToUpperInvariant())
            {
                case "1":
                    UserData.StartingMonth = Months.March;
                    ParentMode.CurrentState = new BuyInitialItemsState(ParentMode, UserData);
                    break;
                case "2":
                    UserData.StartingMonth = Months.April;
                    ParentMode.CurrentState = new BuyInitialItemsState(ParentMode, UserData);
                    break;
                case "3":
                    UserData.StartingMonth = Months.May;
                    ParentMode.CurrentState = new BuyInitialItemsState(ParentMode, UserData);
                    break;
                case "4":
                    UserData.StartingMonth = Months.June;
                    ParentMode.CurrentState = new BuyInitialItemsState(ParentMode, UserData);
                    break;
                case "5":
                    UserData.StartingMonth = Months.July;
                    ParentMode.CurrentState = new BuyInitialItemsState(ParentMode, UserData);
                    break;
                case "6":
                    // Shows information about what the different starting months mean.
                    ParentMode.CurrentState = new StartMonthAdviceState(ParentMode, UserData);
                    break;
                default:
                    UserData.StartingMonth = Months.March;
                    ParentMode.CurrentState = new SelectStartingMonthState(ParentMode, UserData);
                    break;
            }
        }
    }
}