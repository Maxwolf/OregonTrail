using System.Collections.Generic;
using System.Collections.ObjectModel;
using TrailCommon;

namespace TrailEntities
{
    public class Person : IPerson
    {
        private FoodRations _ration;
        private RepairStatus _health;
        private uint _daysStarving;
        private Profession _profession;
        private string _name;
        private bool _isLeader;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:TrailEntities.Person"/> class.
        /// </summary>
        public Person(Profession profession, string name, bool isLeader)
        {
            _profession = profession;
            _name = name;
            _isLeader = isLeader;
            _daysStarving = 0;
            _health = RepairStatus.Good;
            _ration = FoodRations.Filling;
        }

        public FoodRations Ration
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

        public void Eat(FoodRations amount)
        {
            throw new System.NotImplementedException();
        }

        public void Rest()
        {
            throw new System.NotImplementedException();
        }

        public void RepairVehicle()
        {
            throw new System.NotImplementedException();
        }

        public bool IsStarving()
        {
            throw new System.NotImplementedException();
        }

        public bool IsDead()
        {
            throw new System.NotImplementedException();
        }
    }
}