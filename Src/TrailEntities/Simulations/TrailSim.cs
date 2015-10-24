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
        ///     Current location of the players vehicle as index of points of interest list.
        /// </summary>
        private int _vehicleLocation;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailEntities.Trail" /> class.
        /// </summary>
        /// <param name="trail">Collection of points of interest which make up the trail the player is going to travel.</param>
        public TrailSim(IList<PointOfInterest> trail)
        {
            PointsOfInterest = new ReadOnlyCollection<PointOfInterest>(trail);
            _vehicleLocation = 0;
        }

        /// <summary>
        ///     Current location of the players vehicle as index of points of interest list.
        /// </summary>
        public int VehicleLocation
        {
            get { return _vehicleLocation; }
        }

        /// <summary>
        ///     List of all of the points of interest that make up the entire trail.
        /// </summary>
        public ReadOnlyCollection<PointOfInterest> PointsOfInterest { get; }

        /// <summary>
        ///     Advances the vehicle to the next point of interest on the path.
        /// </summary>
        public void ReachedPointOfInterest()
        {
            if (_vehicleLocation < PointsOfInterest.Count)
                _vehicleLocation++;
        }

        /// <summary>
        ///     Returns the current point of interest the players vehicle is on.
        /// </summary>
        public PointOfInterest GetCurrentPointOfInterest()
        {
            return PointsOfInterest[VehicleLocation];
        }
    }
}