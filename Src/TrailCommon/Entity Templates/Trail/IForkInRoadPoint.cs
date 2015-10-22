using System.Collections.ObjectModel;

namespace TrailCommon
{
    public interface IForkInRoadPoint
    {
        ReadOnlyCollection<PointOfInterest> SkipChoices { get; }
    }
}