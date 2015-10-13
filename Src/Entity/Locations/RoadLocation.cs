namespace OregonTrail
{
    public class RoadLocation : LocationBase
    {
        private uint _totalDistance;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:OregonTrail.RoadLocation" /> class.
        /// </summary>
        public RoadLocation(Condition condition, string name, float actionChance, LocationFlag description,
            Weather weather, uint totalDistance) : base(condition, name, actionChance, description, weather)
        {
            _totalDistance = totalDistance;
        }

        public uint TotalDistance
        {
            get { return _totalDistance; }
        }
    }
}