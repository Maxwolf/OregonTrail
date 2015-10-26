namespace TrailCommon
{
    public abstract class PointOfInterest
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailCommon.PointOfInterest" /> class.
        /// </summary>
        protected PointOfInterest(string name, ulong distanceLength)
        {
            DistanceLength = distanceLength;
            Name = name;
        }

        public string Name { get; }

        public ulong DistanceLength { get; }

        public abstract ModeType ModeType { get; }
    }
}