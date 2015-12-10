using System;
using System.Diagnostics;
using System.Linq;
using TrailSimulation.Event;
using TrailSimulation.Game;

namespace TrailSimulation.Entity
{
    /// <summary>
    ///     Represents a human-being. Gender is not tracked, we only care about them as an entity that consumes food and their
    ///     health.
    /// </summary>
    public sealed class Person : IEntity
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailEntities.Entities.Person" /> class.
        /// </summary>
        public Person(Profession profession, string name, bool isLeader)
        {
            Profession = profession;
            Name = name;
            IsLeader = isLeader;
            DaysStarving = 0;
            Health = Entity.Health.Good;
            Infection = Disease.None;
        }

        /// <summary>
        ///     Current health of this person which is enum that also represents the total points they are currently worth.
        /// </summary>
        public Health Health { get; set; }

        /// <summary>
        ///     Determines how many total consecutive days this player has not eaten any food. If this continues for more than five
        ///     (5) days then the probability they will die increases exponentially.
        /// </summary>
        public int DaysStarving { get; private set; }

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
        ///     Determines if this person is inflicted with a disease. It is possible to contract them from bad water, food, not
        ///     eating right, over working, stress, etc. Only one disease supported at a time to prevent people from getting
        ///     multiple infections since that would over complicate things for the player.
        /// </summary>
        public Disease Infection { get; private set; }

        /// <summary>
        ///     Determines if the person is dead and no longer consuming resources. Dead party members in the same vehicle will
        ///     lower total possible mileage for several turns as remaining people mourn the loss of the other.
        /// </summary>
        public bool IsDead { get; }

        /// <summary>
        ///     Name of the person as they should be known by other players and the simulation.
        /// </summary>
        public string Name { get; }

        /// <summary>
        ///     Defines what type of entity this will take the role of in the simulation. Depending on this value the simulation
        ///     will affect how it is treated, points tabulated, and interactions governed.
        /// </summary>
        public Entities Category
        {
            get { return Entities.Person; }
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
        ///     Called when the simulation is ticked by underlying operating system, game engine, or potato. Each of these system
        ///     ticks is called at unpredictable rates, however if not a system tick that means the simulation has processed enough
        ///     of them to fire off event for fixed interval that is set in the core simulation by constant in milliseconds.
        /// </summary>
        /// <remarks>Default is one second or 1000ms.</remarks>
        /// <param name="systemTick">
        ///     TRUE if ticked unpredictably by underlying operating system, game engine, or potato. FALSE if
        ///     pulsed by game simulation at fixed interval.
        /// </param>
        public void OnTick(bool systemTick)
        {
            // Only tick person with simulation.
            if (systemTick)
                return;

            // Dead people do not waste ticks living.
            if (IsDead)
                return;

            ConsumeFood();
            CheckIllness();
        }

        /// <summary>
        ///     Determines how much food party members in the vehicle will eat today.
        /// </summary>
        private void ConsumeFood()
        {
            var cost_food = GameSimulationApp.Instance.Vehicle.Inventory[Entities.Food].TotalValue;
            cost_food = cost_food - 8 - 5*(int) GameSimulationApp.Instance.Vehicle.Ration;
            if (cost_food >= 13)
            {
                // Consume the food since we still have some.
                GameSimulationApp.Instance.Vehicle.Inventory[Entities.Food] =
                    new SimItem(GameSimulationApp.Instance.Vehicle.Inventory[Entities.Food],
                        (int) cost_food);
            }
            else
            {
                // TODO: Complain about low food with event warning.

                // Otherwise we begin to starve.
                DaysStarving++;
            }
        }

        /// <summary>
        ///     Check if party leader or a member of it has been killed by an illness.
        /// </summary>
        private void CheckIllness()
        {
            if (100*GameSimulationApp.Instance.Random.NextDouble() <
                10 + 35*((int) GameSimulationApp.Instance.Vehicle.Ration - 1))
            {
                // Mild illness.
                GameSimulationApp.Instance.Vehicle.ReduceMileage(5);
                Health = Health.Fair;
            }
            else if (100*GameSimulationApp.Instance.Random.NextDouble() < 100 -
                     (40/GameSimulationApp.Instance.Vehicle.Passengers.Count()*
                      ((int) GameSimulationApp.Instance.Vehicle.Ration - 1)))
            {
                // Bad illness.
                GameSimulationApp.Instance.Vehicle.ReduceMileage(10);
                Health = Health.Poor;
            }
            else
            {
                // Severe illness.
                Health = Health.VeryPoor;
                GameSimulationApp.Instance.Vehicle.ReduceMileage(15);

                // Pick an actual severe illness from list, roll the dice for it on very low health.
                if (GameSimulationApp.Instance.Random.Next(100) >= 99 && Infection == Disease.None)
                    GameSimulationApp.Instance.EventDirector.TriggerEvent(this, typeof (InfectPlayer));
            }

            if (Health == Health.VeryPoor &&
                GameSimulationApp.Instance.Random.Next((int) Health) <= 0)
            {
                // Some dying makes everybody take a huge morale hit.
                GameSimulationApp.Instance.Vehicle.ReduceMileage(50);

                // Check if leader died or party member.
                GameSimulationApp.Instance.EventDirector.TriggerEvent(this, IsLeader
                    ? typeof (DeathPlayer)
                    : typeof (DeathCompanion));
            }
        }

        /// <summary>
        ///     Selects a random disease from the enumeration of the same name, applies it to this person. Only one infection can
        ///     be present at any given time.
        /// </summary>
        public void Infect()
        {
            // Skip if this person is already infected with a disease.
            if (Infection != Disease.None)
                return;

            // Create an array of all the disease enumeration values.
            var values = Enum.GetValues(typeof (Disease));

            // Select a random disease from the array, make sure to skip zero index since that is no disease.
            var randomDisease = (Disease) values.GetValue(GameSimulationApp.Instance.Random.Next(1, values.Length));
            if (randomDisease == Disease.None)
                throw new InvalidOperationException(
                    "Attempted to grab a random disease, skipping zero index but was returned zero index for none! That means no disease!");

            // Infect this person.
            Infection = randomDisease;
        }
    }
}