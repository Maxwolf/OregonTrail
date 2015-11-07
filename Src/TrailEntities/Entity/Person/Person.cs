using System;
using System.Diagnostics;
using System.Linq;

namespace TrailEntities
{
    public sealed class Person : IEntity
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

        public RepairStatus Health { get; private set; }

        public int DaysStarving { get; }

        public Profession Profession { get; }

        public bool IsLeader { get; }

        public string Name { get; }

        /// <summary>
        ///     Defines what type of entity this will take the role of in the simulation. Depending on this value the simulation
        ///     will affect how it is treated, points tabulated, and interactions governed.
        /// </summary>
        public SimEntity Category
        {
            get { return SimEntity.Person; }
        }

        /// <summary>
        ///     Compares two objects and returns a value indicating whether one is less than, equal to, or greater than the other.
        /// </summary>
        /// <returns>
        ///     A signed integer that indicates the relative values of <paramref name="x" /> and <paramref name="y" />, as shown in
        ///     the following table.Value Meaning Less than zero<paramref name="x" /> is less than <paramref name="y" />.Zero
        ///     <paramref name="x" /> equals <paramref name="y" />.Greater than zero<paramref name="x" /> is greater than
        ///     <paramref name="y" />.
        /// </returns>
        /// <param name="x">The first object to compare.</param>
        /// <param name="y">The second object to compare.</param>
        public int Compare(IEntity x, IEntity y)
        {
            Debug.Assert(x != null, "x != null");
            Debug.Assert(y != null, "y != null");

            var result = string.Compare(x.Name, y.Name, StringComparison.Ordinal);
            if (result != 0) return result;

            return result;
        }

        /// <summary>
        ///     Compares the current object with another object of the same type.
        /// </summary>
        /// <returns>
        ///     A value that indicates the relative order of the objects being compared. The return value has the following
        ///     meanings: Value Meaning Less than zero This object is less than the <paramref name="other" /> parameter.Zero This
        ///     object is equal to <paramref name="other" />. Greater than zero This object is greater than
        ///     <paramref name="other" />.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public int CompareTo(IEntity other)
        {
            Debug.Assert(other != null, "other != null");

            var result = string.Compare(other.Name, Name, StringComparison.Ordinal);
            if (result != 0) return result;

            return result;
        }

        /// <summary>
        ///     Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        ///     true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public bool Equals(IEntity other)
        {
            // Reference equality check
            if (this == other)
            {
                return true;
            }

            if (other == null)
            {
                return false;
            }

            if (other.GetType() != GetType())
            {
                return false;
            }

            if (Name.Equals(other.Name))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        ///     Determines whether the specified objects are equal.
        /// </summary>
        /// <returns>
        ///     true if the specified objects are equal; otherwise, false.
        /// </returns>
        public bool Equals(IEntity x, IEntity y)
        {
            return x.Equals(y);
        }

        /// <summary>
        ///     Returns a hash code for the specified object.
        /// </summary>
        /// <returns>
        ///     A hash code for the specified object.
        /// </returns>
        /// <param name="obj">The <see cref="T:System.Object" /> for which a hash code is to be returned.</param>
        /// <exception cref="T:System.ArgumentNullException">
        ///     The type of <paramref name="obj" /> is a reference type and
        ///     <paramref name="obj" /> is null.
        /// </exception>
        public int GetHashCode(IEntity obj)
        {
            var hash = 23;
            hash = (hash*31) + Name.GetHashCode();
            return hash;
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

        /// <summary>
        ///     Determines the amount of miles the party is able to travel with a given individual. Will check for Illness, cold
        ///     weather, starvation from having zero food, healing when resting, and if needed killing them off from simulation.
        /// </summary>
        public void TickPerson()
        {
            if (100*GameSimApp.Instance.Random.NextDouble() < 
                10 + 35*((int) GameSimApp.Instance.Vehicle.Ration - 1))
            {
                // Mild illness.
                GameSimApp.Instance.Vehicle.ReduceMileage(5);
                Health = RepairStatus.Fair;
            }
            else if (100*GameSimApp.Instance.Random.NextDouble() <
                100 - (40/GameSimApp.Instance.Vehicle.Passengers.Count()*((int) GameSimApp.Instance.Vehicle.Ration - 1)))
            {
                // Bad illness.
                Health = RepairStatus.Poor;
            }
            else
            {
                // Severe illness.
                Health = RepairStatus.VeryPoor;
                // TODO: Pick an actual severe illness from list, roll the dice for it on very low health.
            }

            // Check if you have been killed by illness, 
            if (Health == RepairStatus.VeryPoor &&
                GameSimApp.Instance.Random.Next((int)Health) <= 0)
            {
                // TODO: Check if leader died or party member.
                GameSimApp.Instance.AddMode(ModeType.EndGame);
            }
        }
    }
}