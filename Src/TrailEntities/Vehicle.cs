using System.Collections.Generic;
using System.Collections.ObjectModel;
using TrailCommon;

namespace TrailEntities
{
    public class Vehicle : EntityBase, IVehicle
    {
        private uint _distanceTraveled;
        private List<IItem> _parts;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailEntities.Vehicle" /> class.
        /// </summary>
        public Vehicle(ConditionTier condition, string name, uint distanceTraveled, List<IItem> parts)
            : base(condition, name)
        {
            _distanceTraveled = distanceTraveled;
            _parts = parts;
        }

        public uint DistanceTraveled
        {
            get { return _distanceTraveled; }
        }

        public ReadOnlyCollection<IItem> Parts
        {
            get { return new ReadOnlyCollection<IItem>(_parts); }
        }
    }
}