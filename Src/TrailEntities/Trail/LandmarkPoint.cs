using System.Collections.Generic;

namespace TrailEntities
{
    /// <summary>
    ///     Defines a location that has a store, people to talk to, safe to rest. Forts are always in good condition since they
    ///     are run
    ///     by the military and always have a source of funding to perform maintenance and upkeep.
    /// </summary>
    public sealed class LandmarkPoint : PointOfInterest
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailEntities.LandmarkPoint" /> class.
        /// </summary>
        public LandmarkPoint(string name, ulong distanceLength, IEnumerable<Item> pointInventory = null,
            bool canRest = true) : base(name, distanceLength, pointInventory, canRest)
        {
        }

        /// <summary>
        ///     Defines the current game mode the inheriting class is going to take responsibility for when attached to the
        ///     simulation.
        /// </summary>
        public override ModeType ModeType
        {
            get { return ModeType.Location; }
        }
    }
}