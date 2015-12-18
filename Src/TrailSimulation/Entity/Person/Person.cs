using System;
using System.Diagnostics;
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
        ///     Defines the current health of the person. It will be tracked and kept within bounds of HealthMin and HealthMax
        ///     constants.
        /// </summary>
        private int _health;

        /// <summary>
        ///     Determines if this person has reached the point of no return and has died. There is no coming back from this and
        ///     this flag will be used to prevent any further operations or resources being performed by this person.
        /// </summary>
        private bool _isDead;

        /// <summary>
        ///     Determines if the persons health was at any time at the very poor level, which means they were close to death. We
        ///     can keep track of this and if they recover to full health we will make note about this for the player to see.
        /// </summary>
        private bool _nearDeathExperience;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailEntities.Entities.Person" /> class.
        /// </summary>
        public Person(Profession profession, string name, bool isLeader)
        {
            // Person needs a name, profession, and need to know if they are the leader.
            Profession = profession;
            Name = name;
            IsLeader = isLeader;

            // Person starts with clean bill of health.
            Infected = false;
            Injured = false;
            Health = (int) HealthLevel.Good;
        }

        /// <summary>
        ///     Flag for indicating if the player is afflicted with a disease, virus, fungus, parasite, etc. The type is not
        ///     defined here only the fact they are infected by something biological.
        /// </summary>
        private bool Infected { get; set; }

        /// <summary>
        ///     Current health of this person which is enum that also represents the total points they are currently worth.
        /// </summary>
        public HealthLevel HealthValue
        {
            get
            {
                // Skip if this person is dead, cannot heal them.
                if (_isDead)
                {
                    _health = (int) HealthLevel.Dead;
                    return HealthLevel.Dead;
                }

                // Health is greater than fair so it must be good.
                if (Health > (int) HealthLevel.Fair)
                {
                    return HealthLevel.Good;
                }

                // Health is less than good, but greater than poor so it must be fair.
                if (Health < (int) HealthLevel.Good && Health > (int) HealthLevel.Poor)
                {
                    return HealthLevel.Fair;
                }

                // Health is less than fair, but greater than very poor so it is just poor.
                if (Health < (int) HealthLevel.Fair && Health > (int) HealthLevel.VeryPoor)
                {
                    return HealthLevel.Poor;
                }

                // Health is less than poor, but not quite dead yet so it must be very poor.
                if (Health < (int) HealthLevel.Poor && Health > (int) HealthLevel.Dead)
                {
                    return HealthLevel.VeryPoor;
                }

                // Default response is to indicate this person is dead.
                return HealthLevel.Dead;
            }
        }

        /// <summary>
        ///     Defines the current health of the person. It will be tracked and kept within bounds of HealthMin and HealthMax
        ///     constants.
        /// </summary>
        private int Health
        {
            get { return _health; }
            set
            {
                // Skip if this person is dead, cannot heal them.
                if (_isDead)
                {
                    _health = (int) HealthLevel.Dead;
                    return;
                }

                // Check that value is not above max.
                if (value >= (int) HealthLevel.Good)
                {
                    _health = (int) HealthLevel.Good;
                    return;
                }

                // Check that value is not below min.
                if (value <= (int) HealthLevel.Dead)
                {
                    _isDead = true;
                    _health = (int) HealthLevel.Dead;
                    return;
                }

                // Set health to ceiling corrected value.
                _health = value;
            }
        }

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
        ///     Sets flag on person that marks them as being physically injured and now is handicapped and will take some time to
        ///     heal that is much longer typically than an infection.
        /// </summary>
        private bool Injured { get; set; }

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

        public int Compare(IEntity x, IEntity y)
        {
            Debug.Assert(x != null, "x != null");
            Debug.Assert(y != null, "y != null");

            var result = string.Compare(x.Name, y.Name, StringComparison.Ordinal);
            if (result != 0) return result;

            return result;
        }

        public int CompareTo(IEntity other)
        {
            Debug.Assert(other != null, "other != null");

            var result = string.Compare(other.Name, Name, StringComparison.Ordinal);
            if (result != 0) return result;

            return result;
        }

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

        public bool Equals(IEntity x, IEntity y)
        {
            return x.Equals(y);
        }

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

            // Skip if this person is dead, cannot heal them.
            if (HealthValue == HealthLevel.Dead || _isDead)
                return;

            // Grab instance of the game simulation to increase readability.
            var game = GameSimulationApp.Instance;

            // Eating poorly raises risk of illness.
            if (game.Vehicle.Ration == RationLevel.BareBones)
            {
                CheckIllness();
            }
            else if (game.Vehicle.Ration == RationLevel.Meager &&
                     game.Random.NextBool())
            {
                CheckIllness();
            }

            // More change for illness if you have no clothes.
            var cost_clothes = game.Vehicle.Inventory[Entities.Clothes].TotalValue;
            if (cost_clothes > 22 + 4*game.Random.Next())
            {
                CheckIllness();
            }
            else
            {
                // Random chance for illness in general, even with nice clothes but much lower.
                if (game.Random.NextDouble() > .25 || game.Random.NextDouble() < .5)
                {
                    CheckIllness();
                }
            }

            // Everyday of the simulation we will attempt to eat food.
            ConsumeFood();
        }

        /// <summary>
        ///     Determines how much food party members in the vehicle will eat today.
        /// </summary>
        private void ConsumeFood()
        {
            // Skip if this person is dead, cannot heal them.
            if (HealthValue == HealthLevel.Dead || _isDead)
                return;

            // Grab instance of the game simulation to increase readability.
            var game = GameSimulationApp.Instance;

            var cost_food = game.Vehicle.Inventory[Entities.Food].TotalValue;
            if (cost_food >= 13)
            {
                // Consume some food based on ration level, then update the cost to check against.
                game.Vehicle.Inventory[Entities.Food].ReduceQuantity((int) (cost_food - 8 - 5*(int) game.Vehicle.Ration));

                // Change to get better when eating well.
                Heal();
            }
            else
            {
                // Reduce the players health until they are dead.
                Damage(10, 50);
            }
        }

        /// <summary>
        ///     Increases person's health until it reaches maximum value. When it does will fire off event indicating to player
        ///     this person is now well again and fully healed.
        /// </summary>
        private void Heal()
        {
            // Skip if this person is dead, cannot heal them.
            if (HealthValue == HealthLevel.Dead || _isDead)
                return;

            // Skip if already at max health.
            if (HealthValue == HealthLevel.Good)
                return;

            // Grab instance of the game simulation to increase readability.
            var game = GameSimulationApp.Instance;

            // Person will not get healed every single time it is possible to do so.
            if (game.Random.NextBool())
                return;

            // Check if the player has made a recovery from near death.
            if (_nearDeathExperience && (Infected || Injured))
            {
                // We only want to show the well again event if the player made a massive recovery.
                _nearDeathExperience = false;

                // Roll the dice, person can get better or way worse here.
                game.EventDirector.TriggerEvent(this, game.Random.NextBool()
                    ? typeof (WellAgain)
                    : typeof (TurnForWorse));
            }
            else
            {
                // Increase health by a random amount.
                Health += game.Random.Next(1, 10);
            }
        }

        /// <summary>
        ///     Sets the persons health back to maximum amount and removes all infections and injuries.
        /// </summary>
        public void HealEntirely()
        {
            Health = (int) HealthLevel.Good;
            Infected = false;
            Injured = false;
        }

        /// <summary>
        ///     Check if party leader or a member of it has been killed by an illness.
        /// </summary>
        private void CheckIllness()
        {
            // Grab instance of the game simulation to increase readability.
            var game = GameSimulationApp.Instance;

            // Cannot calculate illness for the dead.
            if (HealthValue == HealthLevel.Dead || _isDead)
                return;

            if (game.Random.Next(100) <= 10 +
                35*((int) game.Vehicle.Ration - 1))
            {
                // Mild illness.
                game.Vehicle.ReduceMileage(5);
                Damage(10, 50);
            }
            else if (game.Random.Next(100) <= 5 -
                     (40/game.Vehicle.Passengers.Count*
                      ((int) game.Vehicle.Ration - 1)))
            {
                // Bad illness.
                game.Vehicle.ReduceMileage(10);
                Damage(10, 50);
            }
            else
            {
                // Severe illness.
                game.Vehicle.ReduceMileage(15);
                Damage(10, 50);
            }

            // If vehicle is not moving we will assume we are resting.
            if (game.Vehicle.Status != VehicleStatus.Moving)
            {
                Heal();
                return;
            }

            // Determines if we should roll for infections based on previous complications.
            switch (HealthValue)
            {
                case HealthLevel.Good:
                    // Congrats on living a healthy lifestyle...
                    Heal();
                    break;
                case HealthLevel.Fair:
                    // Not eating for a couple days is going to hit you hard.
                    if ((Infected || Injured) && game.Vehicle.Status != VehicleStatus.Stopped)
                    {
                        game.Vehicle.ReduceMileage(5);
                        Damage(10, 50);
                    }
                    break;
                case HealthLevel.Poor:
                    // Player is working themselves to death.
                    if ((Infected || Injured) && game.Vehicle.Status != VehicleStatus.Stopped)
                    {
                        game.Vehicle.ReduceMileage(10);
                        Damage(5, 10);
                    }
                    break;
                case HealthLevel.VeryPoor:
                    _nearDeathExperience = true;
                    game.Vehicle.ReduceMileage(15);
                    Damage(1, 5);
                    break;
                case HealthLevel.Dead:
                    _isDead = true;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        ///     Reduces the persons health by a random amount from minimum health value to highest. If this reduces the players
        ///     health below zero the person will be considered dead.
        /// </summary>
        /// <param name="minAmount">Minimum amount of damage that should be randomly generated.</param>
        /// <param name="maxAmount">Maximum amount of damage that should be randomly generated.</param>
        private void Damage(int minAmount, int maxAmount)
        {
            // Skip what is already dead, no damage to be applied.
            if (HealthValue == HealthLevel.Dead)
                return;

            // Grab instance of the game simulation to increase readability.
            var game = GameSimulationApp.Instance;

            // Reduce the persons health by random amount from death amount to desired damage level.
            Health -= game.Random.Next(minAmount, maxAmount);

            // Chance for broken bones and other ailments related to damage (but not death).
            if (!Infected || !Injured)
            {
                // Only plus is that if you are already infected or injured you will not have more events dumped for this person randomly.
                game.EventDirector.TriggerEventByType(this, EventCategory.Person);
            }

            // Check if health dropped to dead levels.
            if (HealthValue != HealthLevel.Dead)
                return;

            // Reduce person's health to dead level.
            Health = (int) HealthLevel.Dead;

            // Check if leader died or party member and execute corresponding event.
            game.EventDirector.TriggerEvent(this, IsLeader ? typeof (DeathPlayer) : typeof (DeathCompanion));
        }

        /// <summary>
        ///     Reduces the persons health by set amount, will check to make sure the amount is not higher than ceiling or lower
        ///     than floor.
        /// </summary>
        /// <param name="amount">Total amount of damage that should be removed from the person.</param>
        public void Damage(int amount)
        {
            // Skip if the amount is less than or equal to zero.
            if (amount <= 0)
                return;

            // Remove the health from the person.
            Health -= amount;
        }

        /// <summary>
        ///     Kills the person without any regard for any other statistics of dice rolls. Only requirement is that the person is
        ///     alive. Will not trigger event for death of player or companion, that is left up to implementation for this method
        ///     and why it exists at all.
        /// </summary>
        public void Kill()
        {
            // Skip if the person is already dead.
            if (HealthValue == HealthLevel.Dead)
                return;

            // Ashes to ashes, dust to dust...
            Health = 0;
        }

        /// <summary>
        ///     Flags the person as being infected with a disease, virus, fungus, etc. We don't specify what it is here, just that
        ///     they are afflicted with it and it will play into future rolls on their health.
        /// </summary>
        public void Infect()
        {
            Infected = true;
        }

        /// <summary>
        ///     Flags the person as being injured physically, this separates it from the infection flag since it means the player
        ///     will be operating at greatly diminished capacity and will take much longer to heal. An example of this type of
        ///     injury would be the person breaking their arm.
        /// </summary>
        public void Injure()
        {
            Injured = true;
        }
    }
}