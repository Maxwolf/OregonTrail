﻿using System;
using System.Text;

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
            _professionChooser.Append("\nMany kinds of people made the\n");
            _professionChooser.Append("trip to Oregon.\n\n");
            _professionChooser.Append("You may:\n\n");
            _professionChooser.Append("1. Be a banker from Boston\n");
            _professionChooser.Append("2. Be a carpenter from Ohio\n");
            _professionChooser.Append("3. Be a farmer from Illinois\n");
            _professionChooser.Append("4. Find out the differences\n");
            _professionChooser.Append("   between these choices");
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