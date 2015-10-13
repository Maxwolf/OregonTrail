using TrailCommon;

namespace TrailEntities
{
    /// <summary>
    ///     Medical event that will affect the party members if the roll chance triggers it will randomly select someone from
    ///     the list, if the roll count fires again it will never hit the same player twice with the same event.
    /// </summary>
    public class MedicalEvent : RandomEventBase
    {
        private Disease _disease;
        private uint _infectionCount;

        public MedicalEvent(RandomEvent action, string name, float rollChance, uint rollCount, Disease disease,
            uint infectionCount)
            : base(action, name, rollChance, rollCount)
        {
            _disease = disease;
            _infectionCount = infectionCount;
        }

        public Disease Disease
        {
            get { return _disease; }
        }

        public uint InfectionCount
        {
            get { return _infectionCount; }
        }
    }
}