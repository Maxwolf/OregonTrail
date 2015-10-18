using System;
using System.Threading;
using TrailEntities;

namespace TrailGame
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var game = new GameSimulationApp();
            while (!game.IsClosing)
            {
                Thread.Sleep(1);
                Console.Title =
                    "Oregon Trail Clone - " +
                    $"Turns: {game.TotalTurns.ToString("D4")} - " +
                    $"Mode: {game.ActiveModeName} - " +
                    $"[{game.TickPhase}]";
            }
        }
    }
}