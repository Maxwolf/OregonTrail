using System.Collections.Generic;

namespace TrailEntities
{
    public interface IForkInRoadPoint
    {
        IEnumerable<PointOfInterest> SkipChoices { get; }
    }
}