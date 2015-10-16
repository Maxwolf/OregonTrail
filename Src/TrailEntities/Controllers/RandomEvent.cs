using System;
using TrailCommon;

namespace TrailEntities
{
    public class RandomEvent : IRandomEvent
    {
        private string _name;
        private ITrailVehicle _trailVehicle;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailEntities.RandomEvent" /> class.
        /// </summary>
        public RandomEvent(ITrailVehicle trailVehicle)
        {
            _name = "Unknown Random Event";
            _trailVehicle = trailVehicle;
        }

        public string Name
        {
            get { return _name; }
        }

        public TrailModeType Mode
        {
            get { return TrailModeType.RandomEvent; }
        }

        public ITrailVehicle TrailVehicle
        {
            get { return _trailVehicle; }
        }

        public event ModeChanged ModeChangedEvent;

        public void MakeEvent()
        {
            throw new NotImplementedException();
        }

        public void CheckForRandomEvent()
        {
            throw new NotImplementedException();
        }
    }
}