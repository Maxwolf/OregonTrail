using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace OregonTrail
{
    public class Person : EntityBase, IPerson
    {
        private List<Disease> _ailments;
        private uint _money;
        private readonly PersonTier _socialStatus;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:OregonTrail.Person" /> class.
        /// </summary>
        public Person(Condition condition, string name, uint money, PersonTier socialStatus) : base(condition, name)
        {
            _ailments = new List<Disease>();
            _money = money;
            _socialStatus = socialStatus;
        }

        public uint Money
        {
            get { return _money; }
        }

        public ReadOnlyCollection<Disease> Ailments
        {
            get { return new ReadOnlyCollection<Disease>(_ailments); }
        }

        public PersonTier SocialStatus
        {
            get { return _socialStatus; }
        }
    }
}