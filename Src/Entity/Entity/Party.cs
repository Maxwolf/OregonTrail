using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace OregonTrail
{
    public class Party : Entity, IParty
    {
        private LocationType _currentLocation;
        private List<Person> _members;
        private Vehicle _vessel;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:OregonTrail.Party" /> class.
        /// </summary>
        public Party(Condition condition, string name, List<Person> members, Vehicle vessel,
            LocationType currentLocation) : base(condition, name)
        {
            _members = members;
            _vessel = vessel;
            _currentLocation = currentLocation;
        }

        public ReadOnlyCollection<Person> Members
        {
            get { return new ReadOnlyCollection<Person>(_members); }
        }

        public Vehicle Vessel
        {
            get { return _vessel; }
        }

        public LocationType CurrentLocation
        {
            get { return _currentLocation; }
        }
    }
}