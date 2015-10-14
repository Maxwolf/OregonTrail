using System;
using System.Threading;
using TrailEntities;

namespace TrailGame
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            GameInitializer.Create();
            while (GameInitializer.Instance != null)
            {
                Thread.Sleep(1);
                GameInitializer.Instance.Tick();
            }
        }
    }
}