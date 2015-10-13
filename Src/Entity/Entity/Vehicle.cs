using System.Collections.Generic;
using System.Collections.ObjectModel;
using OregonTrail.Common;

namespace OregonTrail.Entity
{
    public class Vehicle : EntityBase, IVehicle
    {
        private uint _distanceTraveled;
        private List<ItemBase> _parts;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:OregonTrail.Entity.Vehicle" /> class.
        /// </summary>
        public Vehicle(ConditionTier condition, string name, uint distanceTraveled, List<ItemBase> parts)
            : base(condition, name)
        {
            _distanceTraveled = distanceTraveled;
            _parts = parts;
        }

        public uint DistanceTraveled
        {
            get { return _distanceTraveled; }
        }

        public ReadOnlyCollection<ItemBase> Parts
        {
            get { return new ReadOnlyCollection<ItemBase>(_parts); }
        }
    }
}