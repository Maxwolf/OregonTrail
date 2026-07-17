// Created by Maxwolf (bigmaxwolf.com) 
// Timestamp 01/03/2016@1:50 AM

using System;
using OregonTrailDotNet.Entity.Location.Weather;

namespace OregonTrailDotNet.Entity.Location
{
    /// <summary>
    ///     Defines a location in the game that is added to a list of points that make up the entire trail which the player and
    ///     his vehicle travel upon.
    /// </summary>
    public abstract class Location : IEntity
    {
        /// <summary>Initializes a new instance of the <see cref="T:OregonTrailDotNet.Entity.Location.Location" /> class.</summary>
        /// <param name="name">Display name of the location as it should be known to the player.</param>
        /// <param name="climateType">Band of country this location sits in, which decides its weather.</param>
        protected Location(string name, ClimateEnum climateType)
        {
            // Default warning message for the location is based on fresh water status.
            Warning = GameSimulationApp.Instance.Random.NextBool() ? LocationWarningEnum.None : LocationWarningEnum.BadWater;

            // Stretch of country this place sits in; the climate module reads it to know what the weather should be doing.
            Climate = climateType;

            // Name of the point as it should be known to the player.
            Name = name;

            // Default location status is not visited by the player or vehicle.
            Status = LocationStatusEnum.Unreached;
        }

        /// <summary>
        ///     Determines the total depth of the location in regards to it's position on the trail. This acts like a weight and
        ///     determines the probability in which the player will visit the location. Lower numbers mean higher chances, higher
        ///     numbers mean less of a chance player will visit because they will have to choose it based on a choice and sometimes
        ///     there are many choices leading up to a given location making it's depth increase.
        /// </summary>
        public int Depth { get; set; }

        /// <summary>
        ///     Determines if this location sits at a high elevation (a mountain pass). High-ground locations have a chance of the
        ///     party becoming stuck, suffer slow-going mileage penalties, risk cave-ins, and see far more blizzards.
        /// </summary>
        public bool HighGround { get; set; }

        /// <summary>
        ///     Percentage chance (0-100) that the party becomes stuck for several days when departing this location. Only used by
        ///     high-ground mountain passes such as South Pass (80%) and the Blue Mountains (70%). Zero for normal locations.
        /// </summary>
        public int StuckChance { get; set; }

        /// <summary>
        ///     Warnings about low food, medical problems, weather, etc.
        /// </summary>
        public LocationWarningEnum Warning { get; }

        /// <summary>
        ///     Band of country this location sits in, which is what decides the weather over it.
        /// </summary>
        public ClimateEnum Climate { get; }

        /// <summary>
        ///     What the weather is doing where the party is standing. There is one sky over the journey rather than a private
        ///     one per location, so this reports whatever the climate module has the weather doing today.
        /// </summary>
        public WeatherConditionsEnum Weather =>
            GameSimulationApp.Instance.Climate?.Condition ?? WeatherConditionsEnum.Warm;

        /// <summary>
        ///     Current outside temperature in Fahrenheit the party is exposed to. Survival mechanics read this so a party that
        ///     skimped on clothing suffers real cold exposure once the trail turns freezing.
        /// </summary>
        public int Temperature => GameSimulationApp.Instance.Climate?.Temperature ?? 60;

        /// <summary>
        ///     Determines if the location allows the player to chat to other NPC's in the area which can offer up advice about the
        ///     trail ahead.
        /// </summary>
        public abstract bool ChattingAllowed { get; }

        /// <summary>
        ///     Determines if this location has a store which the player can buy items from using their monies.
        /// </summary>
        public abstract bool ShoppingAllowed { get; }

        /// <summary>
        ///     Determines if this location has already been visited by the vehicle and party members.
        /// </summary>
        /// <returns>TRUE if location has been passed by, FALSE if location has yet to be reached.</returns>
        public LocationStatusEnum Status { get; set; }

        /// <summary>
        ///     Determines if the look around question has been asked in regards to the player stopping the vehicle to rest or
        ///     change vehicle options. Otherwise they will just continue on the trail, this property prevents the question from
        ///     being asked twice for any one location.
        /// </summary>
        public bool ArrivalFlag { get; set; }

