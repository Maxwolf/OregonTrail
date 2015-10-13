namespace OregonTrail
{
    /// <summary>
    ///     Physical event that messes with the vessel that carries the entire party, if roll chance passes harm will come to
    ///     the vehicle and the parts inside of it.
    /// </summary>
    public class PhysicalEvent : TravelEvent
    {
        private int _damageParty;
        private int _damageVehicle;

        public PhysicalEvent(TravelingEvent action, string name, float rollChance, uint rollCount, int damageParty,
            int damageVehicle) : base(action, name, rollChance, rollCount)
        {
            _damageParty = damageParty;
            _damageVehicle = damageVehicle;
        }

        public int DamageParty
        {
            get { return _damageParty; }
        }

        public int DamageVehicle
        {
            get { return _damageVehicle; }
        }
    }
}