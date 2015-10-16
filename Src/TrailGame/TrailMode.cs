using System;
using TrailCommon;
using TrailEntities;

namespace TrailGame
{
    public abstract class TrailMode : ITrailMode
    {
        protected TrailVehicle _trailVehicle;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailEntities.GameWindow" /> class.
        /// </summary>
        protected TrailMode(TrailVehicle trailVehicle)
        {
            // Complain if game manager does not exist.
            if (SimulationApp.Instance == null)
                throw new InvalidOperationException("Called game window constructor when game manager is null!");

            _trailVehicle = trailVehicle;

            // Hook events that all game windows will want.
            SimulationApp.Instance.TickEvent += Simulation_TickEvent;
        }

        public abstract TrailModeType Mode { get; }

        public ITrailVehicle TrailVehicle
        {
            get { return _trailVehicle; }
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