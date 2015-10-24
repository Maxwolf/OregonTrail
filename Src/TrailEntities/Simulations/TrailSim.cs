using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using TrailCommon;

namespace TrailEntities
{
    /// <summary>
    ///     Holds all the points of interest that make up the entire trail the players vehicle will be traveling along. Keeps
    ///     track of the vehicles current position on the trail and provides helper methods to quickly access it.
    /// </summary>
    public sealed class TrailSim : ITrail
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailEntities.Trail" /> class.
        /// </summary>
        /// <param name="trail">Collection of points of interest which make up the trail the player is going to travel.</param>
        public TrailSim(IList<PointOfInterest> trail)
        {
            PointsOfInterest = new ReadOnlyCollection<PointOfInterest>(trail);
            VehicleLocation = 0;
        }

        public int VehicleLocation { get; }

        public ReadOnlyCollection<PointOfInterest> PointsOfInterest { get; }

        public void ReachedPointOfInterest()
        {
            throw new NotImplementedException();
        }

        public PointOfInterest GetCurrentPointOfInterest()
        {
            return PointsOfInterest[VehicleLocation];
        }
    }
}