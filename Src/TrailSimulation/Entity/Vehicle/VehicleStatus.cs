namespace TrailSimulation.Entity
{
    /// <summary>
    ///     Determines the current travel status of the vehicle, this is important because it could mean the difference between
    ///     the vehicle moving then ticked or staying still.
    /// </summary>
    public enum VehicleStatus
    {
        /// <summary>
        ///     Players are not moving and ticking time by and waiting for weather, illness to go away, or some other reason.
        /// </summary>
        Resting = 0,

        /// <summary>
        ///     Vehicle is moving and decreasing distance to next location on the trail with each turn made.
        /// </summary>
        Moving = 1,

        /// <summary>
        ///     Vehicle is not moving and currently is delayed, this is typically because it is broken because of something like
        ///     tongue, wagon wheel, or some other component preventing the vehicle from functioning properly.
        /// </summary>
        Delayed = 2,

        /// <summary>
        ///     Vehicle is stopped and no longer moving on the trail, this is typically used then at landmarks and locations. Will
        ///     also be used at river crossings and forks in the road.
        /// </summary>
        Stopped = 3
    }
}