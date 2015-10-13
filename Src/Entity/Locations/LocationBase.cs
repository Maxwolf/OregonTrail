using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace OregonTrail
{
    public abstract class LocationBase : EntityBase, ILocation
    {
        private float _actionChance;
        private List<RandomEventBase> _actions = new List<RandomEventBase>();
        private LocationFlag _description;
        private List<Party> _parties = new List<Party>();
        private Weather _weather;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:OregonTrail.Location" /> class.
        /// </summary>
        protected LocationBase(Condition condition, string name, float actionChance, LocationFlag description,
            Weather weather) : base(condition, name)
        {
            _actionChance = actionChance;
            _description = description;
            _weather = weather;
        }

        public Weather Weather
        {
            get { return _weather; }
        }

        public ReadOnlyCollection<Party> Parties
        {
            get { return new ReadOnlyCollection<Party>(_parties); }
        }

        public LocationFlag Description
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