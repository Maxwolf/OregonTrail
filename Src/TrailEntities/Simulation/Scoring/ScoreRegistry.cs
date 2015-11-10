using System.Collections.Generic;
using TrailEntities.Entity;

namespace TrailEntities.Simulation
{
    /// <summary>
    ///     References a list of the original high scores that the game will default to if there are no custom user ones to
    ///     load.
    /// </summary>
    public static class ScoreRegistry
    {
        /// <summary>
        ///     Original high scores from Apple II version of the game.
        /// </summary>
        public static IEnumerable<Highscore> TopTenDefaults
        {
            get
            {
                return new List<Highscore>
                {
                    new Highscore("Stephen Meek", 7650, Performance.TrailGuide),
                    new Highscore("Celinda Hines", 5694, Performance.Adventurer),
                    new Highscore("Andrew Sublette", 4138, Performance.Adventurer),
                    new Highscore("David Hastings", 2945, Performance.Greenhorn),
                    new Highscore("Ezra Meeker", 2052, Performance.Greenhorn),
                    new Highscore("Willian Vaughn", 1401, Performance.Greenhorn),
                    new Highscore("Mary Bartlett", 937, Performance.Greenhorn),
                    new Highscore("Willian Wiggins", 615, Performance.Greenhorn),
                    new Highscore("Charles Hopper", 396, Performance.Greenhorn),
                    new Highscore("Elijah White", 250, Performance.Greenhorn)
                };
            }
        }

        /// <summary>
        ///     Reference to points that will be given for entities of given matching types in this list.
        /// </summary>
        public static IEnumerable<Points> ResourcePoints
        {
            get
            {
                return new List<Points>
                {
                    new Points(Resources.Person, 800),
                    new Points(Resources.Vehicle, 50),
                    new Points(Parts.Oxen, 4),
                    new Points(Parts.Wheel, 2),
                    new Points(Parts.Axle, 2),
                    new Points(Parts.Tongue, 2),
                    new Points(Resources.Clothing, 2),
                    new Points(Resources.Bullets, 1, 50),
                    new Points(Resources.Food, 1, 25),
                    new Points(Resources.Cash, 1, 5)
                };
            }
        }
    }
}