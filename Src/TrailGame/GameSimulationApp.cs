using System;
using TrailCommon;
using TrailEntities;

namespace TrailGame
{
    /// <summary>
    ///     Handles core interaction of the game, all other game types are inherited from this game mode. Deals with weather,
    ///     parties, random events, keeping track of beginning and end of the game.
    /// </summary>
    public class GameSimulationApp : SimulationApp, IGameplay
    {
        private SimulationTime _simulationTime;
        private RandomEvent _randomEvent;
        private uint _turn;
        private TrailVehicle _trailVehicle;
        private Trail _trail;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailGame.GameSimulationApp" /> class.
        /// </summary>
        public GameSimulationApp()
        {
            _simulationTime = new SimulationTime(1985, Months.May, 1, TravelPace.Paused);
            _trail = new Trail();
            _turn = 0;
            _trailVehicle = new TrailVehicle();
            _randomEvent = new RandomEvent(TrailVehicle);
        }

        public Trail Trail
        {
            get { return _trail; }
        }

        public TrailVehicle TrailVehicle
        {
            get { return _trailVehicle; }
        }

        public RandomEvent RandomEvent
        {
            get { return _randomEvent; }
        }

        public SimulationTime SimulationTime
        {
            get { return _simulationTime; }
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
            TrailVehicle.UpdateVehicle();
        }

        private void UpdateTrail()
        {
            Trail.ReachedPointOfInterest();
        }

        private void UpdateClimate()
        {
            SimulationTime.UpdateClimate();
        }

        private void UpdateVehiclePosition()
        {
            TrailVehicle.DistanceTraveled += (uint) TrailVehicle.Pace;
        }
    }
}