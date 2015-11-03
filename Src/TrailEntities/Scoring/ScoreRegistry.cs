using System.Collections.Generic;

namespace TrailEntities
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
        ///     Build up a list of repair status scoring for people objects.
        /// </summary>
        public static IEnumerable<HealthScoring> PeoplePoints
        {
            get
            {
                return new List<HealthScoring>
                {
                    new HealthScoring(RepairStatus.Good, 500),
                    new HealthScoring(RepairStatus.Fair, 400),
                    new HealthScoring(RepairStatus.Poor, 300),
                    new HealthScoring(RepairStatus.VeryPoor, 200)
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
                    new Points(typeof (Vehicle), 50),
                    new Points(typeof (Oxen), 4),
                    new Points(typeof (Part), 2),
                    new Points(typeof (Clothing), 2),
                    new Points(typeof (Bullets), 1, 50),
                    new Points(typeof (Food), 1, 25),
                    new Points(typeof (uint), 1, 5, "Cash")
                };
            }
        }
    }
}