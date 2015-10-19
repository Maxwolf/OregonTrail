using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using TrailCommon;
using TrailCommon.CommandLine_Parser;
using TrailEntities;

namespace TrailGame
{
    internal class Program
    {
        private static Process _serverClientProcess;
        private static SimulationType _simulationType;
        private static GameSimulationApp _game;

        /// <summary>
        /// Create game simulation server.
        /// </summary>
        /// <param name="args"></param>
        private static void Main(string[] args)
        {
            var commandLine = new ArgumentParser(args);

            // Check if there are any command line arguments at all.
            if (commandLine.IsEmpty())
            {
                Console.WriteLine("Cannot start simulation without a flag, to play start with server flag it will spawn client instance automatically!");
                Console.WriteLine("Press ANY key to close this window...");
                Console.ReadKey();
                return;
            }
            
            if (commandLine["server"] != null)
            {
                // Server startup.
                _simulationType = SimulationType.Server;
                _game = new GameSimulationApp(_simulationType);
                _game.FirstTickEvent += GameServer_FirstTickEvent;
            }
            else if (commandLine["client"] != null)
            {
                // Client startup.
                _simulationType = SimulationType.Client;
                _game = new GameSimulationApp(_simulationType);
            }

            // Keep the console app alive during the duration the simulation is running, this is only thing keeping console app open.
            while (!_game.IsClosing)
            {
                Thread.Sleep(1);
                Console.Title = _game.ToString();
            }

            // Close server client console helper if we spawned it in another process and this one closes.
            if (_simulationType == SimulationType.Server)
            {
                _serverClientProcess.Close();
            }
        }

        /// <summary>
        /// Create another console process for client controls, but only if this is a server instance.
        /// </summary>
        private static void GameServer_FirstTickEvent()
        {
            if (_simulationType == SimulationType.Server)
            {
                _serverClientProcess = Process.Start(Assembly.GetExecutingAssembly().Location, "-client");
            }
        }
    }
}