using TrailCommon;

namespace TrailEntities
{
    /// <summary>
    ///     Defines a base item which can represent almost any commodity the player can purchase for the party or vehicle.
    /// </summary>
    public abstract class Item : IItem
    {
        private readonly float _cost;
        private readonly uint _quantity;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailEntities.Item" /> class.
        /// </summary>
        protected Item(float cost, uint quantity)
        {
            _cost = cost;
            _quantity = quantity;
        }

        /// <summary>
        ///     Cost of the item in monies.
        /// </summary>
        public float Cost
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
        public uint Quantity
        {
            get { return _quantity; }
        }

        /// <summary>
        ///     Total number of pounds which this the item multiplied by quantity would be.
        /// </summary>
        public uint TotalWeight
        {
            get { return Weight*Quantity; }
        }

        /// <summary>
        ///     Limit on the number of items that are possible to have of this particular type.
        /// </summary>
        public abstract uint QuantityLimit { get; }
    }
}