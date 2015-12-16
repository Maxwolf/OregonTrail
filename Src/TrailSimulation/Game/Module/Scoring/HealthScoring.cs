using TrailSimulation.Entity;

namespace TrailSimulation.Game
{
    /// <summary>
    ///     Little class that will help me build a nice looking table in the scoring help states in the management options.
    /// </summary>
    public sealed class HealthScoring
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailSimulation.Game.HealthScoring" /> class.
        /// </summary>
        public HealthScoring(HealthLevel partyHealthLevel, int pointsPerPerson)
        {
            PartyHealthLevel = partyHealthLevel;
            PointsPerPerson = pointsPerPerson;
        }

        public HealthLevel PartyHealthLevel { get; }
        public int PointsPerPerson { get; }
    }
}