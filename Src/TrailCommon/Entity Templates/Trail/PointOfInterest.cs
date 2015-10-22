namespace TrailCommon
{
    public abstract class PointOfInterest
    {
        private ulong _distanceLength;
        private string _name;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailCommon.PointOfInterest" /> class.
        /// </summary>
        protected PointOfInterest(string name, ulong distanceLength)
        {
            _distanceLength = distanceLength;
            _name = name;
        }

        public string Name
        {
            get { return _name; }
        }

        public ulong DistanceLength
        {
            get { return _distanceLength; }
        }

        public abstract SimulationMode Mode { get; }
    }
}