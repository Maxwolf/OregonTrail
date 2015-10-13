namespace OregonTrail
{
    /// <summary>
    ///     Medical event that will affect the party members if the roll chance triggers it will randomly select someone from
    ///     the list, if the roll count fires again it will never hit the same player twice with the same event.
    /// </summary>
    public class MedicalEvent : TravelEvent
    {
        private Disease _disease;

        public MedicalEvent(RandomEvent action, string name, float rollChance, uint rollCount, Disease disease)
            : base(action, name, rollChance, rollCount)
        {
            _disease = disease;
        }

        public Disease Disease
        {
            get { return _disease; }
        }
    }
}