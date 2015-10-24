namespace TrailEntities
{
    public sealed class BulletsItem : Item
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailEntities.BulletsItem" /> class.
        /// </summary>
        public BulletsItem(uint cost) : base(cost)
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
        ///     Weight of a single item of this type, the original game used pounds so that is roughly what this should represent.
        /// </summary>
        public override uint Weight
        {
            get { return 0; }
        }

        /// <summary>
        ///     Total number of items this item represents.
        /// </summary>
        public override uint Quantity
        {
            get { return 20; }
        }
    }
}