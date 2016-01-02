// Created by Ron 'Maxwolf' McDowell (ron.mcdowell@gmail.com) 
// Timestamp 01/01/2016@7:40 PM

namespace TrailSimulation
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using WolfCurses;

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
        public const int MAXPLAYERS = 4;

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
        ///     Scoring tracker and tabulator for end game results from current simulation state.
        /// </summary>
        public ScoringModule Scoring { get; private set; }

        /// <summary>
        ///     References all of the tombstones that this trail might have associated with it, also allows adding of tombstones to
        ///     this trail so other players can encounter them in the future.
        /// </summary>
        public TombstoneModule Tombstone { get; private set; }

        /// <summary>
        ///     Determines what windows the simulation will be capable of using and creating using the window managers factory.
        /// </summary>
        public override IEnumerable<Type> AllowedWindows
        {
            get
            {
                var windowList = new List<Type>
                {
                    typeof (Travel),
                    typeof (MainMenu),
                    typeof (RandomEvent),
                    typeof (Graveyard),
                    typeof (GameOver)
                };

                return windowList;
            }
        }

        /// <summary>
        ///     Advances the linear progression of time in the simulation, attempting to move the vehicle forward if it has the
        ///     capacity or want to do so in this turn.
        /// </summary>
        /// <param name="skipDay">Determines if the simulation should tick the day or skip it and not process time and only events.</param>
        public void TakeTurn(bool skipDay)
        {
            // Advance the turn counter if we are not skipping days.
            if (!skipDay)
                TotalTurns++;

            // Let the modules of the game simulation decide how they want to deal with skip time turn.
            Time.TickTime(skipDay);
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
            Vehicle.ResetVehicle(startingInfo.StartingMonies);

            // Add all the player data we collected from attached game Windows states.
            var crewNumber = 1;
            foreach (var name in startingInfo.PlayerNames)
            {
                // First name in list is always the leader.
                var personLeader = startingInfo.PlayerNames.IndexOf(name) == 0 && crewNumber == 1;
                Vehicle.AddPerson(new Person(startingInfo.PlayerProfession, name, personLeader));
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
            Instance.OnPostCreate();
        }

        /// <summary>
        ///     Fired after the simulation instance has been created, allowing us to call it using the instance of the simulation
        ///     from static method.
        /// </summary>
        private void OnPostCreate()
        {
            Scoring = new ScoringModule();

            // Allows for other players to see deaths of previous players on the trail.
            Tombstone = new TombstoneModule();
        }

        /// <summary>
        ///     Called when simulation is about to destroy itself, but right before it actually does it.
        /// </summary>
        protected override void OnPreDestroy()
        {
            // Notify modules of impending doom allowing them to save data.
            Scoring.Destroy();
            Tombstone.Destroy();
            Time.Destroy();
            EventDirector.Destroy();
            Trail.Destroy();

            // Null the destroyed instances.
            Scoring = null;
            Tombstone = null;
            Time = null;
            EventDirector = null;
            Trail = null;
            TotalTurns = 0;
            Vehicle = null;

            // Destroys game simulation instance.
            Instance = null;
        }

        /// <summary>
        ///     Called by the text user interface scene graph renderer before it asks the active window to render itself out for
        ///     display.
        /// </summary>
        public override string OnPreRender()
        {
            // Total number of turns that have passed in the simulation.
            var tui = new StringBuilder();
            tui.AppendLine($"Turns: {TotalTurns.ToString("D4")}");

            // Vehicle and location status.
            tui.AppendLine($"Vehicle: {Vehicle?.Status} - Location:{Trail?.CurrentLocation?.Status}");
            return tui.ToString();
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
        public override void Restart()
        {
            // Reset turn counter back to zero.
            TotalTurns = 0;

            // Linear time simulation (should tick first).
            Time = new TimeModule();

            // Vehicle, weather, conditions, climate, tail, stats, event director, etc.
            EventDirector = new EventDirectorModule();
            Trail = new TrailModule();
            Vehicle = new Vehicle();

            // Resets the window manager in the base simulation.
            base.Restart();

            // Attach traveling Windows since that is the default and bottom most game Windows.
            WindowManager.Add(typeof (Travel));

            // Add the new game configuration screen that asks for names, profession, and lets user buy initial items.
            WindowManager.Add(typeof (MainMenu));
        }
    }
}