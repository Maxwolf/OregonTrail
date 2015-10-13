using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace OregonTrail
{
    public class Person : Entity, IPerson
    {
        private List<Disease> _ailments;
        private uint _money;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:OregonTrail.Person" /> class.
        /// </summary>
        public Person(Condition condition, string name, List<Disease> ailments, uint money) : base(condition, name)
        {
            _ailments = ailments;
            _money = money;
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