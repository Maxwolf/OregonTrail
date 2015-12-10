using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using TrailSimulation.Core;
using TrailSimulation.Entity;

namespace TrailSimulation.Game
{
    /// <summary>
    ///     Primary game simulation singleton. Purpose of this class is to control game specific modules that are independent
    ///     of the simulations ability to manage itself, process ticks, and input.
    /// </summary>
    public class GameSimulationApp : SimulationApp
    {
        /// <summary>
        ///     Defines the limit on the number of players for the vehicle that will be allowed. This also determines how many
        ///     names are asked for in new game Windows.
        /// </summary>
        public const int MAX_PLAYERS = 4;

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
        /// </summary>
        [SuppressMessage("ReSharper", "MemberCanBeMadeStatic.Local")]
        public GameStatus Status
        {
            get
            {
                // Check if the player made it all the way to the end of the trail.
                if (Trail.LocationIndex >= Trail.Locations.Count)
                    return GameStatus.Win;



                // Determine if everybody is dead, otherwise let the game continue.
                if (Vehicle.Passengers.All(p => p.IsDead))
                    return GameStatus.Fail;

                // Default response is to let the simulation keep running.
                return GameStatus.Running;
            }
        }

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
        ///     Attaches the traveling Windows and removes the new game Windows if it exists, this begins the simulation down the
        ///     trail path and all the points of interest on it.
        /// </summary>
        /// <param name="startingInfo">
        ///     User data object that was passed around the new game Windows and populated by user
        ///     selections.
        /// </param>
        internal void SetStartInfo(NewGameInfo startingInfo)
        {
            // Clear out any data amount items, monies, people that might have been in the vehicle.
            // NOTE: Sets starting monies, which is determined by player profession.
            Vehicle.ResetVehicle(startingInfo.StartingMonies);

            // Add all the player data we collected from attached game Windows states.
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
            Restart();
        }

        /// <summary>
        ///     Creates and or clears data sets required for game simulation and attaches the travel menu and the main menu to make
        ///     the program completely restarted as if fresh.
        /// </summary>
        public void Restart()
        {
            // Reset turn counter back to zero.
            TotalTurns = 0;

            // Linear time simulation with ticks.
            Time = new TimeModule();

            // Scoring tracker and tabulator for end game results from current simulation state.
            ScoreTopTen = new List<Highscore>(ScoreRegistry.TopTenDefaults);
            // TODO: Load custom list from JSON with user high scores altered from defaults.

            // Environment, weather, conditions, climate, tail, stats, event director, etc.
            EventDirector = new EventDirectorModule();
            Climate = new ClimateModule();
            Trail = new TrailModule();

            // Vehicle entity for the players to travel in along the trail.
            Vehicle = new Vehicle();

            // Resets the window manager and clears out all windows and forms from previous session.
            WindowManager.Clear();

            // Attach traveling Windows since that is the default and bottom most game Windows.
            WindowManager.Add(GameWindow.Travel);

            // Add the new game configuration screen that asks for names, profession, and lets user buy initial items.
            WindowManager.Add(GameWindow.MainMenu);
        }
    }
}