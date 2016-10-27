// Created by Ron 'Maxwolf' McDowell (ron.mcdowell@gmail.com) 
// Timestamp 01/03/2016@1:50 AM

using OregonTrailDotNet.Entity.Person;

namespace OregonTrailDotNet.Module.Scoring
{
    /// <summary>
    ///     Little class that will help me build a nice looking table in the scoring help states in the management options.
    /// </summary>
    public sealed class HealthScoring
    {
        /// <summary>Initializes a new instance of the <see cref="T:OregonTrailDotNet.Module.Scoring.HealthScoring" /> class.</summary>
        /// <param name="partyHealthStatus">The party Health Level.</param>
        /// <param name="pointsPerPerson">The points Per Person.</param>
        public HealthScoring(HealthStatus partyHealthStatus, int pointsPerPerson)
        {
            PartyHealthStatus = partyHealthStatus;
            PointsPerPerson = pointsPerPerson;
        }

        /// <summary>
        ///     Gets the party health level.
        /// </summary>
        public HealthStatus PartyHealthStatus { get; }

        /// <summary>
        ///     Gets the points per person.
        /// </summary>
        public int PointsPerPerson { get; }
    }
}