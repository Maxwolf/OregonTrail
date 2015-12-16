using TrailSimulation.Core;

namespace TrailSimulation.Game
{
    /// <summary>
    ///     Defines an object that keeps track of a particular high score of a given simulation round. This includes the name
    ///     of the person for bragging rights, points they earned in total at the end of the trip, and the overall rating this
    ///     game them.
    /// </summary>
    public sealed class Highscore
    {
        /// <summary>
        ///     Internal enumeration value for the score the player actually had as enumeration value, we convert this to string
        ///     when asked for it.
        /// </summary>
        private readonly Performance _rating;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailSimulation.Game.Highscore" /> class.
        /// </summary>
        public Highscore(string name, int points)
        {
            // PassengerLeader of party and total number of points.
            Name = name;
            Points = points;

            // Rank the players performance based on the number of points they have.
            if (points >= 7000)
                _rating = Performance.TrailGuide;
            else if (points >= 3000 && points < 7000)
                _rating = Performance.Adventurer;
            else if (points < 3000)
                _rating = Performance.Greenhorn;
        }

        /// <summary>
        ///     Names of the leader of the vehicle party.
        /// </summary>
        public string Name { get; }

        /// <summary>
        ///     Total number of points the player accumulated.
        /// </summary>
        public int Points { get; }

        /// <summary>
        ///     Stores an enumeration as read only inside high score object, returns a string for the rating using extension method
        ///     to get description attribute so it looks correct when rendered and shown to users.
        /// </summary>
        public string Rating
        {
            get { return _rating.ToDescriptionAttribute(); }
        }
    }
}