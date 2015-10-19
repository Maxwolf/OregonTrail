using System;
using TrailCommon;

namespace TrailEntities
{
    public sealed class GameSimulationApp : SimulationApp, IGameSimulation
    {
        private ClimateSimulation _climate;
        private TimeSimulation _time;
        private Vehicle _vehicle;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailGame.SimulationApp" /> class.
        /// </summary>
        public GameSimulationApp(SimulationType simulationType) : base(simulationType)
        {
            _time = new TimeSimulation(1985, Months.May, 5, TravelPace.Paused);
            _time.DayEndEvent += TimeSimulation_DayEndEvent;
            _time.MonthEndEvent += TimeSimulation_MonthEndEvent;
            _time.YearEndEvent += TimeSimulation_YearEndEvent;
            _time.SpeedChangeEvent += TimeSimulation_SpeedChangeEvent;

            _climate = new ClimateSimulation(this, ClimateClassification.Moderate);
            TrailSimulation = new TrailSimulation();
            TotalTurns = 0;
            Vehicle = new Vehicle();
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return $"Oregon Trail {SimulationType} - " +
                    $"Turns: {TotalTurns.ToString("D4")} - " +
                    $"Mode: {ActiveModeName} - " +
                    $"Clients: {TotalClients} - " +
                    $"[{TickPhase}]";
        }

        public TrailSimulation TrailSimulation { get; private set; }

        public override void OnDestroy()
        {
            // Unhook delegates from events.
            _time.DayEndEvent -= TimeSimulation_DayEndEvent;
            _time.MonthEndEvent -= TimeSimulation_MonthEndEvent;
            _time.YearEndEvent -= TimeSimulation_YearEndEvent;
            _time.SpeedChangeEvent -= TimeSimulation_SpeedChangeEvent;

            // Destroy all instances.
            _time = null;
            _climate = null;
            TrailSimulation = null;
            TotalTurns = 0;
            Vehicle = null;

            base.OnDestroy();
        }

        public ITimeSimulation Time
        {
            get { return _time; }
        }

        public IClimateSimulation Climate
        {
            get { return _climate; }
        }

        public IVehicle Vehicle
        {
            get { return _vehicle; }
            private set { _vehicle = value as Vehicle; }
        }

        public uint TotalTurns { get; private set; }

        public void TakeTurn()
        {
            TotalTurns++;
            _time.TickTime();
        }

        protected override void OnFirstTick()
        {
            base.OnFirstTick();

            // Add the new game configuration screen that asks for names, profession, and lets user buy initial items.
            AddMode(ModeType.NewGame);
        }

        /// <summary>
        ///     Change to new view mode when told that internal logic wants to display view options to player for a specific set of
        ///     data in the simulation.
        /// </summary>
        /// <param name="mode">Enumeration of the game mode that requested to be attached.</param>
        /// <returns>New game mode instance based on the mode input parameter.</returns>
        protected override GameMode OnModeChanging(ModeType mode)
        {
            switch (mode)
            {
                case ModeType.Travel:
                    return new TravelMode(this);
                case ModeType.ForkInRoad:
                    return new ForkInRoadMode(this);
                case ModeType.Hunt:
                    return new HuntMode(this);
                case ModeType.Landmark:
                    return new LandmarkMode(this);
                case ModeType.NewGame:
                    return new NewGameMode(this);
                case ModeType.RandomEvent:
                    return new RandomEventMode(this);
                case ModeType.RiverCrossing:
                    return new RiverCrossingMode(this);
                case ModeType.Settlement:
                    return new SettlementMode(this);
                case ModeType.Store:
                    return new StoreMode(this);
                case ModeType.Trade:
                    return new TradeMode(this);
                default:
                    throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
            }
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
            Vehicle.UpdateVehicle();
            TrailSimulation.ReachedPointOfInterest();
            _vehicle.DistanceTraveled += (uint) Vehicle.Pace;
        }

        private void TimeSimulation_MonthEndEvent(uint monthCount)
        {
        }
    }
}