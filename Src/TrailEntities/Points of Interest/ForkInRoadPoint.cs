using System.Collections.Generic;
using System.Collections.ObjectModel;
using TrailCommon;

namespace TrailEntities
{
    public sealed class ForkInRoadPoint : PointOfInterest, IForkInRoadPoint
    {
        private List<PointOfInterest> _skipChoices;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailEntities.ForkInRoad" /> class.
        /// </summary>
        public ForkInRoadPoint(string name, ulong distanceLength, List<PointOfInterest> skipChoices)
            : base(name, distanceLength)
        {
            _skipChoices = skipChoices;
        }

        public override ModeType Mode
        {
            get { return ModeType.ForkInRoad; }
        }

        public ReadOnlyCollection<PointOfInterest> SkipChoices
        {
            get { return new ReadOnlyCollection<PointOfInterest>(_skipChoices); }
        }
    }
}