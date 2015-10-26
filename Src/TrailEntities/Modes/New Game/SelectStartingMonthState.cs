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
        private StringBuilder startMonthHelp;

        /// <summary>
        ///     This constructor will be used by the other one
        /// </summary>
        public SelectStartingMonthState(IMode gameMode, NewGameInfo userData) : base(gameMode, userData)
        {
            // Inform the user about a decision they need to make.
            startMonthHelp = new StringBuilder();
            startMonthHelp.Append("You need to decide when to set off on the trail.  If you leave too soon, there\n");
            startMonthHelp.Append("won't be much grass for your oxen to eat.  You may encounter some very cold\n");
            startMonthHelp.Append("weather and late spring snowstorms.\n\n");

            startMonthHelp.Append("Starting in March will guarantee a rough start.  It will be cold in the\n");
            startMonthHelp.Append("beginning, but you'll get a rather mild weathered journey from the middle on.\n\n");

            startMonthHelp.Append("Starting in April/May is the easiest.  You'll get good weather throughout your\n");
            startMonthHelp.Append("journey, until (possibly) the very end, if you rest a lot.\n\n");

            startMonthHelp.Append("Starting in June-August ensures that you'll have to endure a tough winter in\n");
            startMonthHelp.Append("the end (or middle, depending on how late you start) of your journey.\n\n");

            // Tell the user they need to make a decision.
            startMonthHelp.Append("When do you want to Start?  March, April, May, June, July, or August?\n");
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
            return startMonthHelp.ToString();
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
                default:
                    UserData.StartingMonth = Months.March;
                    ParentMode.CurrentState = new SelectStartingMonthState(ParentMode, UserData);
                    break;
            }
        }
    }
}