using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace OregonTrail
{
    public class Vehicle : Entity, IVehicle
    {
        private LocationType _currentLocation;
        private uint _distanceTraveled;
        private List<Item> _parts;

        public Vehicle(uint distanceTraveled, List<Item> parts, LocationType currentLocation)
        {
            _distanceTraveled = distanceTraveled;
            _parts = parts;
            _currentLocation = currentLocation;
        }

        public uint DistanceTraveled
        {
            get { return _distanceTraveled; }
        }

        public ReadOnlyCollection<Item> Parts
        {
            get { return new ReadOnlyCollection<Item>(_parts); }
        }

        public LocationType CurrentLocation
        {
            get { return _currentLocation; }
        }
    }
}