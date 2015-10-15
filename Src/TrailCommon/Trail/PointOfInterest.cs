namespace TrailCommon
{
    public abstract class PointOfInterest
    {
        private string _name;

        private ulong _distanceLength;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:TrailCommon.PointOfInterest"/> class.
        /// </summary>
        protected PointOfInterest(string name, ulong distanceLength)
        {
            _name = name;
            _distanceLength = distanceLength;
        }

        public string Name
        {
            get { return _name; }
        }

        public ulong DistanceLength
        {
            get { return _distanceLength; }
        }
    }
}