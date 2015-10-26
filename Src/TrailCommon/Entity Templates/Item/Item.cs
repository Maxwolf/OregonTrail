namespace TrailCommon
{
    /// <summary>
    ///     Defines a base item which can represent almost any commodity the player can purchase for the party or vehicle.
    /// </summary>
    public abstract class Item
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailCommon.Item" /> class.
        /// </summary>
        protected Item(float cost, uint minimumAmount)
        {
            Cost = cost;
            MinimumAmount = minimumAmount;
        }

        /// <summary>
        ///     Cost of the item in monies.
        /// </summary>
        public float Cost { get; }

        /// <summary>
        ///     Display name of the item as it should be known to players.
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        ///     Single unit of the items name, for example is there is an Oxen item each one of those items is referred to as an
        ///     'ox'.
        /// </summary>
        public abstract string DelineatingUnit { get; }

        /// <summary>
        ///     Weight of a single item of this type, the original game used pounds so that is roughly what this should represent.
        /// </summary>
        public abstract uint Weight { get; }

        /// <summary>
        ///     Total number of items this item represents.
        /// </summary>
        public uint MinimumAmount { get; }

        /// <summary>
        ///     Total number of pounds which this the item multiplied by MinimumAmount would be.
        /// </summary>
        public uint TotalWeight
        {
            get { return Weight*MinimumAmount; }
        }

        /// <summary>
        ///     Limit on the number of items that are possible to have of this particular type.
        /// </summary>
        public abstract uint CarryLimit { get; }

        /// <summary>
        ///     Shows off a representation of the item as cost per delineating unit of the particular item.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{Cost.ToString("F2")} per {DelineatingUnit}";
        }
    }
}