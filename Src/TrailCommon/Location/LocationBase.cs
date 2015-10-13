using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace TrailCommon
{
    public abstract class LocationBase : ILocation
    {
        private float _actionChance;
        private List<IRandomEvent> _actions = new List<IRandomEvent>();
        private LocationCategory _description;
        private List<IParty> _parties = new List<IParty>();
        private WeatherTier _weather;
        private readonly string _name;
        private readonly ConditionTier _condition;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:TrailCommon.LocationBase"/> class.
        /// </summary>
        protected LocationBase(ConditionTier condition, string name, float actionChance, LocationCategory description, WeatherTier weather)
        {
            _actionChance = actionChance;
            _condition = condition;
            _description = description;
            _name = name;
            _weather = weather;
        }

        public WeatherTier Weather
        {
            get { return _weather; }
        }

        public ReadOnlyCollection<IParty> Parties
        {
            get { return new ReadOnlyCollection<IParty>(_parties); }
        }

        public LocationCategory Description
        {
            get { return _description; }
        }

        public ReadOnlyCollection<IRandomEvent> Actions
        {
            get { return new ReadOnlyCollection<IRandomEvent>(_actions); }
        }

        public float ActionChance
        {
            get { return _actionChance; }
        }

        public string Name
        {
            get { return _name; }
        }

        public ConditionTier Condition
        {
            get { return _condition; }
        }
    }
}