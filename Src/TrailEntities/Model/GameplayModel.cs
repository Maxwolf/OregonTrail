using System;
using System.IO;
using System.Reflection;

namespace TrailEntities
{
    public class GameplayModel : Gameplay
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailEntities.GameplayModel" /> class.
        /// </summary>
        public GameplayModel()
        {
            // Get the currently executing assembly.
            var currentAssem = Assembly.GetExecutingAssembly();
            Console.WriteLine("Welcome to " + currentAssem.GetName().Name);
        }
    }
}