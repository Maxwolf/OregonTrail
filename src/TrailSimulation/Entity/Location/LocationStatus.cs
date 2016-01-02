// Created by Ron 'Maxwolf' McDowell (ron.mcdowell@gmail.com) 
// Timestamp 01/01/2016@7:40 PM

namespace TrailSimulation
{
    /// <summary>
    ///     Determines the status of the location as it is known to the vehicle and persons inside of it. Used to keep track of
    ///     the vehicle as it arrives, visits, and then departs the location.
    /// </summary>
    public enum LocationStatus
    {
        /// <summary>
        ///     Location has not been visited by the player or vehicle yet.
        /// </summary>
        Unreached = 0,

        /// <summary>
        ///     Location has been visited, and the player is there right now.
        /// </summary>
        Arrived = 1,

        /// <summary>
        ///     Location has been visited, and player left to move onward with the trail.
        /// </summary>
        Departed = 2
    }
}