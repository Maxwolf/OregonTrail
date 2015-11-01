namespace TrailCommon
{
    /// <summary>
    ///     Ammunition used in hunting game mode so the players can acquire food by hunting animals.
    /// </summary>
    public sealed class BulletsItem : Item
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailCommon.BulletsItem" /> class.
        /// </summary>
        public BulletsItem(float cost) : base(cost, 20)
        {
        }

        /// <summary>
        ///     Display name of the item as it should be known to players.
        /// </summary>
        public override string Name
        {
            get { return "Ammunition"; }
        }

        /// <summary>
        ///     Single unit of the items name, for example is there is an Oxen item each one of those items is referred to as an
        ///     'ox'.
        /// </summary>
        protected override string DelineatingUnit
        {
            get { return "box"; }
        }

        /// <summary>
        ///     Weight of a single item of this type, the original game used pounds so that is roughly what this should represent.
        /// </summary>
        protected override uint Weight
        {
            get { return 0; }
        }

        /// <summary>
        ///     Limit on the number of items that are possible to have of this particular type.
        /// </summary>
        public override uint CarryLimit
        {
            get { return 99; }
        }
    }
}