using System;
using TrailCommon;

namespace TrailEntities
{
    public class River : PointOfInterest, IRiver
    {
        private readonly uint _depth;
        private readonly uint _ferryCost;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailCommon.LocationBase" /> class.
        /// </summary>
        public River(string name, uint depth, uint ferryCost) : base(name)
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