using System;
using System.Collections.Generic;
using TrailCommon;

namespace TrailEntities
{
    public class GameManager : IGameManager
    {
        private static Gameplay _game = new Gameplay();
        private GameMode _mode;
        private bool _hasChosenNames;
        private bool _hasChosenProfession;
        private bool _hasChosenStartingItems;
        private bool _hasStartedGame;
        private List<string> _playerNames;
        private Random _random = new Random();
        private Profession _playerProfession;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailEntities.GameManager" /> class.
        /// </summary>
        public GameManager()
        {
            TickCount = 0;
        }

        public static GameManager Instance { get; private set; }

        public ulong TickCount { get; private set; }

        public Random Random
        {
            get { return _random; }
        }

        public void ChooseProfession()
        {
            _playerProfession = Profession.Banker;
            var professionCount = 1;

            Console.Clear();
            Console.WriteLine("What profession are is " + _playerNames[0] + "?");

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
            if (!string.IsNullOrEmpty(professionCorrectResponse) &&
                !string.IsNullOrWhiteSpace(professionCorrectResponse))
            {
                professionCorrectResponse = professionCorrectResponse.Trim().ToLowerInvariant();
                if (professionCorrectResponse.Equals("n"))
                {
                    _playerProfession = Profession.Banker;
                    ChooseProfession();
                }
            }
        }

        public void BuyInitialItems()
        {
            SetMode(new Store(_game.Vehicle, new List<IItem>(), 500));
        }

        public void ChooseNames()
        {
            _playerNames = new List<string>();
            Console.Clear();
            Console.WriteLine("Party leader name?");
            var playerName = GetPlayerName();
            _playerNames.Add(playerName);
            Console.WriteLine("Added: " + playerName);

            Console.WriteLine("Party member two name?");
            playerName = GetPlayerName();
            _playerNames.Add(playerName);
            Console.WriteLine("Added: " + playerName);

            Console.WriteLine("Party member three name?");
            playerName = GetPlayerName();
            _playerNames.Add(playerName);
            Console.WriteLine("Added: " + playerName);

            Console.WriteLine("Party member four name?");
            playerName = GetPlayerName();
            _playerNames.Add(playerName);
            Console.WriteLine("Added: " + playerName);

            Console.WriteLine("Your Party Members:");
            int crewNumber = 1;
            foreach (var name in _playerNames)
            {
                var isLeader = _playerNames.IndexOf(name) == 0;
                if (isLeader)
                {
                    Console.WriteLine(crewNumber + ")." + name + " (leader)");
                }
                else
                {
                    Console.WriteLine(crewNumber + ")." + name);
                }
                crewNumber++;
            }

            Console.WriteLine("Does this look correct? Y/N");
            var namesCorrectResponse = Console.ReadLine();
            if (!string.IsNullOrEmpty(namesCorrectResponse) &&
                !string.IsNullOrWhiteSpace(namesCorrectResponse))
            {
                namesCorrectResponse = namesCorrectResponse.Trim().ToLowerInvariant();
                if (namesCorrectResponse.Equals("n"))
                {
                    _playerNames.Clear();
                    ChooseNames();
                }
            }
        }

        public void StartGame()
        {
            Console.WriteLine("Adding " + _playerNames.Count + " people to vehicle...");
            foreach (var name in _playerNames)
            {
                // First name in list in leader.
                var isLeader = _playerNames.IndexOf(name) == 0;
                _game.Vehicle.AddPerson(new Person(_playerProfession, name, isLeader));
            }

            SetMode(new TravelMode(_game.Vehicle));
            NewgameEvent?.Invoke();
        }

        public void SetMode(IGameMode gameMode)
        {
            if (_mode == gameMode)
                return;

            _mode?.ModeChange();
            _mode = gameMode as GameMode;
            Console.WriteLine("Set game mode to " + _mode);
        }

        public void Tick()
        {
            // We do not tick if there is no instance associated with it.
            if (Instance == null)
                throw new InvalidOperationException("Attempted to tick game initializer when instance is null!");

            // Increase the tick count.
            TickCount++;

            // Fire tick event for any subscribers to see.
            TickEvent?.Invoke(TickCount);

            OnTick();
        }

        public event Tick TickEvent;
        public event NewGame NewgameEvent;
        public event EndGame EndgameEvent;

        private static string GetPlayerName()
        {
            var readLine = Console.ReadLine();
            if (readLine != null)
            {
                readLine = readLine.Trim();
                if (!string.IsNullOrEmpty(readLine) &&
                    !string.IsNullOrWhiteSpace(readLine))
                {
                    return readLine;
                }
            }

            // Just return a random name if there is invalid input.
            string[] names = {"Bob", "Joe", "Sally", "Tim", "Steve"};
            return names[Instance.Random.Next(names.Length)];
        }

        public static void Destroy()
        {
            // Complain if destroy was awakened for no reason.
            if (Instance == null)
                throw new InvalidOperationException("Unable to destroy game manager, it has not been created yet!");

            // Allow any data structures to save themselves.
            Console.WriteLine("Closing...");
            Instance.OnDestroy();

            // Actually destroy the instance and close the program.
            Instance = null;
        }

        protected virtual void OnDestroy()
        {
            EndgameEvent?.Invoke();
        }

        public static void Create()
        {
            // Create instance of gameplay initializer.
            Instance = new GameManager();
        }

        protected virtual void OnTick()
        {
            // Every new game has you picking names, profession, and starting items.
            if (!_hasChosenNames && !_hasChosenProfession && !_hasChosenStartingItems && !_hasStartedGame)
            {
                _hasChosenNames = true;
                ChooseNames();
            }
            else if (_hasChosenNames && !_hasChosenProfession && !_hasChosenStartingItems && !_hasStartedGame)
            {
                _hasChosenProfession = true;
                ChooseProfession();
            }
            else if (_hasChosenNames && _hasChosenProfession && !_hasChosenStartingItems && !_hasStartedGame)
            {
                _hasChosenStartingItems = true;
                BuyInitialItems();
            }
            else if (_hasChosenNames && _hasChosenProfession && _hasChosenStartingItems && !_hasStartedGame)
            {
                _hasStartedGame = true;
                StartGame();
            }
        }
    }
}