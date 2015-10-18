using System.Collections.ObjectModel;

namespace TrailCommon
{
    public interface IForkInRoad
    {
        ReadOnlyCollection<PointOfInterest> SkipChoices { get; }
    }
}