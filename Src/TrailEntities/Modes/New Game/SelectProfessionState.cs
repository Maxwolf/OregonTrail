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

            // Loop through every profession in the enumeration for them and print them in string builder.
            professionChooser = new StringBuilder();
            foreach (var possibleProfession in Enum.GetValues(typeof (Profession)))
            {
                professionChooser.AppendFormat("  {0} - {1}\n", professionCount, possibleProfession);
                professionCount++;
            }

            // Ask the user to make a selection, and then wait for input...
            professionChooser.AppendFormat("What profession is {0}?", UserData.PlayerNames[0]);
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
                    Mode.CurrentState = new ConfirmProfessionState(Mode, UserData);
                    break;
                case "2":
                    UserData.PlayerProfession = Profession.Carpenter;
                    Mode.CurrentState = new ConfirmProfessionState(Mode, UserData);
                    break;
                case "3":
                    UserData.PlayerProfession = Profession.Farmer;
                    Mode.CurrentState = new ConfirmProfessionState(Mode, UserData);
                    break;
                default:
                    // If there is some invalid selection just start the process over again.
                    Mode.CurrentState = new SelectProfessionState(Mode, UserData);
                    break;
            }
        }
    }
}