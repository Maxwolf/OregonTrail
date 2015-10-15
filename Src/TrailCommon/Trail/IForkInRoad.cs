using System.Collections.Generic;

namespace TrailCommon
{
    public interface IForkInRoad
    {
        List<PointOfInterest> SkipChoices { get; }
    }
}