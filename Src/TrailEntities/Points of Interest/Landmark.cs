using TrailCommon;

namespace TrailEntities
{
    public class Landmark : PointOfInterest, ILandmark
    {
        private bool _canRest;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailEntities.PointOfInterest" /> class.
        /// </summary>
        public Landmark(string name, ulong distanceLength, bool canRest) : base(name, distanceLength)
        {
            _canRest = canRest;
        }

        public override ModeType Mode
        {
            get { return ModeType.Landmark; }
        }

        public bool CanRest
        {
            get { return _canRest; }
        }
    }
}