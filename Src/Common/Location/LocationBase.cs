using System.Collections.Generic;
using System.Collections.ObjectModel;
using OregonTrail.Entity;

namespace OregonTrail.Common
{
    public abstract class LocationBase : EntityBase, ILocation
    {
        private float _actionChance;
        private List<RandomEventBase> _actions = new List<RandomEventBase>();
        private LocationCategory _description;
        private List<Party> _parties = new List<Party>();
        private WeatherTier _weather;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:OregonTrail.Location" /> class.
        /// </summary>
        protected LocationBase(ConditionTier condition, string name, float actionChance, LocationCategory description,
            WeatherTier weather) : base(condition, name)
        {
            _actionChance = actionChance;
            _description = description;
            _weather = weather;
        }

        public WeatherTier Weather
        {
            get { return _weather; }
        }

        public ReadOnlyCollection<Party> Parties
        {
            get { return new ReadOnlyCollection<Party>(_parties); }
        }

        public LocationCategory Description
        {
            get { return _description; }
        }

        public ReadOnlyCollection<RandomEventBase> Actions
        {
            get { return new ReadOnlyCollection<RandomEventBase>(_actions); }
        }

        public float ActionChance
        {
            get { return _actionChance; }
        }
    }
}