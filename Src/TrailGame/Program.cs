using System;
using System.Threading;
using TrailEntities;

namespace TrailGame
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var gameServer = new GameSimulationApp();
            while (!gameServer.IsClosing)
            {
                Thread.Sleep(1);
                Console.Title =
                    "Oregon Trail Clone -" +
                    $" Turns: {gameServer.TotalTurns.ToString("D4")} - " +
                    $"[{gameServer.TickPhase}]";
            }
        }
    }
}