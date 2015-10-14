using System;
using TrailCommon;

namespace TrailEntities
{
    /// <summary>
    ///     Handles core interaction of the game, all other game types are inherited from this game mode. Deals with weather,
    ///     parties, random events, keeping track of beginning and end of the game.
    /// </summary>
    public abstract class Gameplay : IGameplay
    {
        private RandomEvent _randomEvent;
        private Trail _trail;
        private uint _turn;
        private Vehicle _vehicle;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailEntities.Gameplay" /> class.
        /// </summary>
        protected Gameplay()
        {
            Climate = new Climate();
            _trail = new Trail();
            _turn = 0;
            _vehicle = new Vehicle();
            _randomEvent = new RandomEvent(Vehicle);
        }

        public Trail Trail
        {
            get { return _trail; }
        }

        public Vehicle Vehicle
        {
            get { return _vehicle; }
        }

        public RandomEvent RandomEvent
        {
            get { return _randomEvent; }
        }

        public Climate Climate { get; }

        public uint Turn
        {
            get { return _turn; }
        }

        public void TakeTurn()
        {
            throw new NotImplementedException();
        }

        public void Hunt()
        {
            throw new NotImplementedException();
        }

        public void Rest()
        {
            throw new NotImplementedException();
        }

        public void Trade()
        {
            throw new NotImplementedException();
        }

        private void Restart()
        {
            throw new NotImplementedException();
        }

        private void UpdateVehicle()
        {
            _vehicle.UpdateVehicle();
        }

        private void UpdateTrail()
        {
            _trail.ReachedPointOfInterest();
        }

        private void UpdateClimate()
        {
            Climate.UpdateClimate();
        }

        private void UpdateVehiclePosition()
        {
            _vehicle.DistanceTraveled += (uint) _vehicle.Pace;
        }
    }
}