using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrailEntities
{
    public sealed class VehicleTongueItem : PartItem
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailEntities.Item" /> class.
        /// </summary>
        public VehicleTongueItem(string name, uint quantity, uint weight, uint cost) : base(name, quantity, weight, cost)
        {
        }
    }
}
