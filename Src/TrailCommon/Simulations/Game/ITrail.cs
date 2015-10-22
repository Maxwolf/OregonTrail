using System.Collections.ObjectModel;

namespace TrailCommon
{
    public interface ITrail
    {
        ReadOnlyCollection<PointOfInterest> PointsOfInterest { get; }
        int VehicleLocation { get; }
        void ReachedPointOfInterest();
    }
}