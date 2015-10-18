using System.Collections.Generic;
using System.Collections.ObjectModel;
using TrailCommon;

namespace TrailEntities
{
    public class ForkInRoad : Landmark, IForkInRoad
    {
        private List<PointOfInterest> _skipChoices;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailEntities.ForkInRoad" /> class.
        /// </summary>
        public ForkInRoad(string name, ulong distanceLength, bool canRest, List<PointOfInterest> skipChoices)
            : base(name, distanceLength, canRest)
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