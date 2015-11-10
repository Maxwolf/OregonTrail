namespace TrailEntities.Simulation
{
    /// <summary>
    ///     Defines an object that keeps track of a particular high score of a given simulation round. This includes the name
    ///     of the person for bragging rights, points they earned in total at the end of the trip, and the overall rating this
    ///     game them.
    /// </summary>
    public sealed class Highscore
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailEntities.Simulation.Highscore" /> class.
        /// </summary>
        public Highscore(string name, int points, Performance rating)
        {
            Name = name;
            Points = points;
            Rating = rating;
        }

        public string Name { get; }
        public int Points { get; }
        public Performance Rating { get; }
    }
}