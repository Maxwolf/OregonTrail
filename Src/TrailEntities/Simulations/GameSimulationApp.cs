using System;
using System.Text;
using TrailCommon;

namespace TrailEntities
{
    /// <summary>
    /// Receiver - The main logic will be implemented here and it knows how to perform the necessary actions.
    /// </summary>
    public sealed class GameSimulationApp : SimulationApp, IGameSimulation
    {
        /// <summary>
        ///     Manages weather, temperature, humidity, and current grazing level for living animals.
        /// </summary>
        private ClimateSim _climate;

        /// <summary>
        ///     Manages time in a linear since from the provided ticks in base simulation class. Handles days, months, and years.
        /// </summary>
        private TimeSim _time;

        /// <summary>
        ///     Current vessel which the player character and his party are traveling inside of, provides means of transportation
        ///     other than walking.
        /// </summary>
        private Vehicle _vehicle;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailGame.GameSimulationApp" /> class.
        /// </summary>
        public GameSimulationApp()
        {
            _time = new TimeSim(1985, Months.May, 5, TravelPace.Paused);
            _time.DayEndEvent += TimeSimulation_DayEndEvent;
            _time.MonthEndEvent += TimeSimulation_MonthEndEvent;
            _time.YearEndEvent += TimeSimulation_YearEndEvent;
            _time.SpeedChangeEvent += TimeSimulation_SpeedChangeEvent;

            _climate = new ClimateSim(this, ClimateClassification.Moderate);
            TrailSim = new TrailSim();
            TotalTurns = 0;
            _vehicle = new Vehicle(this);
        }

        public TrailSim TrailSim { get; private set; }

        public static GameSimulationApp Instance { get; private set; }

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
        }

        public uint TotalTurns { get; private set; }

        public void TakeTurn()
        {
            TotalTurns++;
            _time.TickTime();
        }

        protected override string OnTickTUI()
        {
            // Title and current game mode.
            var tui = new StringBuilder();
            tui.Append($"\r[ {TimerTickPhase} ] - ");
            tui.Append($"Mode: {ActiveModeName} - ");
            tui.Append($"Turns: {TotalTurns.ToString("D4")}\n");

            // Prints game mode specific text and options.
            tui.Append($"{base.OnTickTUI()}\n");

            // Allow user to see their input from buffer.
            tui.Append($"User Input: {InputBuffer}");

            return tui.ToString();
        }

        /// <summary>
        ///     Creates new instance of game simulation. Complains if instance already exists.
        /// </summary>
        public static void Create()
        {
            if (Instance != null)
                throw new InvalidOperationException(
                    "Unable to create new instance of game simulation app since it already exists!");

            Instance = new GameSimulationApp();
        }

        /// <summary>
        ///     Fired by the currently ticking and active game mode in the simulation. Implementation is left entirely up to
        ///     concrete handlers for game mode.
        /// </summary>
        /// <param name="returnedLine">Passed in command from controller, was already checking if null, empty, or whitespace.</param>
        protected override void OnReceiveCommand(string returnedLine)
        {
            // Pass command along to currently active game mode if it exists.
            ActiveMode?.ProcessCommand(returnedLine);
        }

        public override void OnDestroy()
        {
            // Unhook delegates from events.
            if (_time != null)
            {
                _time.DayEndEvent -= TimeSimulation_DayEndEvent;
                _time.MonthEndEvent -= TimeSimulation_MonthEndEvent;
                _time.YearEndEvent -= TimeSimulation_YearEndEvent;
                _time.SpeedChangeEvent -= TimeSimulation_SpeedChangeEvent;
            }

            // Destroy all instances.
            _time = null;
            _climate = null;
            TrailSim = null;
            TotalTurns = 0;
            _vehicle = null;
            Instance = null;

            base.OnDestroy();
        }

        protected override void OnFirstTimerTick()
        {
            base.OnFirstTimerTick();

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
                    return new TravelMode();
                case ModeType.ForkInRoad:
                    return new ForkInRoadMode();
                case ModeType.Hunt:
                    return new HuntMode();
                case ModeType.Landmark:
                    return new LandmarkMode();
                case ModeType.NewGame:
                    return new NewGameMode();
                case ModeType.RandomEvent:
                    return new RandomEventMode();
                case ModeType.RiverCrossing:
                    return new RiverCrossingMode();
                case ModeType.Settlement:
                    return new SettlementMode();
                case ModeType.Store:
                    return new StoreMode();
                case ModeType.Trade:
                    return new TradeMode();
                default:
                    throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
            }
        }

        private void TimeSimulation_SpeedChangeEvent()
        {
            //Console.WriteLine("Travel pace changed to " + _vehicle.Pace);
        }

        private void TimeSimulation_YearEndEvent(uint yearCount)
        {
            //Console.WriteLine("Year end!");
        }

        private void TimeSimulation_DayEndEvent(uint dayCount)
        {
            _climate.TickClimate();
            Vehicle.UpdateVehicle();
            TrailSim.ReachedPointOfInterest();
            _vehicle.DistanceTraveled += (uint) Vehicle.Pace;

            //Console.WriteLine("Day end!");
        }

        private void TimeSimulation_MonthEndEvent(uint monthCount)
        {
            //Console.WriteLine("Month end!");
        }
    }
}