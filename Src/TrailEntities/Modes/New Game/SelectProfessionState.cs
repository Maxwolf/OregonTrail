using System;
using System.Text;
using TrailCommon;

namespace TrailEntities
{
    /// <summary>
    ///     Facilitates the ability for a user to select a given profession for the party leader. This will determine the
    ///     starting amount of money their party has access to when purchasing starting items for the journey on the trail path
    ///     simulation.
    /// </summary>
    public sealed class SelectProfessionState : ModeState<NewGameInfo>
    {
        private StringBuilder professionChooser;

        /// <summary>
        ///     Reference to the total number of professions found in the enumeration.
        /// </summary>
        private int professionCount = 1;

        /// <summary>
        ///     This constructor will be used by the other one
        /// </summary>
        public SelectProfessionState(IMode gameMode, NewGameInfo userData) : base(gameMode, userData)
        {
            // Set the profession to default value in case we are retrying this.
            UserData.PlayerProfession = Profession.Banker;
            UserData.StartingMonies = 1600;

            // Information about professions and how they work.
            professionChooser = new StringBuilder();
            professionChooser.Append("You must choose the occupation of the main character.\n");
            professionChooser.Append("Various occupations have advantages over one another:\n");
            professionChooser.Append("-------------------------------------------------------------------------------\n");
            professionChooser.Append("OCCUPATION   | CASH  |  ADVANTAGES                                |FINAL BONUS|\n");
            professionChooser.Append("-------------------------------------------------------------------------------\n");
            professionChooser.Append("Banker       |$1,600 | none                                       | x1        |\n");
            professionChooser.Append("Carpenter    |$800   | more likely to repair broken wagon parts.  | x2        |\n");
            professionChooser.Append("Farmer       |$400   | oxen are less likely to get sick and die.  | x3        |\n");
            professionChooser.Append("-------------------------------------------------------------------------------\n");
            professionChooser.Append("Cash = how much cash a person of that occupation begins with.\n");
            professionChooser.Append("Advantages = special individual attributes of the occupation.\n");
            professionChooser.Append("Final Bonus = amount that your final point total will be multiplied by.\n\n");

            // Loop through every profession in the enumeration for them and print them in string builder.
            foreach (var possibleProfession in Enum.GetValues(typeof (Profession)))
            {
                professionChooser.AppendFormat("  {0} - {1}\n", professionCount, possibleProfession);
                professionCount++;
            }

            // Ask the user to make a selection, and then wait for input...
            professionChooser.AppendFormat("What profession is {0}?", UserData.PlayerNames[0]);
        }

        /// <summary>
        ///     Determines if user input is currently allowed to be typed and filled into the input buffer.
        /// </summary>
        /// <remarks>Default is FALSE. Setting to TRUE allows characters and input buffer to be read when submitted.</remarks>
        public override bool AcceptsInput
        {
            get { return true; }
        }

        public override string GetStateTUI()
        {
            return professionChooser.ToString();
        }

        public override void OnInputBufferReturned(string input)
        {
            // Once a profession is selected, we need to confirm that is what the user wanted.
            switch (input)
            {
                case "1":
                    UserData.PlayerProfession = Profession.Banker;
                    UserData.StartingMonies = 1600;
                    ParentMode.CurrentState = new ConfirmProfessionState(ParentMode, UserData);
                    break;
                case "2":
                    UserData.PlayerProfession = Profession.Carpenter;
                    UserData.StartingMonies = 800;
                    ParentMode.CurrentState = new ConfirmProfessionState(ParentMode, UserData);
                    break;
                case "3":
                    UserData.PlayerProfession = Profession.Farmer;
                    UserData.StartingMonies = 400;
                    ParentMode.CurrentState = new ConfirmProfessionState(ParentMode, UserData);
                    break;
                default:
                    // If there is some invalid selection just start the process over again.
                    UserData.PlayerProfession = Profession.Banker;
                    UserData.StartingMonies = 1600;
                    ParentMode.CurrentState = new SelectProfessionState(ParentMode, UserData);
                    break;
            }
        }
    }
}