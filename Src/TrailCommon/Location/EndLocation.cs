namespace TrailCommon
{
    /// <summary>
    ///     Ending point of the game, when this location is reached by the party the game is considered complete. Scores will
    ///     then me tabulated based on performance.
    ///     To keep weird things from happening we force the weather to be warm, zero chance of actions, and good condition.
    /// </summary>
    public abstract class EndLocation : LocationBase
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailCommon.EndLocation" /> class.
        /// </summary>
        protected EndLocation(string name)
            : base(ConditionTier.Good, name, 0.0f, LocationCategory.End, WeatherTier.Warm)
        {
        }
    }
}