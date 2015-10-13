namespace OregonTrail
{
    public abstract class TravelEvent : ITravelEvent
    {
        private TravelingEvent _action;
        private string _name;
        private float _rollChance;
        private uint _rollCount;

        protected TravelEvent(TravelingEvent action, string name, float rollChance, uint rollCount)
        {
            _action = action;
            _name = name;
            _rollChance = rollChance;
            _rollCount = rollCount;
        }

        public string Name
        {
            get { return _name; }
        }

        public uint RollCount
        {
            get { return _rollCount; }
        }

        public TravelingEvent Action
        {
            get { return _action; }
        }

        public float RollChance
        {
            get { return _rollChance; }
        }
    }
}