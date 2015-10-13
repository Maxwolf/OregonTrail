using OregonTrail.Common;

namespace OregonTrail.Entity
{
    /// <summary>
    ///     Defines all of the logic for the player discovering a derelict vessel that can contain parts, food, and if they are
    ///     really lucky diseases.
    /// </summary>
    public class DerelictEvent : RandomEventBase
    {
        private bool _containsDisease;
        private bool _containsFood;

        public DerelictEvent(RandomEvent action, string name, float rollChance, uint rollCount, bool containsDisease,
            bool containsFood) : base(action, name, rollChance, rollCount)
        {
            _containsDisease = containsDisease;
            _containsFood = containsFood;
        }

        public bool ContainsDisease
        {
            get { return _containsDisease; }
        }

        public bool ContainsFood
        {
            get { return _containsFood; }
        }
    }
}