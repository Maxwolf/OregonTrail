using System;
using TrailCommon;

namespace TrailEntities
{
    public sealed class Person : IPerson
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailEntities.Person" /> class.
        /// </summary>
        public Person(Profession profession, string name, bool isLeader)
        {
            Profession = profession;
            Name = name;
            IsLeader = isLeader;
            DaysStarving = 0;
            Health = RepairStatus.Good;
            Ration = RationLevel.Filling;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailEntities.Person" /> class.
        /// </summary>
        public Person()
        {
            Profession = Profession.Banker;
            Name = "UNKNOWN PERSON";
            IsLeader = false;
            DaysStarving = 0;
            Health = RepairStatus.Good;
            Ration = RationLevel.Filling;
        }

        public RationLevel Ration { get; }

        public RepairStatus Health { get; }

        public uint DaysStarving { get; }

        public Profession Profession { get; }

        public string Name { get; }

        public bool IsLeader { get; }

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