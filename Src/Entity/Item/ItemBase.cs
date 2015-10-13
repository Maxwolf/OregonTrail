namespace OregonTrail
{
    public abstract class ItemBase : EntityBase, IItem
    {
        private int _cost;
        private int _quantity;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:OregonTrail.Item" /> class.
        /// </summary>
        protected ItemBase(Condition condition, string name, int cost, int quantity) : base(condition, name)
        {
            _cost = cost;
            _quantity = quantity;
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