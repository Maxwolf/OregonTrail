using System;

namespace TrailEntities
{
    public sealed class River : Location
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailCommon.Location" /> class.
        /// </summary>
        public River(string name, int distanceLength) : base(name, distanceLength)
        {
        }

        public override ModeType ModeType
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