using System.Collections.Generic;
using TrailCommon;

namespace TrailEntities
{
    public sealed class ForkInRoadPoint : PointOfInterest, IForkInRoadPoint
    {
        private HashSet<PointOfInterest> _skipChoices;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailEntities.ForkInRoad" /> class.
        /// </summary>
        public ForkInRoadPoint(string name, ulong distanceLength, IEnumerable<PointOfInterest> skipChoices)
            : base(name, distanceLength)
        {
            _skipChoices = new HashSet<PointOfInterest>(skipChoices);
        }

        public override ModeType ModeType
        {
            get { return ModeType.ForkInRoad; }
        }

        public IEnumerable<PointOfInterest> SkipChoices
        {
            get { return _skipChoices; }
        }
    }
}