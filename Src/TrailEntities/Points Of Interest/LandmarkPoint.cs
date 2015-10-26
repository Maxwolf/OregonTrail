using TrailCommon;

namespace TrailEntities
{
    public sealed class LandmarkPoint : PointOfInterest, ILandmarkPoint
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailEntities.PointOfInterest" /> class.
        /// </summary>
        public LandmarkPoint(string name, ulong distanceLength, bool canRest) : base(name, distanceLength)
        {
            CanRest = canRest;
        }

        public override ModeType ModeType
        {
            get { return ModeType.Landmark; }
        }

        public bool CanRest { get; }
    }
}