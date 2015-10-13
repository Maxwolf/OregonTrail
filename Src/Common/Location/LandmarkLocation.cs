namespace OregonTrail.Common
{
    /// <summary>
    ///     Area of the map that has no stores, but people camping out and resting. There are some people to talk to for advice
    ///     and trading, resting here will also provide better protection from thieves than just being out on the road.
    /// </summary>
    public abstract class LandmarkLocation : LocationBase
    {
        /// <summary>
        ///     Determines if this landmark will improve the condition of party members if they rest here.
        /// </summary>
        private bool _healParty;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:OregonTrail.Common.LandmarkLocation" /> class.
        /// </summary>
        protected LandmarkLocation(string name, WeatherTier weather, bool healParty)
            : base(ConditionTier.Good, name, 0.1f, LocationCategory.Landmark, weather)
        {
            _healParty = healParty;
        }

        /// <summary>
        ///     Determines if this landmark will improve the condition of party members if they rest here.
        /// </summary>
        public bool HealParty
        {
            get { return _healParty; }
        }
    }
}