using System.Collections.Generic;

namespace TrailEntities
{
    public sealed class ForkInRoad : Location
    {
        private HashSet<Location> _skipChoices;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailEntities.ForkInRoad" /> class.
        /// </summary>
        public ForkInRoad(string name, ulong distanceLength, IEnumerable<Location> skipChoices)
            : base(name, distanceLength)
        {
            _skipChoices = new HashSet<Location>(skipChoices);
        }

        public override ModeType ModeType
        {
            get { return ModeType.ForkInRoad; }
        }

        public IEnumerable<Location> SkipChoices
        {
            get { return _skipChoices; }
        }
    }
}