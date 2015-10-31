using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace TrailCommon
{
    /// <summary>
    ///     Represents a series of nodes that are called points of interest. The players vehicle and party travel down this
    ///     path and run into the points which offer up game modes, and random events. This in turn fuels the need for the
    ///     player to manage resources and turns the cog of the game advancing the player towards the end of the trail.
    /// </summary>
    public interface ITrail
    {
        /// <summary>
        ///     References all of the points that make up the entire trail.
        /// </summary>
        IEnumerable<PointOfInterest> PointsOfInterest { get; }

        /// <summary>
        ///     Reference to how many ticks are between the players vehicle and the next point of interest.
        /// </summary>
        ulong DistanceToNextPoint { get; }

        /// <summary>
        ///     Current location of the vehicle in regards to the index of the points of interest list.
        /// </summary>
        int VehicleLocation { get; }

        /// <summary>
        ///     Fired when the distance to next point reaches zero and we need to setup the next one.
        /// </summary>
        void ReachedPointOfInterest();

        /// <summary>
        ///     Locates the next point of interest if it exists in the list, if this method returns NULL then that means the next
        ///     point of interest is the end of the game when the distance to point reaches zero.
        /// </summary>
        PointOfInterest GetNextPointOfInterest();

        /// <summary>
        ///     Locates the current point of interest the players vehicle is in index wise.
        /// </summary>
        PointOfInterest GetCurrentPointOfInterest();

        /// <summary>
        ///     Event that will be fired when the next point of interest has been reached on the trail.
        /// </summary>
        event PointOfInterestReached OnReachPointOfInterest;
    }

    /// <summary>
    ///     Delegate that passes along the next point of interest that was reached to the event and any subscribers to it.
    /// </summary>
    /// <param name="nextPoint">Next point of interest that will be attached to game simulation.</param>
    public delegate void PointOfInterestReached(PointOfInterest nextPoint);
}