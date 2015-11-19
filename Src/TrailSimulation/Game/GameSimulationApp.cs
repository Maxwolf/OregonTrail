using System;
using System.Collections.Generic;
using TrailSimulation.Core;
using TrailSimulation.Entity;

namespace TrailSimulation.Game
{
    /// <summary>
    ///     Primary game simulation singleton. Purpose of this class is to control game specific modules that are independent
    ///     of the simulations ability to manage itself, process ticks, and input.
    /// </summary>
    public sealed class GameSimulationApp : SimulationApp
    {
        /// <summary>
        ///     Fired when the simulation is closing, event will trigger before data structures are destroyed offering chance to
        ///     save any data they care about.
        /// </summary>
        public delegate void EndGame();

        /// <summary>
        ///     Fired when the first turn is ticked from zero to one.
        /// </summary>
        public delegate void NewGame();

        /// <summary>
        ///     Defines the limit on the number of players for the vehicle that will be allowed. This also determines how many
        ///     names are asked for in new game mode.
        /// </summary>
        public const int MAX_PLAYERS = 4;

        /// <summary>
        ///     Defines the maximum number of miles the vehicle is allowed to travel before the game is considered forcefully over.
        /// </summary>
        public const int TRAIL_LENGTH = 2040;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailGame.GameSimulationApp" /> class.
        /// </summary>
        private GameSimulationApp()
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
        public TrailModule Trail { get; private set; }

        /// <summary>
        ///     Singleton instance for the entire game simulation, does not block the calling thread though only listens for
        ///     commands.
        /// </summary>
        public static GameSimulationApp Instance { get; private set; }

        /// <summary>
        ///     Manages time in a linear since from the provided ticks in base simulation class. Handles days, months, and years.
        /// </summary>
        public TimeModule Time { get; private set; }

        /// <summary>
        ///     Manages weather, temperature, humidity, and current grazing level for living animals.
        /// </summary>
        public ClimateModule Climate { get; private set; }

        /// <summary>
        ///     Keeps track of the total number of points the player has earned through the course of the game.
        /// </summary>
        public List<Highscore> ScoreTopTen { get; private set; }

        /// <summary>
        ///     Base interface for the event manager, it is ticked as a sub-system of the primary game simulation and can affect
        ///     game modes, people, and vehicles.
        /// </summary>
        public EventDirectorModule EventDirector { get; private set; }

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
        ///     References all of the default store items that any clerk will offer to sell you. This is also true for the store
        ///     purchasing mode that keeps track of purchases that need to be made.
        /// </summary>
        internal static IDictionary<SimEntity, SimItem> DefaultInventory
        {
            get
            {
                // Build up the default items every store will have, their prices increase with distance from starting point.
                var defaultInventory = new Dictionary<SimEntity, SimItem>
                {
                    {SimEntity.Animal, Parts.Oxen},
                    {SimEntity.Clothes, Resources.Clothing},
                    {SimEntity.Ammo, Resources.Bullets},
                    {SimEntity.Wheel, Parts.Wheel},
                    {SimEntity.Axle, Parts.Axle},
                    {SimEntity.Tongue, Parts.Tongue},
                    {SimEntity.Food, Resources.Food},
                    {SimEntity.Cash, Resources.Cash}
                };
                return defaultInventory;
            }
        }

        /// <summary>
        ///     Fired when the first turn is ticked from zero to one.
        /// </summary>
        public event NewGame NewgameEvent;

        /// <summary>
        ///     Fired when the simulation is closing, event will trigger before data structures are destroyed offering chance to
        ///     save any data they care about.
        /// </summary>
        public event EndGame EndgameEvent;


        /// <summary>
        ///     Advances the linear progression of time in the simulation, attempting to move the vehicle forward if it has the
        ///     capacity or want to do so in this turn.
        /// </summary>
        public void TakeTurn()
        {
            // Advance the turn counter.
            TotalTurns++;
            Time.TickTime();
        }

        /// <summary>
        ///     Attaches the traveling mode and removes the new game mode if it exists, this begins the simulation down the trail
        ///     path and all the points of interest on it.
        /// </summary>
        /// <param name="startingInfo">User data object that was passed around the new game mode and populated by user selections.</param>
        internal void SetData(MainMenuInfo startingInfo)
        {
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
        ///     Creates new instance of game simulation. Complains if instance already exists.
        /// </summary>
        public static void Create()
        {
            if (Instance != null)
                throw new InvalidOperationException(
                    "Unable to create new instance of game simulation since it already exists!");

            Instance = new GameSimulationApp();
        }

        /// <summary>
        ///     Called when simulation is about to destroy itself, but right before it actually does it.
        /// </summary>
        protected override void OnBeforeDestroy()
        {
            // Fire event that lets any other data structures know the simulation has come to an end.
            EndgameEvent?.Invoke();

            // Unhook delegates from linear time simulation.
            if (Time != null)
                Time.DayEndEvent -= TimeSimulation_DayEndEvent;

            // Destroy all instances.
            ScoreTopTen = null;
            Time = null;
            Climate = null;
            EventDirector = null;
            Trail = null;
            TotalTurns = 0;
            Vehicle = null;
            Instance = null;
        }

        /// <summary>
        ///     Fired when the simulation is loaded and makes it very first tick using the internal timer mechanism keeping track
        ///     of ticks to keep track of seconds.
        /// </summary>
        protected override void OnFirstTick()
        {
            // Linear time simulation with ticks.
            Time = new TimeModule(1848, Months.March, 1);
            Time.DayEndEvent += TimeSimulation_DayEndEvent;

            // Scoring tracker and tabulator for end game results from current simulation state.
            ScoreTopTen = new List<Highscore>(ScoreRegistry.TopTenDefaults);
            // TODO: Load custom list from JSON with user high scores altered from defaults.

            // Environment, weather, conditions, climate, tail, stats, event director, etc.
            EventDirector = new EventDirectorModule();
            Climate = new ClimateModule(ClimateClassification.Moderate);
            Trail = new TrailModule(TrailRegistry.OregonTrail());
            Vehicle = new Vehicle();
            TotalTurns = 0;

            // Attach traveling mode since that is the default and bottom most game mode.
            WindowManager.AddMode(GameMode.Travel);

            // Add the new game configuration screen that asks for names, profession, and lets user buy initial items.
            WindowManager.AddMode(GameMode.MainMenu);
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
            var start_cash = Vehicle.Inventory[SimEntity.Cash].TotalValue;

            // Move towards the next location on the trail.
            Trail.DecreaseDistanceToNextLocation();
        }
    }
}