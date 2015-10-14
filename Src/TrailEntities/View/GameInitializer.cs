using System;
using TrailCommon;

namespace TrailEntities
{
    public class GameInitializer : IGameInitializer
    {
        private static GameInitializer _instance;

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
            Gameplay.Create();
        }

        public static void Create()
        {
            // Complain if gameplay instance is not null.
            if (Gameplay.Instance != null)
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
            // Increase the tick count.
            _tickCount++;
        }
    }
}