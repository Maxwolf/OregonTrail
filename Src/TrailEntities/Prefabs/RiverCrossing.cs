using System;
using TrailCommon;

namespace TrailEntities
{
    /// <summary>
    ///     Manages a boolean event where the player needs to make a choice before they can move onto the next location on the
    ///     trail. Depending on the outcome of this event the player party may lose items, people, or parts depending on how
    ///     bad it is.
    /// </summary>
    public abstract class RiverCrossing : PointOfInterest, IRiverCrossing
    {
        private readonly IVehicle _vehicle;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailCommon.LocationBase" /> class.
        /// </summary>
        protected RiverCrossing(string name, IVehicle vehicle) : base(name)
        {
            _vehicle = vehicle;
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
    }
}