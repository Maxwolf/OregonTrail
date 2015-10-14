using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace TrailCommon
{
    public abstract class PointOfInterest : IPointOfInterest
    {
        private string _name;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:TrailCommon.PointOfInterest"/> class.
        /// </summary>
        protected PointOfInterest(string name)
        {
            _name = name;
        }

        public string Name
        {
            get { return _name; }
        }
    }
}