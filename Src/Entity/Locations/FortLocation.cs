using System.Collections.Generic;

namespace OregonTrail
{
    public class FortLocation : LocationBase
    {
        private List<string> _advice;
        private List<ItemBase> _store;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:OregonTrail.FortLocation" /> class.
        /// </summary>
        public FortLocation(Condition condition, string name, float actionChance, LocationFlag description,
            Weather weather, List<string> advice, List<ItemBase> store)
            : base(condition, name, actionChance, description, weather)
        {
            _advice = advice;
            _store = store;
        }

        public List<ItemBase> Store
        {
            get { return _store; }
        }

        public List<string> Advice
        {
            get { return _advice; }
        }
    }
}