using TrailCommon;

namespace TrailEntities
{
    public abstract class EntityBase : IEntity
    {
        private ConditionTier _condition;
        private string _name;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:OregonTrail.Entity" /> class.
        /// </summary>
        protected EntityBase(ConditionTier condition, string name)
        {
            _condition = condition;
            _name = name;
        }

        public string Name
        {
            get { return _name; }
        }

        public ConditionTier Condition
        {
            get { return _condition; }
        }
    }
}