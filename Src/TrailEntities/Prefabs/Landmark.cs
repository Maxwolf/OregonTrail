using TrailCommon;

namespace TrailEntities
{
    public class Landmark : PointOfInterest, ILandmark
    {
        private bool _canRest;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:TrailCommon.LocationBase"/> class.
        /// </summary>
        public Landmark(string name, bool canRest) : base(name)
        {
            _canRest = canRest;
        }

        public bool CanRest
        {
            get { return _canRest; }
        }
    }
}