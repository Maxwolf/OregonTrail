using System;
using TrailCommon;

namespace TrailEntities
{
    public class River : PointOfInterest, IRiver
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:TrailCommon.PointOfInterest"/> class.
        /// </summary>
        public River(string name, ulong distanceLength) : base(name, distanceLength)
        {
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