using System;
using System.Diagnostics;
using System.Threading;
using TrailEntities;

namespace TrailGame
{
    internal class Program
    {
        private static GameClient _gameClient;
        private static GameServer _gameServer;

        /// <summary>
        ///     Create game simulation server or client depending on what the user wants.
        /// </summary>
        /// <param name="args"></param>
        private static void Main(string[] args)
        {
            // Present main menu.
            Console.Title = "Oregon Trail Clone - Main Menu";
            Console.WriteLine("What do you want to do?");
            Console.WriteLine("1. Dedicated Server");
            Console.WriteLine("2. ServerClient Hybrid");
            Console.WriteLine("3. Server Spawned Client Process");
            Console.WriteLine("4. Client Only");

            // Wait for user input, locks thread.
            var keyPressed = Console.ReadLine();
            Console.Clear();
            SimType simType;
            switch (keyPressed)
            {
                case "1":
                    simType = SimType.DedicatedServer;
                    break;
                case "2":
                    simType = SimType.ServerClientHybrid;
                    break;
                case "3":
                    simType = SimType.ServerSpawnedClientProcess;
                    break;
                case "4":
                    simType = SimType.ClientOnly;
                    break;
                default:
                    return;
            }

            // Setup simulations based on user selection.
            switch (simType)
            {
                case SimType.DedicatedServer:
                    DedicatedServer();
                    break;
                case SimType.ServerClientHybrid:
                    ServerClientHybrid();
                    break;
                case SimType.ServerSpawnedClientProcess:
                    ServerSpawnedClientProcess();
                    break;
                case SimType.ClientOnly:
                    ClientOnly();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(simType), simType, null);
            }

            // Goodbye!
            Console.WriteLine("Closing...");
        }

        private static void DedicatedServer()
        {
            Console.WriteLine("Starting dedicated server...");
            _gameServer = new GameServer();

            while (_gameServer != null && !_gameServer.IsClosing)
            {
                Thread.Sleep(1);
                Console.Title = _gameServer.ToString();
            }
        }

        private static void ServerClientHybrid()
        {
            Console.WriteLine("Starting server...");
            _gameServer = new GameServer();
            Console.WriteLine("Starting client...");
            _gameClient = new GameClient();

            while ((_gameServer != null && !_gameServer.IsClosing) &&
                   (_gameClient != null && !_gameClient.IsClosing))
            {
                Thread.Sleep(1);
                Console.Title = _gameServer.ToString();
            }
        }

        private static void ServerSpawnedClientProcess()
        {
            Console.WriteLine("Starting server...");
            _gameServer = new GameServer();
            Console.WriteLine("Spawning client process...");
            _gameClient = new GameClient();
            var t = new Thread(_gameClient.Create);
            t.Start();

            while (_gameServer != null && !_gameServer.IsClosing)
            {
                Thread.Sleep(1);
                Console.Title = _gameServer.ToString();
            }

        }

        private static void ClientOnly()
        {
            Console.WriteLine("Starting client only...");
            _gameClient = new GameClient();

            while (_gameClient != null && !_gameClient.IsClosing)
            {
                Thread.Sleep(1);
                Console.Title = _gameClient.ToString();
            }
        }
    }
}