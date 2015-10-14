using System;
using System.Collections.Generic;

namespace TrailCommon
{
    public class Trail : ITrail
    {
        private SortedSet<PointOfInterest> _pointsOfInterest;
        private int _vehicleLocation;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailCommon.Trail" /> class.
        /// </summary>
        public Trail()
        {
            _pointsOfInterest = new SortedSet<PointOfInterest>();
            _vehicleLocation = 0;
        }

        public SortedSet<PointOfInterest> PointsOfInterest
        {
            get { return _pointsOfInterest; }
        }

        public int VehicleLocation
        {
            get { return _vehicleLocation; }
        }

        public void ReachedPointOfInterest()
        {
            throw new NotImplementedException();
        }
    }
}