using System;
using System.Threading;
using TrailEntities;

namespace TrailGame
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("Starting...");
            var oregonTrail = new GameplayModel();
            while (oregonTrail.Instance != null)
            {
                Thread.Sleep(1);
            }
            Console.WriteLine("Closing...");
        }
    }
}