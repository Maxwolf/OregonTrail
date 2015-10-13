namespace OregonTrail
{
    public abstract class Entity : IEntity
    {
        private Condition _condition;
        private string _name;

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public Condition Condition
        {
            get { return _condition; }
            set { _condition = value; }
        }
    }
}