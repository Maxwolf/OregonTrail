using System.Text;
using TrailCommon;

namespace TrailEntities
{
    /// <summary>
    ///     Offers the player the ability to change the starting month of the simulation, this affects how many resources will
    ///     be available to them and the severity of the random events they encounter along the trail.
    /// </summary>
    public sealed class SelectStartingMonthState : ModeState<NewGameInfo>
    {
        /// <summary>
        ///     References the string representing the question about starting month, only builds it once and holds in memory while
        ///     state is active.
        /// </summary>
        private StringBuilder _startMonthQuestion;

        /// <summary>
        ///     This constructor will be used by the other one
        /// </summary>
        public SelectStartingMonthState(IMode gameMode, NewGameInfo userData) : base(gameMode, userData)
        {
            // Tell the user they need to make a decision.
            _startMonthQuestion = new StringBuilder();
            _startMonthQuestion.Append("Type HELP for more information about starting months.\n");
            _startMonthQuestion.Append("When do you want to start? Type March, April, May, June, July, or August");
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
                case "MARCH":
                    UserData.StartingMonth = Months.March;
                    ParentMode.CurrentState = new ConfirmStartingMonthState(ParentMode, UserData);
                    break;
                case "APRIL":
                    UserData.StartingMonth = Months.April;
                    ParentMode.CurrentState = new ConfirmStartingMonthState(ParentMode, UserData);
                    break;
                case "MAY":
                    UserData.StartingMonth = Months.May;
                    ParentMode.CurrentState = new ConfirmStartingMonthState(ParentMode, UserData);
                    break;
                case "JUNE":
                    UserData.StartingMonth = Months.June;
                    ParentMode.CurrentState = new ConfirmStartingMonthState(ParentMode, UserData);
                    break;
                case "JULY":
                    UserData.StartingMonth = Months.July;
                    ParentMode.CurrentState = new ConfirmStartingMonthState(ParentMode, UserData);
                    break;
                case "AUGUST":
                    UserData.StartingMonth = Months.August;
                    ParentMode.CurrentState = new ConfirmStartingMonthState(ParentMode, UserData);
                    break;
                case "HELP":
                    // Shows information about what the different starting months mean.
                    UserData.StartingMonth = Months.March;
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