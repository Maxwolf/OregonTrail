using System.Collections.Generic;

namespace TrailCommon
{
    public interface ITrail
    {
        SortedSet<PointOfInterest> PointsOfInterest { get; }
        int VehicleLocation { get; }
        void ReachedPointOfInterest();
    }
}