using System;
using TrailCommon;

namespace TrailEntities
{
    /// <summary>
    ///     Manages a boolean event where the player needs to make a choice before they can move onto the next location on the
    ///     trail. Depending on the outcome of this event the player party may lose items, people, or parts depending on how
    ///     bad it is.
    /// </summary>
    public class RiverCrossing : PointOfInterest, IRiverCrossing, IRiver
    {
        private uint _depth;
        private uint _ferryCost;
        private IVehicle _vehicle;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailCommon.PointOfInterest" /> class.
        /// </summary>
        public RiverCrossing(string name, ulong distanceLength) : base(name, distanceLength)
        {
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

        public void TickMode()
        {
            throw new NotImplementedException();
        }

        public IVehicle Vehicle
        {
            get { return _vehicle; }
        }

        public void CaulkVehicle()
        {
            throw new NotImplementedException();
        }

        public void Ford()
        {
            throw new NotImplementedException();
        }

        public void UseFerry()
        {
            throw new NotImplementedException();
        }

        public void UpdateVehicle()
        {
            throw new NotImplementedException();
        }

        public override ModeType Mode
        {
            get { return ModeType.RiverCrossing; }
        }
    }
}