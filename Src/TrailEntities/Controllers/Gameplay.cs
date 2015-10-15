using System;
using TrailCommon;

namespace TrailEntities
{
    /// <summary>
    ///     Handles core interaction of the game, all other game types are inherited from this game mode. Deals with weather,
    ///     parties, random events, keeping track of beginning and end of the game.
    /// </summary>
    public class Gameplay : IGameplay
    {
        private Climate _climate;
        private RandomEvent _randomEvent;
        private uint _turn;
        private Vehicle _vehicle;
        private Trail _trail;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailEntities.Gameplay" /> class.
        /// </summary>
        public Gameplay()
        {
            _climate = new Climate();
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

        public Climate Climate
        {
            get { return _climate; }
        }

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
            Vehicle.UpdateVehicle();
        }

        private void UpdateTrail()
        {
            Trail.ReachedPointOfInterest();
        }

        private void UpdateClimate()
        {
            Climate.UpdateClimate();
        }

        private void UpdateVehiclePosition()
        {
            Vehicle.DistanceTraveled += (uint) Vehicle.Pace;
        }

        public void Tick()
        {
            throw new NotImplementedException();
        }
    }
}