using System;
using System.Text;
using TrailCommon;

namespace TrailEntities
{
    public sealed class ChooseProfessionState : ModeState<NewGameInfo>
    {
        /// <summary>
        ///     Reference to the total number of professions found in the enumeration.
        /// </summary>
        private int professionCount = 1;

        /// <summary>
        ///     This constructor will be used by the other one
        /// </summary>
        public ChooseProfessionState(IMode gameMode, NewGameInfo userData) : base(gameMode, userData)
        {

        }

        public override void TickState()
        {
            UserData.PlayerProfession = Profession.Banker;
            switch (Console.ReadKey(true).KeyChar.ToString())
            {
                case "1":
                    UserData.PlayerProfession = Profession.Banker;
                    break;
                case "2":
                    UserData.PlayerProfession = Profession.Carpenter;
                    break;
                case "3":
                    UserData.PlayerProfession = Profession.Farmer;
                    break;
                default:
                    Mode.CurrentState = new ChooseProfessionState(Mode, UserData);
                    break;
            }
        }

        public override string GetStateTUI()
        {
            var professionChooser = new StringBuilder();
            professionChooser.Append("What profession is " + UserData.PlayerNames[0] + "?");

            foreach (var possibleProfession in Enum.GetValues(typeof (Profession)))
            {
                Console.WriteLine(professionCount + "). " + possibleProfession);
                professionCount++;
            }

            return professionChooser.ToString();
        }

        public override void OnInputBufferReturned(string input)
        {
            // Nothing to see here, move along...
        }
    }
}