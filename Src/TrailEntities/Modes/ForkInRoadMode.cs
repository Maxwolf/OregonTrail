using System.Collections.Generic;
using System.Collections.ObjectModel;
using TrailCommon;

namespace TrailEntities
{
    public class ForkInRoadMode : GameMode, IForkInRoad
    {
        private readonly List<PointOfInterest> _skipChoices;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailEntities.GameMode" /> class.
        /// </summary>
        public ForkInRoadMode(IGameServer game) : base(game)
        {
            _skipChoices = new List<PointOfInterest>();
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