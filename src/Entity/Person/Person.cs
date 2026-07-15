// Created by Ron 'Maxwolf' McDowell (ron.mcdowell@gmail.com) 
// Timestamp 01/03/2016@1:50 AM

using System;
using OregonTrailDotNet.Entity.Location;
using OregonTrailDotNet.Entity.Vehicle;
using OregonTrailDotNet.Event;
using OregonTrailDotNet.Event.Person;

namespace OregonTrailDotNet.Entity.Person
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
        private int _status;

        /// <summary>
        ///     Determines if this person has reached the point of no return and has died. There is no coming back from this and
        ///     this flag will be used to prevent any further operations or resources being performed by this person.
        /// </summary>
        private bool _dead;

        /// <summary>
        ///     Determines if the persons health was at any time at the very poor level, which means they were close to death. We
        ///     can keep track of this and if they recover to full health we will make note about this for the player to see.
        /// </summary>
        private bool _nearDeathExperience;

        /// <summary>Initializes a new instance of the <see cref="T:TrailEntities.Entities.Person" /> class.</summary>
        /// <param name="profession">The profession.</param>
        /// <param name="name">The name.</param>
        /// <param name="leader">The is Leader.</param>
        public Person(Profession profession, string name, bool leader)
        {
            // Person needs a name, profession, and need to know if they are the leader.
            Profession = profession;
            Name = name;
            Leader = leader;

            // Person starts with clean bill of health.
            Infected = false;
            Injured = false;
            Status = (int) HealthStatus.Good;
        }

        /// <summary>
        ///     Flag for indicating if the player is afflicted with a disease, virus, fungus, parasite, etc. The type is not
        ///     defined here only the fact they are infected by something biological.
        /// </summary>
        private bool Infected { get; set; }

        /// <summary>
        ///     Current health of this person which is enum that also represents the total points they are currently worth.
        /// </summary>
        public HealthStatus HealthStatus
        {
            get
            {
                // Skip if this person is dead, cannot heal them.
                if (_dead)
                {
                    _status = (int) HealthStatus.Dead;
                    return HealthStatus.Dead;
                }

                // Health is greater than fair so it must be good.
                if (Status > (int) HealthStatus.Fair)
                    return HealthStatus.Good;

                // Health is less than good, but greater than poor so it must be fair.
                if ((Status < (int) HealthStatus.Good) && (Status > (int) HealthStatus.Poor))
                    return HealthStatus.Fair;

                // Health is less than fair, but greater than very poor so it is just poor.
                if ((Status < (int) HealthStatus.Fair) && (Status > (int) HealthStatus.VeryPoor))
                    return HealthStatus.Poor;

                // Health is less than poor, but not quite dead yet so it must be very poor.
                if ((Status < (int) HealthStatus.Poor) && (Status > (int) HealthStatus.Dead))
                    return HealthStatus.VeryPoor;

                // Default response is to indicate this person is dead.
                return HealthStatus.Dead;
            }
        }

        /// <summary>
        ///     Defines the current health of the person. It will be tracked and kept within bounds of HealthMin and HealthMax
        ///     constants.
        /// </summary>
        private int Status
        {
            get => _status;
            set
            {
                // Skip if this person is dead, cannot heal them.
                if (_dead)
                {
                    _status = (int) HealthStatus.Dead;
                    return;
                }

                // Check that value is not above max.
                if (value >= (int) HealthStatus.Good)
                {
                    _status = (int) HealthStatus.Good;
                    return;
                }

                // Check that value is not below min.
                if (value <= (int) HealthStatus.Dead)
                {
                    _dead = true;
                    _status = (int) HealthStatus.Dead;
                    return;
                }

                // Set health to ceiling corrected value.
                _status = value;
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
        public bool Leader { get; }

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
        ///     Records how this person died so the death screen can tell the player what happened. Remains
        ///     <see cref="CauseOfDeath.Unknown" /> until the person is actually killed by the dice-rolling damage path.
        /// </summary>
        public CauseOfDeath Cause { get; private set; } = CauseOfDeath.Unknown;

        /// <summary>
        ///     Exposes the private infection flag to the test project (via InternalsVisibleTo) so recovery behavior can be
        ///     observed without reaching into private state.
        /// </summary>
        internal bool IsInfected => Infected;

        /// <summary>The compare.</summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <returns>The <see cref="int" />.</returns>
        public int Compare(IEntity x, IEntity y)
        {
            var result = string.Compare(x?.Name, y?.Name, StringComparison.Ordinal);
            if (result != 0) return result;

            return result;
        }

        /// <summary>The compare to.</summary>
        /// <param name="other">The other.</param>
        /// <returns>The <see cref="int" />.</returns>
        public int CompareTo(IEntity other)
        {
            var result = string.Compare(other.Name, Name, StringComparison.Ordinal);
            return result;
        }

        /// <summary>The equals.</summary>
        /// <param name="other">The other.</param>
        /// <returns>The <see cref="bool" />.</returns>
        public bool Equals(IEntity other)
        {
            // Reference equality check
            if (this == other)
                return true;

            if (other == null)
                return false;

            if (other.GetType() != GetType())
                return false;

            if (Name.Equals(other.Name))
                return true;

            return false;
        }

        /// <summary>The equals.</summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <returns>The <see cref="bool" />.</returns>
        public bool Equals(IEntity x, IEntity y)
        {
            return x.Equals(y);
        }

        /// <summary>The get hash code.</summary>
        /// <param name="obj">The obj.</param>
        /// <returns>The <see cref="int" />.</returns>
        public int GetHashCode(IEntity obj)
        {
            var hash = 23;
            hash = hash*31 + Name.GetHashCode();
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
        /// <param name="skipDay">
        ///     Determines if the simulation has force ticked without advancing time or down the trail. Used by
        ///     special events that want to simulate passage of time without actually any actual time moving by.
        /// </param>
        public void OnTick(bool systemTick, bool skipDay)
        {
            // Only tick person with simulation.
            if (systemTick)
                return;

            // Skip if this person is dead, cannot heal them.
            if ((HealthStatus == HealthStatus.Dead) || _dead)
                return;

            // Grab instance of the game simulation to increase readability.
            var game = GameSimulationApp.Instance;

            // Eating poorly raises risk of illness.
            if (game.Vehicle.Ration == RationLevel.BareBones)
                CheckIllness();
            else if ((game.Vehicle.Ration == RationLevel.Meager) &&
                     game.Random.NextBool())
                CheckIllness();

            // Insufficient clothing for the party raises the risk of illness. The fewer sets of clothing the party carries
            // relative to how many living members it has, the more likely someone falls ill in the cold.
            var clothingCount = game.Vehicle.Inventory[Entities.Clothes].Quantity;
            var partySize = game.Vehicle.PassengerLivingCount;
            if (clothingCount < partySize + game.Random.Next(0, 4))
            {
                CheckIllness();
            }
            else
            {
                // Random chance for illness in general, even with plenty of clothes but much lower.
                if (game.Random.NextBool() && game.Random.NextBool())
                    CheckIllness();
            }

            // The rest only happens when a whole day actually passes; realtime/force ticks don't apply these penalties.
            if (!skipDay)
            {
                // Drinking from a bad-water source at the current location can bring on cholera or dysentery.
                CheckWaterDisease();

                // Consume food based on ration level.
                ConsumeFood();

                // Freezing weather with too little clothing is directly dangerous, independent of how well the party eats.
                // In the original game warm clothes were essential once you hit the snow and mountain passes; here a party
                // that carries fewer than one set per person takes real cold damage - and can freeze to death - and the
                // colder it gets (and the shorter it is on clothing) the worse the exposure. A properly clothed party is
                // spared entirely, which is what gives the optimizer a clean reason to buy clothing.
                ApplyColdExposure(clothingCount, partySize);

                // Running out of ammunition (no way to hunt) or medical supplies (no way to treat the sick) wears the party down.
                ApplySupplyPenalties(skipDay);
            }
        }

        /// <summary>
        ///     Determines how much food party members in the vehicle will eat today.
        /// </summary>
        private void ConsumeFood()
        {
            // Skip if this person is dead, cannot heal them.
            if ((HealthStatus == HealthStatus.Dead) || _dead)
                return;

            // Grab instance of the game simulation to increase readability.
            var game = GameSimulationApp.Instance;

            // Check if player has any food to eat.
            if (game.Vehicle.Inventory[Entities.Food].Quantity > 0)
            {
                // Consume this person's individual share of food for the day based on the ration level. This method runs
                // once per living passenger each traveling day, so the party-wide daily burn already scales with the
                // living count (ration * N). Multiplying by the living count here as well made a party of N eat ration * N
                // squared pounds per day, starving larger parties far faster than the original game intended.
                game.Vehicle.Inventory[Entities.Food].ReduceQuantity((int) game.Vehicle.Ration);

                // Change to get better when eating well.
                Heal();
            }
            else
            {
                // Reduce the players health until they are dead.
                Damage(10, 50, CauseOfDeath.Starvation);
            }
        }

        /// <summary>
        ///     Increases person's health until it reaches maximum value. When it does will fire off event indicating to player
        ///     this person is now well again and fully healed.
        /// </summary>
        private void Heal()
        {
            // Skip if this person is dead, cannot heal them.
            if ((HealthStatus == HealthStatus.Dead) || _dead)
                return;

            // Skip if already at max health.
            if (HealthStatus == HealthStatus.Good)
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
                    ? typeof(WellAgain)
                    : typeof(TurnForWorse));
            }
            else
            {
                // Increase health by a random amount.
                Status += game.Random.Next(1, 10);
            }
        }

        /// <summary>
        ///     Sets the persons health back to maximum amount and removes all infections and injuries.
        /// </summary>
        public void HealEntirely()
        {
            Status = (int) HealthStatus.Good;
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
            if ((HealthStatus == HealthStatus.Dead) || _dead)
                return;

            // Person will not get hurt every single time it is called.
            if (game.Random.NextBool())
                return;

            // Poor eating makes illness both more likely and more severe. There are three tiers: a mild ailment, a worse
            // "bad" illness, and a very serious illness. Only the very serious tier leaves the person infected and thus in
            // need of medical services to recover; the two milder tiers can be shrugged off on their own.
            if (game.Random.Next(100) <= 10 +
                35*(3 - (int) game.Vehicle.Ration))
            {
                // Mild illness.
                game.Vehicle.ReduceMileage(5);
                Damage(10, 50, CauseOfDeath.Illness);
            }
            else if (game.Random.Next(100) <= 5 +
                     20*(3 - (int) game.Vehicle.Ration))
            {
                // Bad (moderate) illness.
                game.Vehicle.ReduceMileage(10);
                Damage(10, 50, CauseOfDeath.Illness);
            }
            else if (game.Random.Next(100) <= 5 +
                     40/game.Vehicle.Passengers.Count*
                     (3 - (int) game.Vehicle.Ration))
            {
                // Very serious illness that will require medical supplies to recover from.
                game.Vehicle.ReduceMileage(15);
                Infect();
                Damage(10, 50, CauseOfDeath.Illness);
            }

            // While the party is resting the sick can actually recover; while traveling, existing infections and injuries
            // only make things worse. Determine which case we are in once.
            var resting = game.Vehicle.Status == VehicleStatus.Stopped;

            // Determines if we should roll for infections based on previous complications.
            switch (HealthStatus)
            {
                case HealthStatus.Good:
                    if (Infected || Injured)
                    {
                        if (resting)
                            TreatWhileResting();
                        else
                        {
                            game.Vehicle.ReduceMileage(5);
                            Damage(10, 50, CauseOfDeath.Illness);
                        }
                    }

                    break;
                case HealthStatus.Fair:
                    if (Infected || Injured)
                    {
                        if (resting)
                            TreatWhileResting();
                        else if (game.Random.NextBool())
                        {
                            // Hurt the player and reduce total possible mileage this turn.
                            game.Vehicle.ReduceMileage(5);
                            Damage(10, 50, CauseOfDeath.Illness);
                        }
                    }

                    break;
                case HealthStatus.Poor:
                    if (Infected || Injured)
                    {
                        if (resting)
                            TreatWhileResting();
                        else
                        {
                            game.Vehicle.ReduceMileage(10);
                            Damage(5, 10, CauseOfDeath.Illness);
                        }
                    }

                    break;
                case HealthStatus.VeryPoor:
                    _nearDeathExperience = true;
                    if (resting)
                    {
                        TreatWhileResting();
                    }
                    else
                    {
                        game.Vehicle.ReduceMileage(15);
                        Damage(1, 5, CauseOfDeath.Illness);
                    }

                    break;
                case HealthStatus.Dead:
                    _dead = true;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        ///     Attempts to treat a sick or injured party member while the vehicle is stopped to rest. If the party has medical
        ///     supplies on hand a kit is used up to quickly cure the infection or injury; otherwise the person can only rely on
        ///     slower natural recovery.
        /// </summary>
        private void TreatWhileResting()
        {
            var game = GameSimulationApp.Instance;

            if ((Infected || Injured) &&
                game.Vehicle.Inventory.ContainsKey(Entities.Medicine) &&
                (game.Vehicle.Inventory[Entities.Medicine].Quantity > 0))
            {
                // Medical supplies cure the ailment quickly.
                game.Vehicle.Inventory[Entities.Medicine].ReduceQuantity(1);
                Infected = false;
                Injured = false;
                Status += game.Random.Next(15, 40);
            }
            else
            {
                // No medicine on hand, fall back to slow natural recovery.
                Heal();
            }
        }

        /// <summary>
        ///     Rolls for a waterborne disease (cholera or dysentery) based on the quality of the water at the party's current
        ///     location. Locations flagged with bad water double the daily chance of contracting one of these diseases.
        /// </summary>
        private void CheckWaterDisease()
        {
            var game = GameSimulationApp.Instance;

            // Cannot get sick if already dead.
            if ((HealthStatus == HealthStatus.Dead) || _dead)
                return;

            var location = game.Trail.CurrentLocation;
            if (location == null)
                return;

            // Base daily chance of a waterborne illness; a bad-water source doubles it.
            var threshold = location.Warning == LocationWarning.BadWater ? 2 : 1;
            if (game.Random.Next(100) < threshold)
                game.EventDirector.TriggerEvent(this,
                    game.Random.NextBool() ? typeof(Cholera) : typeof(Dysentery));
        }

        /// <summary>
        ///     Applies gradual health penalties when the party has run out of critical supplies: with neither food nor
        ///     ammunition there is no way to hunt and starvation sets in faster, and a sick traveler with no medical supplies
        ///     slowly worsens.
        /// </summary>
        /// <param name="skipDay">Whether the simulation force-ticked without advancing a real day.</param>
        private void ApplySupplyPenalties(bool skipDay)
        {
            // Only apply once per real day and never to the dead.
            if (skipDay || (HealthStatus == HealthStatus.Dead) || _dead)
                return;

            var game = GameSimulationApp.Instance;
            var inventory = game.Vehicle.Inventory;

            // Out of food AND out of ammunition means no way to hunt for more, so starvation accelerates.
            if ((inventory[Entities.Food].Quantity <= 0) && (inventory[Entities.Ammo].Quantity <= 0))
                Damage(5, 15, CauseOfDeath.Starvation);

            // Medical supplies work on the move, not only when the party stops to rest. A sick or injured traveler with
            // medicine on hand spends a kit that day to treat the ailment - a weaker cure than a full rest-stop, but enough
            // to clear the infection and keep them going - while one with no medicine steadily worsens. This is what makes
            // carrying medicine pay off during the journey rather than only during a (costly) rest, so the optimizer has a
            // reason to buy it instead of dropping it to zero.
            if (Infected || Injured)
            {
                if (inventory.ContainsKey(Entities.Medicine) && (inventory[Entities.Medicine].Quantity > 0))
                {
                    inventory[Entities.Medicine].ReduceQuantity(1);
                    Infected = false;
                    Injured = false;
                    Status += game.Random.Next(5, 20);
                }
                else
                {
                    Damage(1, 5, CauseOfDeath.Illness);
                }
            }
        }

        /// <summary>
        ///     Applies cold-exposure damage when the party is travelling through freezing weather without enough clothing. Warm
        ///     clothes are only relevant once the temperature drops below freezing, so a party crossing warm country is never
        ///     penalised no matter how little clothing it carries. Below freezing, a party holding fewer than one set of clothing
        ///     per living member risks direct health loss each day; the risk climbs with how many sets it is short and how far
        ///     below freezing it is, so the snow and high mountain passes are lethal to the ill-prepared while a party with a set
        ///     per person shrugs the cold off entirely.
        /// </summary>
        /// <param name="clothingCount">Sets of clothing the party currently carries.</param>
        /// <param name="partySize">Number of living party members needing to stay warm.</param>
        private void ApplyColdExposure(int clothingCount, int partySize)
        {
            var game = GameSimulationApp.Instance;

            // Warm country (at or above freezing) never threatens a clothing-short party.
            var temperature = game.Trail.CurrentLocation?.Temperature ?? 20;
            if (temperature > 0 || clothingCount >= partySize)
                return;

            // Missing sets of clothing and severity of the cold both raise the exposure risk. chill is 1 at freezing and
            // climbs by one for every 10 degrees below zero; the risk is capped so even a brutal cold snap stays survivable
            // with a little luck, which keeps a gradient for the optimizer to climb rather than wiping every party.
            var shortfall = partySize - clothingCount;
            var chill = 1 + (-temperature) / 10;
            var risk = Math.Min(65, 10 + 6 * shortfall * chill);
            if (game.Random.Next(100) < risk)
                Damage(10, 40, CauseOfDeath.Illness);
        }

        /// <summary>
        ///     Reduces the persons health by a random amount from minimum health value to highest. If this reduces the players
        ///     health below zero the person will be considered dead.
        /// </summary>
        /// <param name="minAmount">Minimum amount of damage that should be randomly generated.</param>
        /// <param name="maxAmount">Maximum amount of damage that should be randomly generated.</param>
        /// <param name="cause">What is inflicting this damage, recorded as the cause of death if it proves fatal.</param>
        private void Damage(int minAmount, int maxAmount, CauseOfDeath cause)
        {
            // Skip what is already dead, no damage to be applied.
            if (HealthStatus == HealthStatus.Dead)
                return;

            // Grab instance of the game simulation to increase readability.
            var game = GameSimulationApp.Instance;

            // Reduce the persons health by random amount from death amount to desired damage level.
            Status -= game.Random.Next(minAmount, maxAmount);

            // Chance for broken bones and other ailments related to damage (but not death).
            if (!Infected || !Injured)
                game.EventDirector.TriggerEventByType(this, EventCategory.Person);

            // Check if health dropped to dead levels.
            if (HealthStatus != HealthStatus.Dead)
                return;

            // Record what killed them so the death screen can explain what happened.
            Cause = cause;

            // Reduce person's health to dead level.
            Status = (int) HealthStatus.Dead;

            // Check if leader died or party member and execute corresponding event.
            game.EventDirector.TriggerEvent(this, Leader ? typeof(DeathPlayer) : typeof(DeathCompanion));
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
            Status -= amount;
        }

        /// <summary>
        ///     Kills the person without any regard for any other statistics of dice rolls. Only requirement is that the person is
        ///     alive. Will not trigger event for death of player or companion, that is left up to implementation for this method
        ///     and why it exists at all.
        /// </summary>
        /// <param name="cause">What killed them, recorded so the death screen can say how they died instead of falling back to a
        ///     blank "unknown" (catastrophic events used to kill through this path without recording any cause).</param>
        public void Kill(CauseOfDeath cause = CauseOfDeath.Unknown)
        {
            // Skip if the person is already dead.
            if (HealthStatus == HealthStatus.Dead)
                return;

            // Record how they died before flipping the health to dead.
            Cause = cause;

            // Ashes to ashes, dust to dust...
            Status = 0;
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