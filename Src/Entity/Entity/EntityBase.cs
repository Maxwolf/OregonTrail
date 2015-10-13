namespace OregonTrail
{
    public abstract class EntityBase : IEntity
    {
        private Condition _condition;
        private string _name;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:OregonTrail.Entity" /> class.
        /// </summary>
        protected EntityBase(Condition condition, string name)
        {
            _condition = condition;
            _name = name;
        }

        public string Name
        {
            get { return _name; }
        }

        public Condition Condition
        {
            get { return _condition; }
        }
    }
}