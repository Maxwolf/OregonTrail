using System;
using TrailCommon;

namespace TrailEntities
{
    public class GameInitializer : IGameInitializer
    {
        private static GameInitializer _instance;
        private static Gameplay _gamePlay;

        private ulong _tickCount;

        public static void Destroy()
        {
            _instance = null;
            Console.WriteLine("Closing...");
        }

        public static GameInitializer Instance
        {
            get { return _instance; }
        }

        public ulong TickCount
        {
            get { return _tickCount; }
        }

        public void ChooseProfession()
        {
            
        }

        public void BuyInitialItems()
        {
            
        }

        public void StartGame()
        {
            _gamePlay = new Gameplay();
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
            Console.WriteLine("Starting...");
            _instance = new GameInitializer();

            // Get the currently executing assembly.
            Console.WriteLine("Welcome to Oregon Trail Clone!");
        }

        public void Tick()
        {
            OnTick();
        }

        protected virtual void OnTick()
        {
            // Increase the tick count.
            _tickCount++;

            // Fire tick event for any subscribers to see.
            TickEvent?.Invoke(_tickCount);
        }

        public event Tick TickEvent;
    }
}