using System;
using System.Collections.Generic;
using System.Text;

namespace TrailEntities
{
    /// <summary>
    ///     Receiver - The main logic will be implemented here and it knows how to perform the necessary actions.
    /// </summary>
    public sealed class GameSimApp : SimApp
    {
        /// <summary>
        ///     Holds a constant representation of the string telling the user to press enter key to continue so we don't repeat
        ///     ourselves.
        /// </summary>
        public const string PRESS_ENTER = "Press ENTER KEY to continue";

        /// <summary>
        ///     Defines the limit on the number of players for the vehicle that will be allowed. This also determines how many
        ///     names are asked for in new game mode.
        /// </summary>
        public const int MAX_PLAYERS = 4;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailGame.GameSimApp" /> class.
        /// </summary>
        private GameSimApp()
        {
            // Repair status reference dictionary.
            RepairLevels = new Dictionary<string, int>();
            foreach (var repairStat in Enum.GetNames(typeof (RepairStatus)))
            {
                RepairLevels.Add(repairStat, (int) Enum.Parse(typeof (RepairStatus), repairStat));
            }
        }

        /// <summary>
        ///     References all of the possible repair status mapped to their string and integer values for easy compares without
        ///     using reflection all the time.
        /// </summary>
        public Dictionary<string, int> RepairLevels { get; }

        /// <summary>
        ///     Keeps track of all the points of interest we want to visit from beginning to end that makeup the entire journey.
        /// </summary>
        public TrailSim Trail { get; private set; }

        /// <summary>
        ///     Singleton instance for the entire game simulation, does not block the calling thread though only listens for
        ///     commands.
        /// </summary>
        public static GameSimApp Instance { get; private set; }

        /// <summary>
        ///     Manages time in a linear since from the provided ticks in base simulation class. Handles days, months, and years.
        /// </summary>
        public TimeSim Time { get; private set; }

        /// <summary>
        ///     Manages weather, temperature, humidity, and current grazing level for living animals.
        /// </summary>
        public ClimateSim Climate { get; private set; }

        /// <summary>
        ///     Keeps track of the total number of points the player has earned through the course of the game.
        /// </summary>
        public List<Highscore> ScoreTopTen { get; private set; }

        /// <summary>
        ///     Base interface for the event manager, it is ticked as a sub-system of the primary game simulation and can affect
        ///     game modes, people, and vehicles.
        /// </summary>
        public EventSim Director { get; private set; }

        /// <summary>
        ///     Current vessel which the player character and his party are traveling inside of, provides means of transportation
        ///     other than walking.
        /// </summary>
        public Vehicle Vehicle { get; private set; }

        /// <summary>
        ///     Total number of turns that have taken place. Typically a game will not go past eighteen (18) turns or 20+ weeks
        ///     (246 days) or approximately two-thousand (2000) miles.
        /// </summary>
        public int TotalTurns { get; private set; }

