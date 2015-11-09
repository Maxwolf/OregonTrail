using System;
using System.Text;
using TrailEntities.Entity.Person;
using TrailEntities.Mode;
using TrailEntities.Simulation;

namespace TrailEntities.Game.MainMenu
{
    /// <summary>
    ///     Facilitates the ability for a user to select a given profession for the party leader. This will determine the
    ///     starting amount of money their party has access to when purchasing starting items for the journey on the trail path
    ///     simulation.
    /// </summary>
    public sealed class SelectProfessionState : StateProduct
    {
        /// <summary>
        ///     References the string for the profession selection so it is only constructed once.
        /// </summary>
        private StringBuilder _professionChooser;

        /// <summary>
        ///     This constructor will be used by the other one
        /// </summary>
        public SelectProfessionState(ModeProduct gameMode, MainMenuInfo userData) : base(gameMode, userData)
        {
            // Set the profession to default value in case we are retrying this.
            UserData.PlayerProfession = Profession.Banker;
            UserData.StartingMonies = 1600;

            // Pass the game data to the simulation for each new game mode state.
            GameSimulationApp.Instance.SetData(userData);

            // Loop through every profession in the enumeration.
            _professionChooser = new StringBuilder();
            _professionChooser.Append($"{Environment.NewLine}Many kinds of people made the{Environment.NewLine}");
            _professionChooser.Append($"trip to Oregon.{Environment.NewLine}{Environment.NewLine}");
            _professionChooser.Append($"You may:{Environment.NewLine}{Environment.NewLine}");
            _professionChooser.Append($"1. Be a banker from Boston{Environment.NewLine}");
            _professionChooser.Append($"2. Be a carpenter from Ohio{Environment.NewLine}");
            _professionChooser.Append($"3. Be a farmer from Illinois{Environment.NewLine}");
            _professionChooser.Append($"4. Find out the differences{Environment.NewLine}");
            _professionChooser.Append($"   between these choices");
        }

        /// <summary>
        ///     Determines if user input is currently allowed to be typed and filled into the input buffer.
        /// </summary>
        /// <remarks>Default is FALSE. Setting to TRUE allows characters and input buffer to be read when submitted.</remarks>
        public override bool AcceptsInput
        {
            get { return true; }
        }

        public override string OnRenderState()
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
                    ParentMode.AddState(typeof (InputPlayerNameState));
                    break;
                case "2":
                    UserData.PlayerProfession = Profession.Carpenter;
                    UserData.StartingMonies = 800;
                    ParentMode.AddState(typeof (InputPlayerNameState));
                    break;
                case "3":
                    UserData.PlayerProfession = Profession.Farmer;
                    UserData.StartingMonies = 400;
                    ParentMode.AddState(typeof (InputPlayerNameState));
                    break;
                case "4":
                    // Shows information about what the different profession choices mean.
                    UserData.PlayerProfession = Profession.Banker;
                    UserData.StartingMonies = 1600;
                    ParentMode.AddState(typeof (ProfessionAdviceState));
                    break;
                default:
                    // If there is some invalid selection just start the process over again.
                    UserData.PlayerProfession = Profession.Banker;
                    UserData.StartingMonies = 1600;
                    ParentMode.AddState(typeof (SelectProfessionState));
                    break;
            }
        }
    }
}