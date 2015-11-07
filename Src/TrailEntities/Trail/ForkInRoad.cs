using System.Collections.Generic;

namespace TrailEntities
{
    /// <summary>
    ///     Offers up a decision when traveling on the trail, there are normally one of many possible outcomes from which the
    ///     player can choose. The trade-off generally is
    /// </summary>
    public sealed class ForkInRoad : Location
    {
        private HashSet<Location> _skipChoices;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailEntities.ForkInRoad" /> class.
        /// </summary>
        public ForkInRoad(string name, IEnumerable<Location> skipChoices) : base(name)
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