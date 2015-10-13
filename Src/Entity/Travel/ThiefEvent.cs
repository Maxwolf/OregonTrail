using OregonTrail.Common;

namespace OregonTrail.Entity
{
    /// <summary>
    ///     Logic for controlling a thief that will take food and part items from the party. It is also possible for the thief
    ///     to murder one of the party members if it is determined they tried to resist. The chance of this happening is very
    ///     low though.
    /// </summary>
    public class ThiefEvent : RandomEventBase
    {
        private bool _murderer;

        public ThiefEvent(RandomEvent action, string name, float rollChance, uint rollCount, bool murderer)
            : base(action, name, rollChance, rollCount)
        {
            _murderer = murderer;
        }

        public bool Murderer
        {
            get { return _murderer; }
        }
    }
}