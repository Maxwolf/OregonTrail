using System;
using System.Threading;
using TrailEntities;

namespace TrailGame
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var gameServer = new GameServerApp();
            var gameClient = new GameClientApp();
            while (!gameServer.IsClosing || !gameClient.IsClosing)
            {
                Thread.Sleep(1);
                Console.Title =
                    "Oregon Trail Clone - " +
                    $"Turns: {gameServer.TotalTurns.ToString("D4")} - " +
                    $"Mode: {gameServer.ActiveModeName} - " +
                    $"[{gameServer.TickPhase}]";
            }
        }
    }
}