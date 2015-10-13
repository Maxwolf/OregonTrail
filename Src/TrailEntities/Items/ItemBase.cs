using TrailCommon;

namespace TrailEntities
{
    public abstract class ItemBase : EntityBase, IItem
    {
        private int _cost;
        private ItemCategory _description;
        private int _quantity;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:OregonTrail.Item" /> class.
        /// </summary>
        protected ItemBase(ConditionTier condition, string name, int cost, int quantity, ItemCategory description)
            : base(condition, name)
        {
            _cost = cost;
            _quantity = quantity;
            _description = description;
        }

        public ItemCategory Description
        {
            get { return _description; }
        }

        public int Cost
        {
            get { return _cost; }
        }

        public int Quantity
        {
            get { return _quantity; }
        }
    }
}