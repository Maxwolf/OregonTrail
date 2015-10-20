using System;
using System.Collections.Generic;
using TrailCommon;

namespace TrailEntities
{
    public sealed class NewGameMode : GameMode, INewGame
    {
        private bool _hasChosenNames;
        private bool _hasChosenProfession;
        private bool _hasChosenStartingItems;
        private bool _hasStartedGame;
        
        private Profession _playerProfession;
        private List<string> _playerNames;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailEntities.GameMode" /> class.
        /// </summary>
        public NewGameMode(IGameSimulation game) : base(game)
        {
            _playerNames = new List<string>();
            AddCommand(new Tuple<string, ICommand>("ChooseNames", new CommandChooseNames(game)));
        }

        public void ChooseNames()
        {
            // TODO: Make me get triggered by named pipe wrapper from client command name selection.
            //ExecuteCommandByName("ChooseNames");
        }

        public override ModeType Mode
        {
            get { return ModeType.NewGame; }
        }

        public void ChooseProfession()
        {
            _playerProfession = Profession.Banker;
            var professionCount = 1;

            Console.Clear();
            Console.WriteLine("What profession is " + _playerNames[0] + "?");

            foreach (var possibleProfession in Enum.GetValues(typeof (Profession)))
            {
                Console.WriteLine(professionCount + "). " + possibleProfession);
                professionCount++;
            }

            switch (Console.ReadKey(true).KeyChar.ToString())
            {
                case "1":
                    _playerProfession = Profession.Banker;
                    break;
                case "2":
                    _playerProfession = Profession.Carpenter;
                    break;
                case "3":
                    _playerProfession = Profession.Farmer;
                    break;
                default:
                    ChooseProfession();
                    break;
            }

            Console.WriteLine("You selected " + _playerProfession);
            Console.WriteLine("Does this look correct? Y/N");
            var professionCorrectResponse = Console.ReadLine();
            if (!string.IsNullOrEmpty(professionCorrectResponse))
            {
                professionCorrectResponse = professionCorrectResponse.Trim().ToLowerInvariant();
                if (professionCorrectResponse.Equals("n"))
                {
                    _playerProfession = Profession.Banker;
                    ChooseProfession();
                }
            }
            else
            {
                _playerProfession = Profession.Banker;
                ChooseProfession();
            }
        }

        public void BuyInitialItems()
        {
        }

        public void StartGame()
        {
            Console.WriteLine("Adding " + _playerNames.Count + " people to vehicle...");
            foreach (var name in _playerNames)
            {
                // First name in list in leader.
                var isLeader = _playerNames.IndexOf(name) == 0;
                Game.Vehicle.AddPerson(new Person(_playerProfession, name, isLeader));
            }
        }

        public override void TickMode()
        {
            base.TickMode();

            //// Every new game has you picking names, profession, and starting items.
            //if (!_hasChosenNames && !_hasChosenProfession && !_hasChosenStartingItems && !_hasStartedGame)
            //{
            //    _hasChosenNames = true;
            //    ChooseNames();
            //}
            //else if (_hasChosenNames && !_hasChosenProfession && !_hasChosenStartingItems && !_hasStartedGame)
            //{
            //    _hasChosenProfession = true;
            //    ChooseProfession();
            //}
            //else if (_hasChosenNames && _hasChosenProfession && !_hasChosenStartingItems && !_hasStartedGame)
            //{
            //    _hasChosenStartingItems = true;
            //    BuyInitialItems();
            //}
            //else if (_hasChosenNames && _hasChosenProfession && _hasChosenStartingItems && !_hasStartedGame)
            //{
            //    _hasStartedGame = true;
            //    StartGame();
            //}
        }
    }
}