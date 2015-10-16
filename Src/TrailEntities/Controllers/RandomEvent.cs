using System;
using TrailCommon;

namespace TrailEntities
{
    public class RandomEvent : IRandomEvent
    {
        private string _name;
        private IVehicle _vehicle;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailEntities.RandomEvent" /> class.
        /// </summary>
        public RandomEvent(IVehicle vehicle)
        {
            _name = "Unknown Random Event";
            _vehicle = vehicle;
        }

        public string Name
        {
            get { return _name; }
        }

        public GameMode ModeType
        {
            get { return GameMode.RandomEvent; }
        }

        public void TickMode()
        {
            throw new NotImplementedException();
        }

        public IVehicle Vehicle
        {
            get { return _vehicle; }
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