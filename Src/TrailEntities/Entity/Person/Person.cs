using System;
using System.Diagnostics;
using System.Linq;

namespace TrailEntities
{
    /// <summary>
    ///     Represents a human-being. Gender is not tracked, we only care about them as an entity that consumes food and their
    ///     health.
    /// </summary>
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

        /// <summary>
        ///     Determines how much food every day this person eats in pounds.
        /// </summary>
        public RationLevel Ration { get; }

        /// <summary>
        ///     Current health of this person which is enum that also represents the total points they are currently worth.
        /// </summary>
        public RepairStatus Health { get; private set; }

        /// <summary>
        ///     Determines how many total consecutive days this player has not eaten any food. If this continues for more than five
        ///     (5) days then the probability they will die increases exponentially.
        /// </summary>
        public int DaysStarving { get; }

        /// <summary>
        ///     Profession of this person, typically if the leader is a banker then the entire family is all bankers for sanity
        ///     sake.
        /// </summary>
        public Profession Profession { get; }

        /// <summary>
        ///     Determines if this person is the party leader, without this person the game will end. The others cannot go on
        ///     without them.
        /// </summary>
        public bool IsLeader { get; }

        /// <summary>
        ///     Name of the person as they should be known by other players and the simulation.
        /// </summary>
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
            else if (100*GameSimApp.Instance.Random.NextDouble() < 100 -
                     (40/GameSimApp.Instance.Vehicle.Passengers.Count()*((int) GameSimApp.Instance.Vehicle.Ration - 1)))
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

            // Check if party leader or a member of it has been killed by an illness.
            if (Health == RepairStatus.VeryPoor &&
                GameSimApp.Instance.Random.Next((int) Health) <= 0)
            {
                // Check if leader died or party member.
                if (IsLeader)
                {
                    GameSimApp.Instance.AddMode(ModeType.EndGame);
                }
                else
                {
                    // Fire event that a party member has died.
                    GameSimApp.Instance.Director.TriggerEvent("DeathPlayer");
                }
            }
        }
    }
}