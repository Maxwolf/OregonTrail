using System;
using TrailCommon;

namespace TrailEntities
{
    public abstract class GameMode : IMode
    {
        protected Vehicle _vehicle;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailEntities.TrailMode" /> class.
        /// </summary>
        protected GameMode(Vehicle vehicle)
        {
            // Complain if game manager does not exist.
            if (SimulationApp.Instance == null)
                throw new InvalidOperationException("Called game window constructor when game manager is null!");

            _vehicle = vehicle;

            // Hook events that all game windows will want.
            SimulationApp.Instance.TickEvent += Simulation_TickEvent;
        }

        public abstract ModeType Mode { get; }

        public void TickMode()
        {
            throw new NotImplementedException();
        }

        public IVehicle Vehicle
        {
            get { return _vehicle; }
        }

        /// <summary>
        ///     Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        ///     A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return Mode.ToString();
        }

        private void Simulation_TickEvent(uint tickCount)
        {
            OnTick();
        }

        protected virtual void OnTick()
        {
            // Complain if game manager does not exist.
            if (SimulationApp.Instance == null)
                throw new InvalidOperationException("Unable to continue to tick game window since game manager is null!");
        }
    }
}