using TrailCommon;

namespace TrailEntities
{
    /// <summary>
    ///     Defines a base item which can represent almost any commodity the player can purchase for the party or vehicle.
    /// </summary>
    public abstract class Item : IItem
    {
        private readonly uint _cost;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailEntities.Item" /> class.
        /// </summary>
        protected Item(uint cost)
        {
            _cost = cost;
        }

        /// <summary>
        ///     Cost of the item in monies.
        /// </summary>
        public uint Cost
        {
            get { return _cost; }
        }

        /// <summary>
        ///     Display name of the item as it should be known to players.
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        ///     Weight of a single item of this type, the original game used pounds so that is roughly what this should represent.
        /// </summary>
        public abstract uint Weight { get; }

        /// <summary>
        ///     Total number of items this item represents.
        /// </summary>
        public abstract uint Quantity { get; }

        /// <summary>
        ///     Total number of pounds which this the item multiplied by quantity would be.
        /// </summary>
        public uint TotalWeight
        {
            get { return Weight*Quantity; }
        }
    }
}