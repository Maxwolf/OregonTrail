using System;
using TrailCommon;

namespace TrailEntities
{
    public sealed class RiverPoint : PointOfInterest, IRiverPoint
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailCommon.PointOfInterest" /> class.
        /// </summary>
        public RiverPoint(string name, ulong distanceLength) : base(name, distanceLength)
        {
        }

        public override SimulationMode Mode
        {
            get { return SimulationMode.RiverCrossing; }
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