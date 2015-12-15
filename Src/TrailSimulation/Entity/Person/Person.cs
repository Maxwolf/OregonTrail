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
            Health = Health.Good;
        }

        /// <summary>
        ///     Current health of this person which is enum that also represents the total points they are currently worth.
        /// </summary>
        public Health Health { get; set; }

        /// <summary>
        ///     Determines how many total consecutive days this player has not eaten any food. If this continues for more than five
        ///     (5) days then the probability they will die increases exponentially.
        /// </summary>
        private int DaysStarving { get; set; }

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
        ///     Determines if the person is dead and no longer consuming resources. Dead party members in the same vehicle will
        ///     lower total possible mileage for several turns as remaining people mourn the loss of the other.
        /// </summary>
        public bool IsDead { get; private set; }

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
            // Grab instance of the game simulation to increase readability.
            var game = GameSimulationApp.Instance;

            var cost_food = game.Vehicle.Inventory[Entities.Food].TotalValue;
            cost_food = cost_food - 8 - 5*(int) game.Vehicle.Ration;
            if (cost_food >= 13)
            {
                // Consume the food since we still have some.
                game.Vehicle.Inventory[Entities.Food] = new SimItem(
                    game.Vehicle.Inventory[Entities.Food],
                    (int) cost_food);

                // Change to get better when eating well.
                TryHeal();
            }
            else
            {
                if (DaysStarving > 5)
                {
                    Kill();
                }
                else
                {
                    // Otherwise we begin to starve.
                    DaysStarving++;

                    // Not eating well is gonna hurt you bad.
                    if (game.Vehicle.Status != VehicleStatus.Stopped)
                        TryInfect();
                }
            }
        }

        /// <summary>
        ///     Check if party leader or a member of it has been killed by an illness.
        /// </summary>
        private void CheckIllness()
        {
            // Grab instance of the game simulation to increase readability.
            var game = GameSimulationApp.Instance;

            if (game.Random.Next(100) <= 10 +
                35*((int) game.Vehicle.Ration - 1))
            {
                // Mild illness.
                game.Vehicle.ReduceMileage(5);
                Health = Health.Fair;
            }
            else if (game.Random.Next(100) <= 5 -
                     (40/game.Vehicle.Passengers.Count()*
                      ((int) game.Vehicle.Ration - 1)))
            {
                // Bad illness.
                game.Vehicle.ReduceMileage(10);
                Health = Health.Poor;
            }
            else
            {
                // Severe illness.
                Health = Health.VeryPoor;
                game.Vehicle.ReduceMileage(15);

                TryInfect();
            }

            // If vehicle is not moving we will assume we are resting.
            if (game.Vehicle.Status != VehicleStatus.Moving)
                TryHeal();

            // Determines if we should roll for infections based on previous complications.
            switch (Health)
            {
                case Health.Good:
                    // Congrats on living a healthy lifestyle...
                    TryHeal();
                    break;
                case Health.Fair:
                    // Not eating for a couple days is going to hit you hard.
                    if (DaysStarving > 2 &&
                        game.Vehicle.Status == VehicleStatus.Moving)
                    {
                        game.Vehicle.ReduceMileage(5);
                        Health = Health.Poor;
                    }
                    break;
                case Health.Poor:
                    // Player is working themselves to death.
                    if (DaysStarving > 5 &&
                        game.Vehicle.Status == VehicleStatus.Moving)
                    {
                        game.Vehicle.ReduceMileage(10);
                        Health = Health.VeryPoor;
                    }
                    break;
                case Health.VeryPoor:
                    TryInfect();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        ///     Attempts to infect the person with some ailment, rolls the dice to determine if this should be done. Will not
        ///     infect people that already have an infection.
        /// </summary>
        private void TryInfect()
        {
            // Grab instance of the game simulation to increase readability.
            var game = GameSimulationApp.Instance;

            // Infects the uninfected, progresses infections of existing people.
            if (game.Random.Next(100) <= 5)
            {
                // Pick an actual severe illness from list, roll the dice for it on very low health.
                game.EventDirector.TriggerEventByType(this, EventCategory.Person);
            }
            else if (game.Random.Next(100) >= 50)
            {
                StepHealthDown();
            }
        }

        /// <summary>
        ///     Attempts to heal the player and increase their health and remove infections. Requires them to not be moving and
        ///     resting, eating well, and various other factors such as current climate and condition.
        /// </summary>
        private void TryHeal()
        {
            // Grab instance of the game simulation to increase readability.
            var game = GameSimulationApp.Instance;

            // Completely heal the player.
            if (game.Random.Next(100) >= 99)
            {
                game.EventDirector.TriggerEvent(this, typeof (WellAgain));
            }
            else if (game.Random.Next(100) >= 50)
            {
                StepHealthUp();
            }
        }

        /// <summary>
        ///     Increases the health of the player up to the best possible status, effectively healing them with rest and good
        ///     eating.
        /// </summary>
        private void StepHealthUp()
        {
            switch (Health)
            {
                case Health.Good:
                    // Chance to be rid of infections when at good health.
                    if (GameSimulationApp.Instance.Random.Next(100) >= 99)
                    {
                        GameSimulationApp.Instance.EventDirector.TriggerEvent(this, typeof (WellAgain));
                    }
                    break;
                case Health.Fair:
                    Health = Health.Good;
                    break;
                case Health.Poor:
                    Health = Health.Fair;
                    break;
                case Health.VeryPoor:
                    Health = Health.Poor;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        ///     Progress the severity of a infection the person already has.
        /// </summary>
        private void StepHealthDown()
        {
            switch (Health)
            {
                case Health.Good:
                    Health = Health.Fair;
                    break;
                case Health.Fair:
                    Health = Health.Poor;
                    break;
                case Health.Poor:
                    Health = Health.VeryPoor;
                    break;
                case Health.VeryPoor:
                    // Player succumbs to their poor health.
                    Kill();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        ///     Kills the person, meaning they will no longer consume resources or count towards final point total if the leader
        ///     still wins the game.
        /// </summary>
        private void Kill()
        {
            // Grab instance of the game simulation to increase readability.
            var game = GameSimulationApp.Instance;

            // Death makes everybody take a huge morale hit.
            game.Vehicle.ReduceMileage(50);

            // Mark the player as being dead now.
            IsDead = true;

            // Check if leader died or party member.
            game.EventDirector.TriggerEvent(this, IsLeader ? typeof (DeathPlayer) : typeof (DeathCompanion));
        }
    }
}