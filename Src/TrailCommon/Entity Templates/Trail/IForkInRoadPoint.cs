using System.Collections.Generic;

namespace TrailCommon
{
    public interface IForkInRoadPoint
    {
        IEnumerable<PointOfInterest> SkipChoices { get; }
    }
}