namespace TrailEntities
{
    /// <summary>
    ///     Little class that will help me build a nice looking table in the scoring help states in the management options.
    /// </summary>
    public sealed class HealthScoring
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailEntities.HealthScoring" /> class.
        /// </summary>
        public HealthScoring(RepairStatus partyHealth, uint pointsPerPerson)
        {
            PartyHealth = partyHealth;
            PointsPerPerson = pointsPerPerson;
        }

        public RepairStatus PartyHealth { get; }
        public uint PointsPerPerson { get; }
    }
}