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
        private static Gameplay _instance;
        private Climate _climate;
        private RandomEvent _randomEvent;
        private uint _turn;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailEntities.Gameplay" /> class.
        /// </summary>
        public Gameplay()
        {
            if (GameInitializer.Instance == null)
                throw new InvalidOperationException("Unable to create gameplay instance without using game initializer!");

            _climate = new Climate();
            Trail = new Trail();
            _turn = 0;
            Vehicle = new Vehicle();
            _randomEvent = new RandomEvent(Vehicle);
        }

        public static void Create()
        {
            _instance = new Gameplay();
        }

        public static void Destroy()
        {
            _instance = null;
        }

        public Trail Trail { get; }

        public Vehicle Vehicle { get; }

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

        public static Gameplay Instance
        {
            get { return _instance; }
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