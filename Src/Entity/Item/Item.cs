namespace OregonTrail
{
    public abstract class Item : Entity, IItem
    {
        private int _cost;
        private int _quantity;

        public int Cost
        {
            get { return _cost; }
            set { _cost = value; }
        }

        public int Quantity
        {
            get { return _quantity; }
            set { _quantity = value; }
        }
    }
}