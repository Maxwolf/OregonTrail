using System;
using TrailCommon;

namespace TrailEntities
{
    /// <summary>
    ///     Manages a boolean event where the player needs to make a choice before they can move onto the next location on the
    ///     trail. Depending on the outcome of this event the player party may lose items, people, or parts depending on how
    ///     bad it is.
    /// </summary>
    public class RiverCrossing : River, IRiverCrossing
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:TrailCommon.PointOfInterest"/> class.
        /// </summary>
        public RiverCrossing(string name, ulong distanceLength) : base(name, distanceLength)
        {
        }

        public TrailModeType Mode
        {
            get { throw new NotImplementedException(); }
        }

        public ITrailVehicle TrailVehicle
        {
            get { throw new NotImplementedException(); }
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
    }
}