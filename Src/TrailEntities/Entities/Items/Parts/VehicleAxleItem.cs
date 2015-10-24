namespace TrailEntities
{
    public sealed class VehicleAxleItem : PartItem
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailEntities.Item" /> class.
        /// </summary>
        public VehicleAxleItem(uint cost) : base(cost)
        {
        }

        /// <summary>
        ///     Display name of the item as it should be known to players.
        /// </summary>
        public override string Name
        {
            get { throw new System.NotImplementedException(); }
        }

        /// <summary>
        ///     Weight of a single item of this type, the original game used pounds so that is roughly what this should represent.
        /// </summary>
        public override uint Weight
        {
            get { throw new System.NotImplementedException(); }
        }

        /// <summary>
        ///     Total number of items this item represents.
        /// </summary>
        public override uint Quantity
        {
            get { throw new System.NotImplementedException(); }
        }
    }
}