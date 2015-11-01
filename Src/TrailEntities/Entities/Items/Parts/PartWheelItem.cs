using TrailCommon;

namespace TrailEntities
{
    /// <summary>
    ///     Required to keep the vehicle moving down the path, if any of the wheel parts break they must be replaced before the
    ///     journey can continue.
    /// </summary>
    public sealed class PartWheelItem : PartItem
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailEntities.PartWheelItem" /> class.
        /// </summary>
        public PartWheelItem(float cost) : base(cost, 1)
        {
        }

        /// <summary>
        ///     Display name of the item as it should be known to players.
        /// </summary>
        public override string Name
        {
            get { return "Vehicle Wheel"; }
        }

        /// <summary>
        ///     Single unit of the items name, for example is there is an Oxen item each one of those items is referred to as an
        ///     'ox'.
        /// </summary>
        public override string DelineatingUnit
        {
            get { return "wheel"; }
        }

        /// <summary>
        ///     When multiple of this item exist in a stack or need to be referenced, such as "10 pounds of food" the 'pounds' is
        ///     very important to get correct in context. Another example of this property being used is for Oxen item, a single Ox
        ///     is the delineating and the plural form would be "Oxen".
        /// </summary>
        public override string PluralForm
        {
            get { return "wheels"; }
        }
    }
}