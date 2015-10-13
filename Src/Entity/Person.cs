using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace OregonTrail
{
    public class Person : Entity, IPerson
    {
        private List<Disease> _ailments;
        private uint _money;

        public Person(uint money)
        {
            _money = money;
            _ailments = new List<Disease>();
        }

        public uint Money
        {
            get { return _money; }
        }

        public ReadOnlyCollection<Disease> Ailments
        {
            get { return new ReadOnlyCollection<Disease>(_ailments); }
        }
    }
}