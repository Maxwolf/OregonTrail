using System;
using TrailCommon;

namespace TrailEntities
{
    public sealed class Person : IPerson
    {
        private uint _daysStarving;
        private RepairStatus _health;
        private bool _isLeader;
        private string _name;
        private Profession _profession;
        private RationLevel _ration;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailEntities.Person" /> class.
        /// </summary>
        public Person(Profession profession, string name, bool isLeader)
        {
            _profession = profession;
            _name = name;
            _isLeader = isLeader;
            _daysStarving = 0;
            _health = RepairStatus.Good;
            _ration = RationLevel.Filling;
        }

        public RationLevel Ration
        {
            get { return _ration; }
        }

        public RepairStatus Health
        {
            get { return _health; }
        }

        public uint DaysStarving
        {
            get { return _daysStarving; }
        }

        public Profession Profession
        {
            get { return _profession; }
        }

        public string Name
        {
            get { return _name; }
        }

        public bool IsLeader
        {
            get { return _isLeader; }
        }

        public void Eat(RationLevel amount)
        {
            throw new NotImplementedException();
        }

        public void Rest()
        {
            throw new NotImplementedException();
        }

        public void RepairVehicle()
        {
            throw new NotImplementedException();
        }

        public bool IsStarving()
        {
            throw new NotImplementedException();
        }

        public bool IsDead()
        {
            throw new NotImplementedException();
        }
    }
}