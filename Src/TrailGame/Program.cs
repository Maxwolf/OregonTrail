using System;
using System.Threading;
using TrailEntities;

namespace TrailGame
{
    internal class Program
    {
        /// <summary>
        ///     Create game simulation server or client depending on what the user wants.
        /// </summary>
        /// <param name="args"></param>
        private static void Main(string[] args)
        {
            // Check if there are any command line arguments at all.
            Console.Title = "Oregon Trail Clone - Main Menu";
            Console.WriteLine("What do you want to do?");
            Console.WriteLine("1. Server");
            Console.WriteLine("2. Client");
            var keyPressed = Console.ReadLine();

            Console.Clear();
            switch (keyPressed)
            {
                case "1":
                    StartServer();
                    break;
                case "2":
                    StartClient();
                    break;
                default:
                    return;
            }
        }

        /// <summary>
        /// Client startup.
        /// </summary>
        private static void StartClient()
        {
            Console.WriteLine("Starting client...");
            var _gameClient = new GameClient();

            // Keep the console app alive during the duration the simulation is running, this is only thing keeping console app open.
            while (!_gameClient.IsClosing)
            {
                Thread.Sleep(1);
                Console.Title = _gameClient.ToString();
            }
        }

        /// <summary>
        /// Server startup.
        /// </summary>
        private static void StartServer()
        {
            Console.WriteLine("Starting server...");
            var _gameServer = new GameServer();

            // Keep the console app alive during the duration the simulation is running, this is only thing keeping console app open.
            while (!_gameServer.IsClosing)
            {
                Thread.Sleep(1);
                Console.Title = _gameServer.ToString();
            }
        }
    }
}