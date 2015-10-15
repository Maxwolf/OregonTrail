using System;
using TrailCommon;

namespace TrailEntities
{
    public abstract class GameMode : IGameMode
    {
        protected Vehicle _vehicle;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailEntities.GameWindow" /> class.
        /// </summary>
        protected GameMode(Vehicle vehicle)
        {
            // Complain if game manager does not exist.
            if (GameManager.Instance == null)
                throw new InvalidOperationException("Called game window constructor when game manager is null!");

            _vehicle = vehicle;

            // Hook events that all game windows will want.
            GameManager.Instance.TickEvent += Instance_TickEvent;
        }

        public virtual string Name
        {
            get { return "Unknown"; }
        }

        public void ModeChange()
        {
            ModeChangedEvent?.Invoke();
        }

        public event ModeChanged ModeChangedEvent;

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
            return Name;
        }

        private void Instance_TickEvent(ulong tickCount)
        {
            OnTick();
        }

        protected virtual void OnTick()
        {
            // Complain if game manager does not exist.
            if (GameManager.Instance == null)
                throw new InvalidOperationException("Unable to continue to tick game window since game manager is null!");
        }
    }
}