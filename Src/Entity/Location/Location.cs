using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace OregonTrail
{
    public abstract class Location : Entity, ILocation
    {
        private float _actionChance;
        private List<TravelEvent> _actions = new List<TravelEvent>();
        private LocationType _description;
        private List<Party> _parties = new List<Party>();
        private Weather _weather;

        protected Location(float actionChance, LocationType description, Weather weather)
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

        public LocationType Description
        {
            get { return _description; }
        }

        public ReadOnlyCollection<TravelEvent> Actions
        {
            get { return new ReadOnlyCollection<TravelEvent>(_actions); }
        }

        public float ActionChance
        {
            get { return _actionChance; }
        }
    }
}