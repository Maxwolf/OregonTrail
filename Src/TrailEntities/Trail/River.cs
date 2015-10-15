using System;
using TrailCommon;

namespace TrailEntities
{
    public class River : PointOfInterest, IRiver
    {
        private uint _depth;
        private uint _ferryCost;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:TrailEntities.PointOfInterest"/> class.
        /// </summary>
        public River(string name, ulong distanceLength, uint depth, uint ferryCost) : base(name, distanceLength)
        {
            _depth = depth;
            _ferryCost = ferryCost;
        }

        public uint Depth
        {
            get { return _depth; }
        }

        public uint FerryCost
        {
            get { return _ferryCost; }
        }

        public void CrossRiver()
        {
            throw new NotImplementedException();
        }
    }
}