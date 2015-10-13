using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace OregonTrail
{
    public class Party : Entity, IParty
    {
        private List<Person> _members;
        private Vehicle _vessel;

        public Party(List<Person> members, Vehicle vessel)
        {
            _members = members;
            _vessel = vessel;
        }

        public ReadOnlyCollection<Person> Members
        {
            get { return new ReadOnlyCollection<Person>(_members); }
        }

        public Vehicle Vessel
        {
            get { return _vessel; }
        }
    }
}