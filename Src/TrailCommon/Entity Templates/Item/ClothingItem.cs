namespace TrailCommon
{
    public sealed class ClothingItem : Item
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailEntities.Item" /> class.
        /// </summary>
        public ClothingItem(float cost) : base(cost, 1)
        {
        }

        /// <summary>
        ///     Display name of the item as it should be known to players.
        /// </summary>
        public override string Name
        {
            get { return "Sets of Clothing"; }
        }

        /// <summary>
        ///     Single unit of the items name, for example is there is an Oxen item each one of those items is referred to as an
        ///     'ox'.
        /// </summary>
        public override string DelineatingUnit
        {
            get { return "set"; }
        }

        /// <summary>
        ///     Weight of a single item of this type, the original game used pounds so that is roughly what this should represent.
        /// </summary>
        public override uint Weight
        {
            get { return 0; }
        }

        /// <summary>
        ///     Limit on the number of items that are possible to have of this particular type.
        /// </summary>
        public override uint CarryLimit
        {
            get { return 50; }
        }
    }
}