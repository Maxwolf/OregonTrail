// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HealthScoring.cs" company="Ron 'Maxwolf' McDowell">
//   ron.mcdowell@gmail.com
// </copyright>
// <summary>
//   Little class that will help me build a nice looking table in the scoring help states in the management options.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace TrailSimulation.Game
{
    using Entity;

    /// <summary>
    ///     Little class that will help me build a nice looking table in the scoring help states in the management options.
    /// </summary>
    public sealed class HealthScoring
    {
        /// <summary>Initializes a new instance of the <see cref="T:TrailSimulation.Game.HealthScoring"/> class.</summary>
        /// <param name="partyHealthLevel">The party Health Level.</param>
        /// <param name="pointsPerPerson">The points Per Person.</param>
        public HealthScoring(HealthLevel partyHealthLevel, int pointsPerPerson)
        {
            PartyHealthLevel = partyHealthLevel;
            PointsPerPerson = pointsPerPerson;
        }

        /// <summary>
        ///     Gets the party health level.
        /// </summary>
        public HealthLevel PartyHealthLevel { get; }

        /// <summary>
        ///     Gets the points per person.
        /// </summary>
        public int PointsPerPerson { get; }
    }
}