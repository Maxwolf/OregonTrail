using System;

namespace TrailSimulation.Game
{
    /// <summary>
    ///     Defines a location in the game that is added to a list of points that make up the entire trail which the player and
    ///     his vehicle travel upon.
    /// </summary>
    public class Location
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailSimulation.Game.Location" /> class.
        /// </summary>
        public Location(string name, bool storeOpen = false)
        {
            // Name of the point as it should be known to the player.
            Name = name;
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
        ///     Determines if this location has already been visited by the vehicle and party members.
        /// </summary>
        /// <returns>TRUE if location has been passed by, FALSE if location has yet to be reached.</returns>
        public bool HasVisited { get; private set; }

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