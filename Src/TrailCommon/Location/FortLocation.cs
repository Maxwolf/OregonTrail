using System.Collections.Generic;

namespace TrailCommon
{
    /// <summary>
    ///     Defines a location that has a store, people to talk to, safe to rest. Forts are always in good condition since they
    ///     are run
    ///     by the military and always have a source of funding to perform maintenance and upkeep.
    /// </summary>
    public abstract class FortLocation : LocationBase
    {
        private List<string> _advice;
        private List<IItem> _store;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailCommon.FortLocation" /> class.
        /// </summary>
        protected FortLocation(string name, float actionChance, WeatherTier weather, List<string> advice,
            List<IItem> store)
            : base(ConditionTier.Good, name, actionChance, LocationCategory.Fort, weather)
        {
            _advice = advice;
            _store = store;
        }

        public List<IItem> Store
        {
            get { return _store; }
        }

        public List<string> Advice
        {
            get { return _advice; }
        }
    }
}