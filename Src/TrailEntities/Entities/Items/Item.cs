using TrailCommon;

namespace TrailEntities
{
    /// <summary>
    ///     Defines a base item which can represent almost any commodity the player can purchase for the party or vehicle.
    /// </summary>
    public abstract class Item : IItem
    {
        private readonly uint _cost;
        private readonly string _name;
        private readonly uint _quantity;
        private readonly uint _weight;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:System.Object" /> class.
        /// </summary>
        protected Item(string name, uint cost, uint quantity, uint weight)
        {
            _cost = cost;
            _name = name;
            _quantity = quantity;
            _weight = weight;
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
        public string Name
        {
            get { return _name; }
        }

        /// <summary>
        ///     Weight of a single item of this type, the original game used pounds so that is roughly what this should represent.
        /// </summary>
        public uint Weight
        {
            get { return _weight; }
        }

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
    }
}