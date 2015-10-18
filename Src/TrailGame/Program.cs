using System;
using System.Threading;
using TrailEntities;

namespace TrailGame
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            SimulationApp.Create(new GameSimulationApp());
            while (SimulationApp.Instance != null)
            {
                Thread.Sleep(1);
                Console.Title = $"Oregon Trail Clone - {SimulationApp.Instance.TickPhase}";
            }
        }
    }
}