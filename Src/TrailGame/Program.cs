using System;
using System.Threading;
using TrailEntities;

namespace TrailGame
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Simulation.Create(new GameSimulation());
            while (Simulation.Instance != null)
            {
                Thread.Sleep(1);
                Console.Title = $"Oregon Trail Clone - {Simulation.Instance.TickPhase}";
            }
        }
    }
}