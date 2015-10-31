using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace TrailCommon
{
    public interface IForkInRoadPoint
    {
        IEnumerable<PointOfInterest> SkipChoices { get; }
    }
}