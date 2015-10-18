using System;
using TrailCommon;

namespace TrailEntities
{
    public class RandomEventMode : IRandomEvent
    {
        private string _name;
        private IVehicle _vehicle;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailEntities.RandomEvent" /> class.
        /// </summary>
        public RandomEventMode(IVehicle vehicle)
        {
            _name = "Unknown Random Event";
            _vehicle = vehicle;
        }

        public string Name
        {
            get { return _name; }
        }

        public ModeType Mode
        {
            get { return ModeType.RandomEvent; }
        }

        public void TickMode()
        {
            throw new NotImplementedException();
        }

        public IVehicle Vehicle
        {
            get { return _vehicle; }
        }

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