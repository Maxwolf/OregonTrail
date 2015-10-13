using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace OregonTrail
{
    public class Vehicle : Entity, IVehicle
    {
        private uint _distanceTraveled;
        private List<Item> _parts;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:OregonTrail.Vehicle" /> class.
        /// </summary>
        public Vehicle(Condition condition, string name, uint distanceTraveled, List<Item> parts)
            : base(condition, name)
        {
            _distanceTraveled = distanceTraveled;
            _parts = parts;
        }

        public uint DistanceTraveled
        {
            get { return _distanceTraveled; }
        }

        public ReadOnlyCollection<Item> Parts
        {
            get { return new ReadOnlyCollection<Item>(_parts); }
        }
    }
}