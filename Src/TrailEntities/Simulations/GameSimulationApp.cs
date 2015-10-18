using System;
using TrailCommon;

namespace TrailEntities
{
    /// <summary>
    ///     Handles core interaction of the game, all other game types are inherited from this game mode. Deals with weather,
    ///     parties, random events, keeping track of beginning and end of the game.
    /// </summary>
    public class GameSimulationApp : SimulationApp, IGameSimulation
    {
        private ClimateSimulation _climate;
        private RandomEventMode _randomEventMode;
        private TimeSimulation _time;
        private TrailSimulation _trailSimulation;
        private Vehicle _vehicle;
        private uint _turn;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailGame.GameSimulationApp" /> class.
        /// </summary>
        public GameSimulationApp()
        {
            _time = new TimeSimulation(1985, Months.May, 5, TravelPace.Paused);
            _time.DayEndEvent += TimeSimulation_DayEndEvent;
            _time.MonthEndEvent += TimeSimulation_MonthEndEvent;
            _time.YearEndEvent += TimeSimulation_YearEndEvent;
            _time.SpeedChangeEvent += TimeSimulation_SpeedChangeEvent;

            _climate = new ClimateSimulation(this, ClimateClassification.Moderate);
            _trailSimulation = new TrailSimulation();
            _turn = 0;
            _vehicle = new Vehicle();
            _randomEventMode = new TrailEntities.RandomEventMode(Vehicle);
            
        }

        public override void ChooseProfession()
        {
            throw new NotImplementedException();
        }

        public override void BuyInitialItems()
        {
            throw new NotImplementedException();
        }

        public override void ChooseNames()
        {
            throw new NotImplementedException();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            // Unhook delegates from events.
            _time.DayEndEvent -= TimeSimulation_DayEndEvent;
            _time.MonthEndEvent -= TimeSimulation_MonthEndEvent;
            _time.YearEndEvent -= TimeSimulation_YearEndEvent;
            _time.SpeedChangeEvent -= TimeSimulation_SpeedChangeEvent;

            // Destroy all instances.
            _time = null;
            _climate = null;
            _trailSimulation = null;
            _turn = 0;
            _vehicle = null;
            _randomEventMode = null;
        }

        private void TimeSimulation_SpeedChangeEvent()
        {
        }

        private void TimeSimulation_YearEndEvent(uint yearCount)
        {
        }

        private void TimeSimulation_DayEndEvent(uint dayCount)
        {
            _climate.TickClimate();
        }

        private void TimeSimulation_MonthEndEvent(uint monthCount)
        {
        }

        public TrailSimulation TrailSimulation
        {
            get { return _trailSimulation; }
        }

        public Vehicle Vehicle
        {
            get { return _vehicle; }
        }

        public TrailEntities.RandomEventMode RandomEventMode
        {
            get { return _randomEventMode; }
        }

        public uint Turn
        {
            get { return _turn; }
        }

        public void TakeTurn()
        {
            
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

        public ITimeSimulation Time
        {
            get { return _time; }
        }

        public IClimateSimulation Climate
        {
            get { return _climate; }
        }

        protected override void OnTick()
        {
            _time.TickTime();            
        }

        private void UpdateVehicle()
        {
            Vehicle.UpdateVehicle();
        }

        private void UpdateTrail()
        {
            TrailSimulation.ReachedPointOfInterest();
        }

        private void UpdateClimate()
        {
            _climate.UpdateClimate();
        }

        private void UpdateVehiclePosition()
        {
            Vehicle.DistanceTraveled += (uint) Vehicle.Pace;
        }
    }
}