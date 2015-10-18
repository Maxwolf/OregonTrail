using System;
using TrailCommon;

namespace TrailEntities
{
    public sealed class River : PointOfInterest, IRiver
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailCommon.PointOfInterest" /> class.
        /// </summary>
        public River(string name, ulong distanceLength) : base(name, distanceLength)
        {
        }

        public override ModeType Mode
        {
            get { return ModeType.RiverCrossing; }
        }

        public uint Depth
        {
            get { throw new NotImplementedException(); }
        }

        public uint FerryCost
        {
            get { throw new NotImplementedException(); }
        }

        public void CrossRiver()
        {
            throw new NotImplementedException();
        }
    }
}