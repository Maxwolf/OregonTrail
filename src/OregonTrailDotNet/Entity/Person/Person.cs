// Created by Maxwolf (bigmaxwolf.com) 
// Timestamp 01/03/2016@1:50 AM

using System;
using System.Linq;
using OregonTrailDotNet.Entity.Location;
using OregonTrailDotNet.Entity.Location.Weather;
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
        ///     How badly worn down this person is, from nothing at all up to the point of death. Everything the trail does to
        ///     a person adds to this, and it eases off a little every day, so a party that is merely having a hard week
        ///     recovers while one that is cold and hungry and pushing too hard for too long does not.
        /// </summary>
        private double _ailment;

        /// <summary>
        ///     Wear that has built up from being cold or hungry for days on end. It halves away once conditions improve but
        ///     climbs while they do not, which is what makes a long spell of hardship so much worse than a bad day.
        /// </summary>
        private double _fatigue;

        /// <summary>
        ///     Days left of the illness this person is carrying, if any.
        /// </summary>
        private int _illnessDays;

        /// <summary>
        ///     Determines if this person has reached the point of no return and has died. There is no coming back from this and
        ///     this flag will be used to prevent any further operations or resources being performed by this person.
        /// </summary>
        private bool _dead;

        /// <summary>Initializes a new instance of the <see cref="T:TrailEntities.Entities.Person" /> class.</summary>
        /// <param name="profession">The profession.</param>
        /// <param name="name">The name.</param>
        /// <param name="leader">The is Leader.</param>
        public Person(ProfessionEnum profession, string name, bool leader)
        {
            // Person needs a name, profession, and need to know if they are the leader.
            Profession = profession;
            Name = name;
            Leader = leader;

            // Person starts with clean bill of health.
            Infected = false;
            Injured = false;
            _ailment = 0;
        }

        /// <summary>
        ///     Worst this person can be worn down before the trail takes them.
        /// </summary>
        private const double AilmentMax = 139;

        /// <summary>
        ///     How much wear separates one health band from the next: good, fair, poor, very poor.
        /// </summary>
        private const double AilmentPerBand = 35;

        /// <summary>
        ///     Days an illness runs before the person shakes it off on their own.
        /// </summary>
        private const int IllnessLength = 10;

        /// <summary>
        ///     Days a broken bone keeps a person laid up. Far longer than an illness, which is what makes an injury the
        ///     worse thing to be carrying when the trail comes looking for somebody.
        /// </summary>
        private const int InjuryLength = 30;

        /// <summary>
        ///     Flag for indicating if the player is afflicted with a disease, virus, fungus, parasite, etc. The type is not
        ///     defined here only the fact they are infected by something biological.
        /// </summary>
        private bool Infected { get; set; }

        /// <summary>
        ///     Current health of this person which is enum that also represents the total points they are currently worth.
        /// </summary>
        public HealthStatusEnum HealthStatus
        {
            get
            {
                if (_dead)
                    return HealthStatusEnum.Dead;

                // Wear is read in bands rather than as a number: a person is good until they are worn a third of the way
                // down, then fair, then poor, then very poor right up until the trail finishes them.
                var band = (int) (_ailment/AilmentPerBand);
                switch (band)
                {
                    case 0:
                        return HealthStatusEnum.Good;
                    case 1:
                        return HealthStatusEnum.Fair;
                    case 2:
                        return HealthStatusEnum.Poor;
                    default:
                        return HealthStatusEnum.VeryPoor;
                }
            }
        }

        /// <summary>
        ///     Profession of this person, typically if the leader is a banker then the entire family is all bankers for sanity
        ///     sake.
        /// </summary>
        public ProfessionEnum Profession { get; }

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
        ///     <see cref="CauseOfDeathEnum.Unknown" /> until the person is actually killed by the dice-rolling damage path.
        /// </summary>
        public CauseOfDeathEnum Cause { get; private set; } = CauseOfDeathEnum.Unknown;

        /// <summary>
        ///     Exposes the private infection flag to the test project (via InternalsVisibleTo) so recovery behavior can be
        ///     observed without reaching into private state.
        /// </summary>
        internal bool IsInfected => Infected;

        /// <summary>
        ///     Days this person still has to run of whatever laid them up. Exposed to the test project (via InternalsVisibleTo)
        ///     so that illness being temporary can be asserted without reaching into private state.
        /// </summary>
        internal int IllnessDaysRemaining => _illnessDays;

        /// <summary>
        ///     Whether this person is currently laid up with something. Nursing the sick wears on the whole party, so this is
        ///     counted across everyone each day, and falling ill while already ill is what kills.
        /// </summary>
        public bool IsSick => !_dead && (Infected || Injured);

        /// <summary>
        ///     The party leader is spared every misfortune for as long as anyone else is still alive to suffer it instead.
        ///     The original picked a victim at random for each illness, injury and drowning and then explicitly stepped that
        ///     choice past index zero whenever the party held more than one living member, so the leader could only ever be
        ///     hurt once they were the last one left. Everything that harms a person routes through Damage, so the rule is
        ///     enforced there rather than at each call site.
        /// </summary>
        private bool ShieldedAsLeader
        {
            get
            {
                if (!Leader)
                    return false;

                var vehicle = GameSimulationApp.Instance?.Vehicle;
                return vehicle != null && vehicle.PassengerLivingCount > 1;
            }
        }

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
            if ((HealthStatus == HealthStatusEnum.Dead) || _dead)
                return;

            // Nothing below happens on a force tick; this is the reckoning of a whole day lived.
            if (skipDay)
                return;

            // Drinking from a bad-water source at the current location can bring on cholera or dysentery.
            CheckWaterDisease();

            // Consume food based on ration level.
            ConsumeFood();

            // Medical supplies work on the move, not only when the party stops to rest.
            TreatWithMedicine();

            // Count down whatever illness this person is carrying; ten days and they shake it off unaided.
            if (_illnessDays > 0)
            {
                _illnessDays--;
                if (_illnessDays <= 0)
                {
                    Infected = false;
                    Injured = false;
                }
            }

            // Everything the day did to this person, reckoned at once.
            ApplyDailyWear();
        }

        /// <summary>
        ///     Works out how much the day wore this person down and folds it into their health. Nothing here is a single
        ///     cause of death; each is a pressure, and it is living under several of them at once and for long enough that
        ///     kills. The reckoning always eases off by a tenth first, so a party whose luck turns really does recover -
        ///     which is why the way to save a sick party is to stop, eat properly and wait, and why a party that is merely
        ///     cold will sit at "poor" indefinitely rather than dying of it.
        /// </summary>
        private void ApplyDailyWear()
        {
            var game = GameSimulationApp.Instance;
            var vehicle = game.Vehicle;
            var climate = game.Climate;

            // How hot or cold it is, in the six steps a person actually feels, and what the sky is doing.
            var warmth = climate?.TemperatureBand ?? 3;
            var sky = (int) (climate?.Condition ?? WeatherConditionsEnum.Warm);

            // Weather that is comfortable costs nothing; it is the extremes at either end that wear people down, and the
            // cold end is worse than the hot.
            var exposure = warmth - 3;
            if (exposure < 0)
                exposure = 2 - warmth;

            // Cold bites only as far as the party is short of warm clothes: a set each and they shrug it off, which is why
            // clothing is worth buying rather than merely worth points.
            var living = vehicle.PassengerLivingCount;
            var clothing = living > 0 ? vehicle.Inventory[EntitiesEnum.Clothes].Quantity/(double) living : 0;
            var chill = 5 - 2*warmth - clothing;
            if (chill < 0)
                chill = 0;

            // Short rations wear people down; no food at all wears them down far faster. The ration level counts pounds a
            // head, so eating well is the high value and costs nothing, and it is the lean settings that hurt.
            var foodless = vehicle.Inventory[EntitiesEnum.Food].Quantity <= 0;
            var hunger = foodless ? 8 : 2*(RationLevelEnum.Filling - vehicle.Ration);

            // Rain and snow are hard going, and so is pushing the pace. A party that has stopped is not pushing at all.
            var toil = (sky > 5 ? 1 : 0) + (sky > 7 ? 1 : 0);
            if (vehicle.Status == VehicleStatusEnum.Moving)
                toil += 2*(int) vehicle.Pace;

            // Being cold or hungry does not just cost today; it builds up, and only eases once the party is neither.
            _fatigue = (chill > 0.5) || foodless ? _fatigue + 0.8 : _fatigue*0.5;

            // Nursing the sick wears on everybody.
            var sickCount = vehicle.Passengers.Count(passenger => passenger.IsSick);

            _ailment = 0.9*_ailment + exposure + chill + hunger + toil + _fatigue + sickCount;
            if (_ailment < 0)
                _ailment = 0;
            if (_ailment > AilmentMax)
                _ailment = AilmentMax;
        }

        /// <summary>
        ///     How worn down this person is, for the party to weigh when working out whether the trail claims anybody today.
        /// </summary>
        internal double Ailment => _ailment;

        /// <summary>
        ///     Whether this person is as worn down as a person can get, at which point something has to give.
        /// </summary>
        internal bool AtBreakingPoint => !_dead && (_ailment >= AilmentMax);

        /// <summary>
        ///     Lets the party visit the day's misfortune on this person. Sickness is dealt out once a day for the party as a
        ///     whole rather than rolled for separately by everybody, so the decision of whether it happens at all belongs to
        ///     the vehicle and only the consequence belongs here.
        /// </summary>
        internal void StrikeIllness()
        {
            StrikeDownWithIllness();
        }

        /// <summary>
        ///     Visits an illness on this person, or kills them if they were already carrying one. That is the whole of how
        ///     the trail kills: nobody dies outright of hunger or cold, they sicken first, and it is falling ill while
        ///     already ill that finishes them. The leader is spared while anybody else is still standing.
        /// </summary>
        private void StrikeDownWithIllness()
        {
            if (_dead || ShieldedAsLeader)
                return;

            // Announcing what happened is for the benefit of a running game; the harm itself does not depend on there
            // being one, so a person can be worn down in isolation without a simulation attached.
            var director = GameSimulationApp.Instance?.EventDirector;

            // Already ailing, and now this.
            if (Infected || Injured)
            {
                Kill(CauseOfDeathEnum.Illness);
                director?.TriggerEvent(this, Leader ? typeof(DeathPlayer) : typeof(DeathCompanion));
                return;
            }

            Infect();
            director?.TriggerEventByType(this, EventCategoryEnum.Person);
        }

        /// <summary>
        ///     Determines how much food party members in the vehicle will eat today.
        /// </summary>
        private void ConsumeFood()
        {
            // Skip if this person is dead, cannot heal them.
            if ((HealthStatus == HealthStatusEnum.Dead) || _dead)
                return;

            // Grab instance of the game simulation to increase readability.
            var game = GameSimulationApp.Instance;

            // Check if player has any food to eat.
            if (game.Vehicle.Inventory[EntitiesEnum.Food].Quantity > 0)
            {
                // Consume this person's individual share of food for the day based on the ration level. This method runs
                // once per living passenger each traveling day, so the party-wide daily burn already scales with the
                // living count (ration * N). Multiplying by the living count here as well made a party of N eat ration * N
                // squared pounds per day, starving larger parties far faster than the original game intended.
                game.Vehicle.Inventory[EntitiesEnum.Food].ReduceQuantity((int) game.Vehicle.Ration);
            }
        }

        /// <summary>
        ///     Sets the persons health back to maximum amount and removes all infections and injuries.
        /// </summary>
        public void HealEntirely()
        {
            _ailment = 0;
            _fatigue = 0;
            _illnessDays = 0;
            Infected = false;
            Injured = false;
        }

        /// <summary>
        ///     Spends a medical kit on a sick or injured person to clear the ailment outright, sparing them the ten days it
        ///     would otherwise run - and, more to the point, sparing them being struck down a second time while still ill,
        ///     which is what actually kills. Does nothing if they are well or there is no medicine left.
        /// </summary>
        private void TreatWithMedicine()
        {
            var game = GameSimulationApp.Instance;
            var inventory = game.Vehicle.Inventory;

            if (!Infected && !Injured)
                return;

            if (!inventory.ContainsKey(EntitiesEnum.Medicine) || (inventory[EntitiesEnum.Medicine].Quantity <= 0))
                return;

            inventory[EntitiesEnum.Medicine].ReduceQuantity(1);
            Infected = false;
            Injured = false;
            _illnessDays = 0;

            // Being nursed back to health takes some of the wear off with it.
            _ailment -= game.Random.Next(15, 40);
            if (_ailment < 0)
                _ailment = 0;
        }

        /// <summary>
        ///     Rolls for a waterborne disease (cholera or dysentery) based on the quality of the water at the party's current
        ///     location. Locations flagged with bad water double the daily chance of contracting one of these diseases.
        /// </summary>
        private void CheckWaterDisease()
        {
            var game = GameSimulationApp.Instance;

            // Cannot get sick if already dead.
            if ((HealthStatus == HealthStatusEnum.Dead) || _dead)
                return;

            var location = game.Trail.CurrentLocation;
            if (location == null)
                return;

            // Base daily chance of a waterborne illness; a bad-water source doubles it.
            var threshold = location.Warning == LocationWarningEnum.BadWater ? 2 : 1;
            if (game.Random.Next(100) < threshold)
                game.EventDirector.TriggerEvent(this,
                    game.Random.NextBool() ? typeof(Cholera) : typeof(Dysentery));
        }

        /// <summary>
        ///     Reduces the persons health by a random amount from minimum health value to highest. If this reduces the players
        ///     health below zero the person will be considered dead.
        /// </summary>
        /// <param name="minAmount">Minimum amount of damage that should be randomly generated.</param>
        /// <param name="maxAmount">Maximum amount of damage that should be randomly generated.</param>
        /// <param name="cause">What is inflicting this damage, recorded as the cause of death if it proves fatal.</param>
        private void Damage(int minAmount, int maxAmount, CauseOfDeathEnum cause)
        {
            // Skip what is already dead, no damage to be applied.
            if (HealthStatus == HealthStatusEnum.Dead)
                return;

            // Misfortune lands on somebody else while the leader still has company.
            if (ShieldedAsLeader)
                return;

            // Grab instance of the game simulation to increase readability.
            var game = GameSimulationApp.Instance;

            // Whatever happened wears on them now; the day's own reckoning will ease it off again afterwards.
            Cause = cause;
            AddWear(game.Random.Next(minAmount, maxAmount));
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

            // Misfortune lands on somebody else while the leader still has company.
            if (ShieldedAsLeader)
                return;

            AddWear(amount);
        }

        /// <summary>
        ///     Wears a person down by some amount and sees whether it was more than they could take. Everything that harms a
        ///     person comes through here, so this is the single place the trail is allowed to make somebody ill or kill them.
        /// </summary>
        /// <param name="amount">How much wear to add, on the same scale as the health bands.</param>
        private void AddWear(double amount)
        {
            if (_dead || (amount <= 0))
                return;

            _ailment += amount;

            if (_ailment <= AilmentMax)
                return;

            _ailment = AilmentMax;
            StrikeDownWithIllness();
        }

        /// <summary>
        ///     Kills the person without any regard for any other statistics of dice rolls. Only requirement is that the person is
        ///     alive. Will not trigger event for death of player or companion, that is left up to implementation for this method
        ///     and why it exists at all.
        /// </summary>
        /// <param name="cause">What killed them, recorded so the death screen can say how they died instead of falling back to a
        ///     blank "unknown" (catastrophic events used to kill through this path without recording any cause).</param>
        public void Kill(CauseOfDeathEnum cause = CauseOfDeathEnum.Unknown)
        {
            // Skip if the person is already dead.
            if (HealthStatus == HealthStatusEnum.Dead)
                return;

            // Record how they died before flipping the health to dead.
            Cause = cause;

            // Ashes to ashes, dust to dust...
            _dead = true;
            _ailment = AilmentMax;
        }

        /// <summary>
        ///     Flags the person as being infected with a disease, virus, fungus, etc. We don't specify what it is here, just that
        ///     they are afflicted with it and it will play into future rolls on their health.
        /// </summary>
        public void Infect()
        {
            Infected = true;

            // Start them on the mend, or an illness caught this way never ends: only a person with days left to run ever
            // recovers, and one who is permanently sick both wears on the whole party and dies to the next misfortune.
            if (_illnessDays < IllnessLength)
                _illnessDays = IllnessLength;
        }

        /// <summary>
        ///     Flags the person as being injured physically, this separates it from the infection flag since it means the player
        ///     will be operating at greatly diminished capacity and will take much longer to heal. An example of this type of
        ///     injury would be the person breaking their arm.
        /// </summary>
        public void Injure()
        {
            Injured = true;

            // A broken bone keeps somebody down far longer than a fever does.
            if (_illnessDays < InjuryLength)
                _illnessDays = InjuryLength;
        }
    }
}