        /// <summary>
        ///     Determines if the given location is the last location on the trail, this is useful to know because we want to do
        ///     something special with the location before we actually arrive to it but we can know the next location is last using
        ///     this.
        /// </summary>
        public bool LastLocation { get; set; }

        /// <summary>
        ///     Total distance to the next location the player must travel before it will be triggered
        /// </summary>
        public int TotalDistance { get; set; }

        /// <summary>
        ///     Miles a day a full ox team makes at a steady pace out on the open plains.
        /// </summary>
        public const int PlainsMilesPerDay = 20;

        /// <summary>
        ///     Miles a day a full ox team makes at a steady pace once the trail turns mountainous past Fort Laramie.
        /// </summary>
        public const int MountainMilesPerDay = 12;

        /// <summary>
        ///     Base miles a day for the leg leading away from this location, before the ox team and pace scale it. The
        ///     original kept this per leg rather than per region: twenty across the plains, twelve from Fort Laramie west.
        /// </summary>
        public int BaseMilesPerDay { get; set; } = MountainMilesPerDay;

        /// <summary>
        ///     Miles from the fork that offers this location as a branch to the branch itself, for locations that are one.
        ///     A fork's own distance only describes staying on the main trail, so each branch has to carry the length of the
        ///     road leading to it: South Pass is 57 miles from the Green River but 125 from Fort Bridger. Zero for any
        ///     location that is not a fork branch.
        /// </summary>
        public int LegDistance { get; set; }

        /// <summary>
        ///     Name of the current point of interest as it should be known to the player.
        /// </summary>
        public string Name { get; }

        /// <summary>
        ///     Compares two objects and returns a value indicating whether one is less than, equal to, or greater than the
        ///     other.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <returns>
        ///     A signed integer that indicates the relative values of <paramref name="x" /> and <paramref name="y" />, as shown in
        ///     the following table.Value Meaning Less than zero<paramref name="x" /> is less than <paramref name="y" />.Zero
        ///     <paramref name="x" /> equals <paramref name="y" />.Greater than zero<paramref name="x" /> is greater than
        ///     <paramref name="y" />.
        /// </returns>
        public int Compare(IEntity x, IEntity y)
        {
            var result = string.Compare(x?.Name, y?.Name, StringComparison.Ordinal);
            if (result != 0) return result;

            return result;
        }

        /// <summary>Compares the current object with another object of the same type.</summary>
        /// <returns>
        ///     A value that indicates the relative order of the objects being compared. The return value has the following
        ///     meanings: Value Meaning Less than zero This object is less than the <paramref name="other" /> parameter.Zero This
        ///     object is equal to <paramref name="other" />. Greater than zero This object is greater than
        ///     <paramref name="other" />.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public int CompareTo(IEntity other)
        {
            var result = string.Compare(other.Name, Name, StringComparison.Ordinal);
            if (result != 0) return result;

            return result;
        }

        /// <summary>Indicates whether the current object is equal to another object of the same type.</summary>
        /// <returns>true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.</returns>
        /// <param name="other">An object to compare with this object.</param>
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

        /// <summary>Determines whether the specified objects are equal.</summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <returns>true if the specified objects are equal; otherwise, false.</returns>
        public bool Equals(IEntity x, IEntity y)
        {
            return x.Equals(y);
        }

        /// <summary>Returns a hash code for the specified object.</summary>
        /// <returns>A hash code for the specified object.</returns>
        /// <param name="obj">The <see cref="T:System.Object" /> for which a hash code is to be returned.</param>
        /// <exception cref="T:System.ArgumentNullException">
        ///     The type of <paramref name="obj" /> is a reference type and
        ///     <paramref name="obj" /> is null.
        /// </exception>
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
            // Skip system ticks.
            if (systemTick)
                return;

            // Weather belongs to the journey rather than to any one place, so the climate module advances it once a day
            // from the simulation itself; there is nothing location-specific left to tick here.
        }
    }
}