        /// <summary>
        ///     Advances the linear progression of time in the simulation, attempting to move the vehicle forward if it has the
        ///     capacity or want to do so in this turn.
        /// </summary>
        public void TakeTurn()
        {
            TotalTurns++;
            Time.TickTime();
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
            tui.Append($"[ {TickPhase} ] - ");

            // Keeps track of active mode name and active mode current state name for debugging purposes.
            tui.Append(ActiveMode?.CurrentState != null
                ? $"Mode({Modes.Count}): {ActiveMode}({ActiveMode.CurrentState}) - "
                : $"Mode({Modes.Count}): {ActiveMode}(NO STATE) - ");

            // Total number of turns that have passed in the simulation.
            tui.Append($"Turns: {TotalTurns.ToString("D4")}{Environment.NewLine}");

            // Prints game mode specific text and options. This typically is menus from commands, or states showing some information.
            tui.Append($"{base.OnTickTUI()}{Environment.NewLine}");

            // Only print and accept user input if there is a game mode and menu system to support it.
            if (AcceptingInput)
            {
                // Allow user to see their input from buffer.
                tui.Append($"What is your choice? {InputBuffer}");
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

            Instance = new GameSimApp();
        }

        protected override void OnDestroy()
        {
            // Unhook delegates from linear time simulation.
            if (Time != null)
            {
                Time.DayEndEvent -= TimeSimulation_DayEndEvent;
            }

            // Unhook director for random events.
            if (Director != null)
            {
                Director.EventAdded -= OnDirectorAddEvent;
            }

            // Destroy all instances.
            ScoreTopTen = null;
            Time = null;
            Climate = null;
            Director = null;
            Trail = null;
            TotalTurns = 0;
            Vehicle = null;
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
            Time = new TimeSim(1848, Months.March, 1);
            Time.DayEndEvent += TimeSimulation_DayEndEvent;

            // Scoring tracker and tabulator for end game results from current simulation state.
            ScoreTopTen = new List<Highscore>(ScoreRegistry.TopTenDefaults);
            // TODO: Load custom list from JSON with user high scores altered from defaults.

            // Director event manager, and his delegate.
            Director = new EventSim();
            Director.EventAdded += OnDirectorAddEvent;

            // Environment, weather, conditions, climate, tail, stats.
            Climate = new ClimateSim(ClimateClassification.Moderate);
            Trail = new TrailSim(TrailRegistry.OregonTrail());
            TotalTurns = 0;

            // Vehicle information and events for changing face and rations.
            Vehicle = new Vehicle();

            // Attach traveling mode since that is the default and bottom most game mode.
            AddMode(ModeType.Travel);

            // Add the new game configuration screen that asks for names, profession, and lets user buy initial items.
            AddMode(ModeType.NewGame);
        }

        /// <summary>
        ///     Fired when one of the sub-routines in the simulation determines that a random event should occur to the player.
        ///     Once this has been processed and created this event will get fired and the data passed into it for decision making
        ///     at this point in the simulation.
        /// </summary>
        /// <remarks>
        ///     All events will want to affect the vehicle, players inside it, or the inventory items they hold. All events will
        ///     want to print a message telling the user what happened. Some events will want to attach a new game mode and then
        ///     deal with the processing after it has been removed.
        /// </remarks>
        /// <param name="whatHappened">Event with data describing what is happening to the player as they travel down the trail.</param>
        private void OnDirectorAddEvent(IEventItem whatHappened)
        {
            // TODO: Pause the simulation.

            // TODO: Attach a game mode or alter the state of travel mode to show message about event.


            // Makes the event do whatever it is going to do.
            // NOTE: Data will very likely change or party members die after this is run...
            whatHappened.Execute();
        }

        /// <summary>
        ///     Change to new view mode when told that internal logic wants to display view options to player for a specific set of
        ///     data in the simulation.
        /// </summary>
        /// <param name="modeType">Enumeration of the game mode that requested to be attached.</param>
        /// <returns>New game mode instance based on the mode input parameter.</returns>
        protected override IMode OnModeChange(ModeType modeType)
        {
            switch (modeType)
            {
                case ModeType.Travel:
                    return new TravelMode();
                case ModeType.ForkInRoad:
                    return new ForkInRoadMode();
                case ModeType.Hunt:
                    return new HuntingMode();
                case ModeType.NewGame:
                    return new NewGameMode();
                case ModeType.RiverCrossing:
                    return new RiverCrossingMode();
                case ModeType.Store:
                    return new StoreMode();
                case ModeType.InitialPurchases:
                    return new StoreMode(true);
                case ModeType.Trade:
                    return new TradingMode();
                case ModeType.ManagementOptions:
                    return new OptionsMode();
                default:
                    throw new ArgumentOutOfRangeException(nameof(modeType), modeType, null);
            }
        }

        /// <summary>
        ///     References all of the default store items that any clerk will offer to sell you. This is also true for the store
        ///     purchasing mode that keeps track of purchases that need to be made.
        /// </summary>
        internal static IDictionary<SimEntity, SimItem> DefaultInventory
        {
            get
            {
                // Build up the default items every store will have, their prices increase with distance from starting point.
                var defaultStoreInventory = new Dictionary<SimEntity, SimItem>
                {
                    {SimEntity.Animal, Parts.Oxen},
                    {SimEntity.Clothes, Resources.Clothing},
                    {SimEntity.Ammo, Resources.Bullets},
                    {SimEntity.Wheel, Parts.Wheel},
                    {SimEntity.Axle, Parts.Axle},
                    {SimEntity.Tongue, Parts.Tongue},
                    {SimEntity.Food, Resources.Food},
                    {SimEntity.Aid, Resources.Aid},
                    {SimEntity.Cash, Resources.Cash}
                };
                return defaultStoreInventory;
            }
        }

        /// <summary>
        ///     Fired after each simulated day.
        /// </summary>
        /// <param name="dayCount">Total number of days in the simulation that have passed.</param>
        private void TimeSimulation_DayEndEvent(int dayCount)
        {
            // Each day we tick the weather, vehicle, and the people in it.
            Climate.TickClimate();

            // Update total distance traveled on vehicle if we have not reached the point.
            Vehicle.TickVehicle();

            // Grab the total amount of monies the player has spent on the items in their inventory.
            var cost_ammo = Vehicle.Inventory[SimEntity.Ammo].TotalValue;
            var cost_clothes = Vehicle.Inventory[SimEntity.Clothes].TotalValue;
            var cost_aid = Vehicle.Inventory[SimEntity.Aid].TotalValue;
            var start_cash = Vehicle.Inventory[SimEntity.Cash].TotalValue;

            // Move towards the next location on the trail.
            Trail.DecreaseDistanceToNextLocation();

            // Check if we have arrive at the next location.
            if (Trail.DistanceToNextLocation <= 0)
            {
                Trail.ArriveAtNextLocation();
            }
        }
    }
}