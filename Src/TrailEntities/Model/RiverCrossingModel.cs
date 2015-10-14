using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrailCommon;

namespace TrailEntities
{
    public class RiverCrossingModel : RiverCrossing
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailCommon.LocationBase" /> class.
        /// </summary>
        public RiverCrossingModel(string name, IVehicle vehicle) : base(name, vehicle)
        {
        }
    }
}
