using System;
using System.Threading;
using TrailEntities;

namespace TrailGame
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.Title = "Oregon Trail Clone";
            GameManager.Create();
            while (GameManager.Instance != null)
            {
                Thread.Sleep(1);
                GameManager.Instance.Tick();
            }
        }
    }
}