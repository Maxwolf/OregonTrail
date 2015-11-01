using System;
using System.Text;
using TrailCommon;

namespace TrailEntities
{
    /// <summary>
    ///     Receiver - The main logic will be implemented here and it knows how to perform the necessary actions.
    /// </summary>
    public sealed class GameSimulationApp : SimulationApp
    {
        /// <summary>
        ///     Manages weather, temperature, humidity, and current grazing level for living animals.
        /// </summary>
        private ClimateSim _climate;

        /// <summary>
        ///     Base interface for the event manager, it is ticked as a sub-system of the primary game simulation and can affect
        ///     game modes, people, and vehicles.
        /// </summary>
        private EventSim _director;

        /// <summary>
        ///     Manages time in a linear since from the provided ticks in base simulation class. Handles days, months, and years.
        /// </summary>
        private TimeSim _time;

        /// <summary>
        ///     Current vessel which the player character and his party are traveling inside of, provides means of transportation
        ///     other than walking.
        /// </summary>
        private Vehicle _vehicle;

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

        public IEventSimulation Director
        {
            get { return _director; }
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

        /// <summary>
        ///     Attaches the traveling mode and removes the new game mode if it exists, this begins the simulation down the trail
        ///     path and all the points of interest on it.
        /// </summary>
        /// <param name="startingInfo">User data object that was passed around the new game mode and populated by user selections.</param>
        public override void SetData(NewGameInfo startingInfo)
        {
            base.SetData(startingInfo);

            // Clear out any data amount items, monies, people that might have been in the vehicle.
            // NOTE: Sets starting monies, which was determined by player profession selection.
            Vehicle.ResetVehicle(startingInfo.StartingMonies);

            // Add all the player data we collected from attached game mode states.
            var crewNumber = 1;
            foreach (var name in startingInfo.PlayerNames)
            {
                // First name in list is always the leader.
                var isLeader = startingInfo.PlayerNames.IndexOf(name) == 0 && crewNumber == 1;
                Vehicle.AddPerson(new Person(startingInfo.PlayerProfession, name, isLeader));
                crewNumber++;
            }

            // Set the starting month to match what the user selected.
            Time.SetMonth(startingInfo.StartingMonth);
        }

        /// <summary>
        ///     Prints game mode specific text and options.
        /// </summary>
        protected override string OnTickTUI()
        {
            // Spinning ticker that shows activity, lets us know if application hangs or freezes.
            var tui = new StringBuilder();
            tui.Append($"\r[ {TickPhase} ] - ");

            // Keeps track of active mode name and active mode current state name for debugging purposes.
            tui.Append(ActiveMode?.CurrentState != null
                ? $"Mode({Modes.Count}): {ActiveMode}({ActiveMode.CurrentState}) - "
                : $"Mode({Modes.Count}): {ActiveMode}(NO STATE) - ");

            // Total number of turns that have passed in the simulation.
            tui.Append($"Turns: {TotalTurns.ToString("D4")}\n");

            // Prints game mode specific text and options. This typically is menus from commands, or states showing some information.
            tui.Append($"{base.OnTickTUI()}\n");

            // Only print and accept user input if there is a game mode and menu system to support it.
            if (AcceptingInput)
            {
                // Allow user to see their input from buffer.
                tui.Append($"User Input: {InputBuffer}");
            }

            // Outputs the result of the string builder to TUI builder above.
            return tui.ToString();
        }

        /// <summary>
        ///     Fired by messaging system or user interface that wants to interact with the simulation by sending string command
        ///     that should be able to be parsed into a valid command that can be run on the current game mode.
        /// </summary>
        /// <param name="returnedLine">Passed in command from controller, text was trimmed but nothing more.</param>
        protected override void OnInputBufferReturned(string returnedLine)
        {
            // Pass command along to currently active game mode if it exists.
            ActiveMode?.SendInputBuffer(returnedLine.Trim());
        }

        /// <summary>
        ///     Creates new instance of game simulation. Complains if instance already exists.
        /// </summary>
        public static void Create()
        {
            if (Instance != null)
                throw new InvalidOperationException(
                    "Unable to create new instance of game simulation since it already exists!");

            Instance = new GameSimulationApp();
        }

        protected override void OnDestroy()
        {
            // Unhook delegates from time events.
            if (_time != null)
            {
                _time.DayEndEvent -= TimeSimulation_DayEndEvent;
                _time.MonthEndEvent -= TimeSimulation_MonthEndEvent;
                _time.YearEndEvent -= TimeSimulation_YearEndEvent;
                _time.SpeedChangeEvent -= TimeSimulation_SpeedChangeEvent;
            }

            // Unhook director event manager events.
            if (_director != null)
            {
                _director.EventAdded -= DirectorOnEventAdded;
            }

            // Destroy all instances.
            _time = null;
            _climate = null;
            _director = null;
            TrailSim = null;
            TotalTurns = 0;
            _vehicle = null;
            Instance = null;

            base.OnDestroy();
        }

        /// <summary>
        ///     Fired when the simulation is loaded and makes it very first tick using the internal timer mechanism keeping track
        ///     of ticks to keep track of seconds.
        /// </summary>
        protected override void OnFirstTick()
        {
            base.OnFirstTick();

            // Linear time simulation with ticks.
            _time = new TimeSim(1848, Months.March, 1, TravelPace.Paused);
            _time.DayEndEvent += TimeSimulation_DayEndEvent;
            _time.MonthEndEvent += TimeSimulation_MonthEndEvent;
            _time.YearEndEvent += TimeSimulation_YearEndEvent;
            _time.SpeedChangeEvent += TimeSimulation_SpeedChangeEvent;

            // Director event manager, and his delegate.
            _director = new EventSim();
            _director.EventAdded += DirectorOnEventAdded;

            // Environment, weather, conditions, climate, tail, vehicle, stats.
            _climate = new ClimateSim(ClimateClassification.Moderate);
            TrailSim = new TrailSim(Trails.OregonTrail());
            TotalTurns = 0;
            _vehicle = new Vehicle();

            // Attach traveling mode since that is the default and bottom most game mode.
            AddMode(ModeType.Travel);

            // Forcefully trigger the arrival on the first spot on the trail.
            TrailSim.ReachedPointOfInterest();

            // Add the new game configuration screen that asks for names, profession, and lets user buy initial items.
            AddMode(ModeType.NewGame);
        }

        private void DirectorOnEventAdded(IEventItem theEvent)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Change to new view mode when told that internal logic wants to display view options to player for a specific set of
        ///     data in the simulation.
        /// </summary>
        /// <param name="modeType">Enumeration of the game mode that requested to be attached.</param>
        /// <returns>New game mode instance based on the mode input parameter.</returns>
        protected override IMode OnModeChanging(ModeType modeType)
        {
            switch (modeType)
            {
                case ModeType.Travel:
                case ModeType.Location:
                    return new TravelingMode();
                case ModeType.ForkInRoad:
                    return new ForkInRoadMode();
                case ModeType.Hunt:
                    return new HuntingMode();
                case ModeType.NewGame:
                    return new NewGameMode();
                case ModeType.RandomEvent:
                    return new RandomEventMode();
                case ModeType.RiverCrossing:
                    return new RiverCrossingMode();
                case ModeType.Store:
                    return new StoreMode();
                case ModeType.StartingStore:
                    return new StoreMode(true);
                case ModeType.Trade:
                    return new TradingMode();
                default:
                    throw new ArgumentOutOfRangeException(nameof(modeType), modeType, null);
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
            _vehicle.Odometer += (uint) Vehicle.Pace;

            //Console.WriteLine("Day end!");
        }

        private void TimeSimulation_MonthEndEvent(uint monthCount)
        {
            //Console.WriteLine("Month end!");
        }
    }
}