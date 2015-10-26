using TrailCommon;

namespace TrailEntities
{
    /// <summary>
    ///     Required to keep the vehicle moving if this part is broken it must be replaced before the player can
    ///     continue their journey.
    /// </summary>
    public sealed class PartAxleItem : PartItem
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailEntities.PartAxleItem" /> class.
        /// </summary>
        public PartAxleItem(float cost) : base(cost, 1)
        {
        }

        /// <summary>
        ///     Display name of the item as it should be known to players.
        /// </summary>
        public override string Name
        {
            get { return "Vehicle Axle"; }
        }
    }
}