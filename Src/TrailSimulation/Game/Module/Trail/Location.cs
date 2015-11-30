using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace TrailSimulation.Game
{
    /// <summary>
    ///     Defines a location in the game that is added to a list of points that make up the entire trail which the player and
    ///     his vehicle travel upon.
    /// </summary>
    public class Location
    {
        /// <summary>
        ///     List of possible alternate locations the player might have to choose from if this list is not null and greater than
        ///     one. Game simulation will ask using modes and states what choice player would like.
        /// </summary>
        private List<Location> _skipChoices;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailSimulation.Game.Location" /> class.
        /// </summary>
        public Location(string name, bool hasStore = false, bool riverCrossing = false,
            IEnumerable<Location> skipChoices = null)
        {
            // Offers up a decision when traveling on the trail, there are normally one of many possible outcomes.
            if (skipChoices != null)
                _skipChoices = new List<Location>(skipChoices);

            // Name of the point as it should be known to the player.
            Name = name;

            // Does this location have a store, is it a river crossing.
            HasStore = hasStore;
            RiverCrossing = riverCrossing;
        }

        /// <summary>
        ///     Defines the current game mode the inheriting class is going to take responsibility for when attached to the
        ///     simulation.
        /// </summary>
        public virtual Mode Mode
        {
            get { return Mode.Travel; }
        }

        /// <summary>
        ///     Name of the current point of interest as it should be known to the player.
        /// </summary>
        public string Name { get; }

        /// <summary>
        ///     Determines if this location has a store which the player can buy items from using their monies.
        /// </summary>
        public bool HasStore { get; }

        /// <summary>
        ///     Determines if this location is an obstruction to the player that needs to be crossed.
        /// </summary>
        public bool RiverCrossing { get; }

        /// <summary>
        ///     Determines if this location has already been visited by the vehicle and party members.
        /// </summary>
        /// <returns>TRUE if location has been passed by, FALSE if location has yet to be reached.</returns>
        public bool HasVisited { get; private set; }

        /// <summary>
        ///     Defines all of the skip choices that were defined for this location. Will return null if there are no skip choices
        ///     associated with this location.
        /// </summary>
        public ReadOnlyCollection<Location> SkipChoices
        {
            get { return _skipChoices.AsReadOnly(); }
        }

        /// <summary>
        ///     Determines if the vehicle has reached this location or not, this can only be set once and will throw an exception
        ///     is hit after it is set.
        /// </summary>
        public void SetVisited()
        {
            // Complain if visited is already true.
            if (HasVisited)
                throw new InvalidOperationException("Visited flag for location " + Name +
                                                    " has already been set to TRUE. Cannot do this twice!");

            HasVisited = true;
        }
    }
}