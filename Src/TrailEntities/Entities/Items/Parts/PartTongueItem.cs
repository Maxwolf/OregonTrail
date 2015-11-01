using TrailCommon;

namespace TrailEntities
{
    /// <summary>
    ///     Required to keep the vehicle running, if the tongue breaks then the player will have to fix or replace it before
    ///     they can continue on the journey again.
    /// </summary>
    public sealed class PartTongueItem : PartItem
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailEntities.PartTongueItem" /> class.
        /// </summary>
        public PartTongueItem(float cost) : base(cost, 1)
        {
        }

        /// <summary>
        ///     Display name of the item as it should be known to players.
        /// </summary>
        public override string Name
        {
            get { return "Vehicle Tongue"; }
        }

        /// <summary>
        ///     Single unit of the items name, for example is there is an Oxen item each one of those items is referred to as an
        ///     'ox'.
        /// </summary>
        public override string DelineatingUnit
        {
            get { return "tongue"; }
        }

        /// <summary>
        ///     When multiple of this item exist in a stack or need to be referenced, such as "10 pounds of food" the 'pounds' is
        ///     very important to get correct in context. Another example of this property being used is for Oxen item, a single Ox
        ///     is the delineating and the plural form would be "Oxen".
        /// </summary>
        public override string PluralForm
        {
            get { return "tongues"; }
        }
    }
}