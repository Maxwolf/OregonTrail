using System;
using System.Collections.ObjectModel;
using TrailCommon;

namespace TrailEntities
{
    public class Trail : ITrail
    {
        private ReadOnlyCollection<PointOfInterest> _pointsOfInterest;
        private int _vehicleLocation;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailEntities.Trail" /> class.
        /// </summary>
        public Trail()
        {
            _pointsOfInterest = new ReadOnlyCollection<PointOfInterest>(TrailPrefabs.OregonTrail);
            _vehicleLocation = 0;
        }

        public int VehicleLocation
        {
            get { return _vehicleLocation; }
        }

        public ReadOnlyCollection<PointOfInterest> PointsOfInterest
        {
            get { return _pointsOfInterest; }
        }

        public void ReachedPointOfInterest()
        {
            throw new NotImplementedException();
        }
    }
}