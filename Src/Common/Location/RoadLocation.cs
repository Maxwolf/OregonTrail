namespace OregonTrail.Common
{
    /// <summary>
    ///     Defines a section of the game which must be traveled by the player in one go, they can stop at any point but cannot
    ///     go backwards and must always decide to continue forward. Stopping only gives options to rest and configure party
    ///     and vehicle.
    /// </summary>
    public abstract class RoadLocation : LocationBase
    {
        private uint _totalDistance;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:OregonTrail.Common.RoadLocation" /> class.
        /// </summary>
        protected RoadLocation(ConditionTier condition, string name, float actionChance, WeatherTier weather,
            uint totalDistance)
            : base(condition, name, actionChance, LocationCategory.Road, weather)
        {
            _totalDistance = totalDistance;
        }

        public uint TotalDistance
        {
            get { return _totalDistance; }
        }
    }
}