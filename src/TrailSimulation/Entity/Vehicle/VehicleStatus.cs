// Created by Ron 'Maxwolf' McDowell (ron.mcdowell@gmail.com) 
// Timestamp 01/01/2016@7:40 PM

namespace TrailSimulation
{
    /// <summary>
    ///     Determines the current travel status of the vehicle, this is important because it could mean the difference between
    ///     the vehicle moving then ticked or staying still.
    /// </summary>
    public enum VehicleStatus
    {
        /// <summary>
        ///     Vehicle is stopped and no longer moving on the trail, this is typically used then at landmarks and locations. Will
        ///     also be used at river crossings and forks in the road.
        /// </summary>
        Stopped = 0,

        /// <summary>
        ///     Vehicle is moving and decreasing distance to next location on the trail with each turn made.
        /// </summary>
        Moving = 1,

        /// <summary>
        ///     Vehicle cannot move anymore because it has no animals or engine to pull it forward, or some critical piece of infrastructure has been destroyed.
        /// </summary>
        Disabled = 2
    }
}