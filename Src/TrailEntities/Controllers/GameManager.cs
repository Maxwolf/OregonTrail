using System;
using TrailCommon;

namespace TrailEntities
{
    public class GameManager : IGameManager
    {
        private static Gameplay _gamePlay;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailEntities.GameInitializer" /> class.
        /// </summary>
        public GameManager()
        {
            // Reset the tick counter.
            TickCount = 0;

            // Pick initial profession, buy initial store items, start new game.
            ChooseProfession();
            BuyInitialItems();
            StartGame();
        }

        public static GameManager Instance { get; private set; }

        public ulong TickCount { get; private set; }

        public void ChooseProfession()
        {
        }

        public void BuyInitialItems()
        {
        }

        public void StartGame()
        {
            Console.WriteLine("Starting new game...");
            _gamePlay = new Gameplay();
            NewgameEvent?.Invoke();
        }

        public void Tick()
        {
            // We do not tick if there is no instance associated with it.
            if (Instance == null)
                throw new InvalidOperationException("Attempted to tick game initializer when instance is null!");

            OnTick();
        }

        public event Tick TickEvent;
        public event KeyPress KeypressEvent;
        public event NewGame NewgameEvent;
        public event EndGame EndgameEvent;

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
            // Complain if gameplay instance is not null.
            if (_gamePlay != null)
            {
                throw new InvalidOperationException(
                    "Unable to create gameplay initializer because gameplay instance is not null. Perhaps a game already been created.");
            }

            // Create instance of gameplay initializer.
            Instance = new GameManager();
        }

        protected virtual void OnTick()
        {
            // Increase the tick count.
            TickCount++;

            // Fire tick event for any subscribers to see.
            TickEvent?.Invoke(TickCount);

            // Grab the current key from the console.
            var keyGet = Console.ReadKey(false);
            var keyText = keyGet.KeyChar.ToString();
            if (!string.IsNullOrEmpty(keyText) &&
                !string.IsNullOrWhiteSpace(keyText))
            {
                KeypressEvent?.Invoke(keyText);
            }
        }
    }
}