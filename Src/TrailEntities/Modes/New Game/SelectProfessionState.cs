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
        /// <summary>
        ///     References the string for the profession selection so it is only constructed once.
        /// </summary>
        private StringBuilder _professionChooser;

        /// <summary>
        ///     Reference to the total number of professions found in the enumeration.
        /// </summary>
        private int _professionCount = 1;

        /// <summary>
        ///     This constructor will be used by the other one
        /// </summary>
        public SelectProfessionState(IMode gameMode, NewGameInfo userData) : base(gameMode, userData)
        {
            // Set the profession to default value in case we are retrying this.
            UserData.PlayerProfession = Profession.Banker;
            UserData.StartingMonies = 1600;

            // Pass the game data to the simulation for each new game mode state.
            GameSimulationApp.Instance.SetData(userData);

            // Loop through every profession in the enumeration.
            _professionChooser = new StringBuilder();
            _professionChooser.Append("\nMany kinds of people made the trip to Oregon.\n");
            _professionChooser.Append("You may:\n\n");
            foreach (Profession profession in Enum.GetValues(typeof (Profession)))
            {
                // Depending on what profession is there make a pretty display name for it.
                switch (profession)
                {
                    case Profession.Banker:
                        _professionChooser.AppendFormat("  {0} - {1}\n", _professionCount, "Be a banker from Boston");
                        break;
                    case Profession.Carpenter:
                        _professionChooser.AppendFormat("  {0} - {1}\n", _professionCount, "Be a carpenter from Ohio");
                        break;
                    case Profession.Farmer:
                        _professionChooser.AppendFormat("  {0} - {1}\n", _professionCount, "Be a farmer from Illinois");
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                // Increment the profession counts.
                _professionCount++;
            }

            // Ask the user to make a selection, and then wait for input...
            _professionChooser.AppendFormat("  {0} - {1}\n\n", _professionCount,
                "Find out the differences between these choices");
            _professionChooser.Append("What is your choice?");
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
            return _professionChooser.ToString();
        }

        public override void OnInputBufferReturned(string input)
        {
            // Once a profession is selected, we need to confirm that is what the user wanted.
            switch (input.ToUpperInvariant())
            {
                case "1":
                    UserData.PlayerProfession = Profession.Banker;
                    UserData.StartingMonies = 1600;
                    ParentMode.CurrentState = new InputPlayerNameState(0, ParentMode, UserData);
                    break;
                case "2":
                    UserData.PlayerProfession = Profession.Carpenter;
                    UserData.StartingMonies = 800;
                    ParentMode.CurrentState = new InputPlayerNameState(0, ParentMode, UserData);
                    break;
                case "3":
                    UserData.PlayerProfession = Profession.Farmer;
                    UserData.StartingMonies = 400;
                    ParentMode.CurrentState = new InputPlayerNameState(0, ParentMode, UserData);
                    break;
                case "4":
                    UserData.PlayerProfession = Profession.Banker;
                    UserData.StartingMonies = 1600;
                    ParentMode.CurrentState = new ProfessionAdviceState(ParentMode, UserData);
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