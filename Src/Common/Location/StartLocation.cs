namespace OregonTrail.Common
{
    /// <summary>
    ///     Starting point of all new games on a given trail. Condition is always good, zero chance of actions, weather clear.
    /// </summary>
    public abstract class StartLocation : LocationBase
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:OregonTrail.Common.StartLocation" /> class.
        /// </summary>
        protected StartLocation(string name)
            : base(ConditionTier.Good, name, 0.0f, LocationCategory.Start, WeatherTier.Warm)
        {
        }
    }
}