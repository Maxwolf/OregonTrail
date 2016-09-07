// Created by Ron 'Maxwolf' McDowell (ron.mcdowell@gmail.com) 
// Timestamp 01/03/2016@1:50 AM

using System.Collections.ObjectModel;
using OregonTrailDotNet.TrailSimulation.Entity.Location;
using OregonTrailDotNet.TrailSimulation.Entity.Vehicle;
using OregonTrailDotNet.TrailSimulation.Window.Travel;

namespace OregonTrailDotNet.TrailSimulation.Module.Trail
{
    /// <summary>
    ///     Holds all the points of interest that make up the entire trail the players vehicle will be traveling along. Keeps
    ///     track of the vehicles current position on the trail and provides helper methods to quickly access it.
    /// </summary>
    public sealed class TrailModule : WolfCurses.Module.Module
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="TrailModule" /> class.
        /// </summary>
        public TrailModule()
        {
            // Load a trail from file or prefab.
            Trail = TrailRegistry.OregonTrail;

            // Startup location on the trail and distance to next point so it triggers immediately when we tick the first day.
            LocationIndex = 0;
            DistanceToNextLocation = 0;
        }

        /// <summary>
        ///     Reference to the loaded trail which the simulation and trail module are using to let the player iterate over them.
        /// </summary>
        private Trail Trail { get; set; }

        /// <summary>
        ///     Distance in miles the player needs to travel before they are considered arrived at next point.
        /// </summary>
        public int DistanceToNextLocation { get; private set; }

        /// <summary>
        ///     Current location of the players vehicle as index of points of interest list.
        /// </summary>
        public int LocationIndex { get; private set; }

        /// <summary>
        ///     Reference to all locations in this trail, indexed in the order they should be visited by vehicle.
        /// </summary>
        public ReadOnlyCollection<Location> Locations
        {
            get { return Trail.Locations; }
        }

        /// <summary>
        ///     Total length of the entire trail and locations added together. Simulation will decide distance between points
        ///     randomly but keep it within this range.
        /// </summary>
        public int Length
        {
            get { return Trail.Length; }
        }

        /// <summary>
        ///     Returns the current point of interest the players vehicle is on. Lazy initialization of path when accessed by first
        ///     attached Windows getting current point.
        /// </summary>
        public Location CurrentLocation
        {
            get { return Locations[LocationIndex]; }
        }

        /// <summary>
        ///     Locates the next point of interest if it exists in the list, if this method returns NULL then that means the next
        ///     point of interest is the end of the game when the distance to point reaches zero.
        /// </summary>
        public Location NextLocation
        {
            get
            {
                // Build next point index from current position in collection.
                var nextPointIndex = LocationIndex + 1;

                // Check if the next point is greater than point count, then get next point of interest if within bounds.
                return nextPointIndex >= Locations.Count ? null : Locations[nextPointIndex];
            }
        }

        /// <summary>
        ///     Determines if the current point of interest is indeed the first one of the game, makes it easier for game modes and
        ///     states to check this for doing special actions on the first move.
        /// </summary>
        /// <returns>TRUE if first point on trail, FALSE if not.</returns>
        public bool IsFirstLocation
        {
            get { return LocationIndex <= 0; }
        }

        /// <summary>
        ///     Fired when the simulation is closing and needs to clear out any data structures that it created so the program can
        ///     exit cleanly.
        /// </summary>
        public override void Destroy()
        {
            DistanceToNextLocation = 0;
            LocationIndex = 0;
            Trail = null;
        }

        /// <summary>
        ///     Called when the simulation is ticked by underlying operating system, game engine, or potato. Each of these system
        ///     ticks is called at unpredictable rates, however if not a system tick that means the simulation has processed enough
        ///     of them to fire off event for fixed interval that is set in the core simulation by constant in milliseconds.
        /// </summary>
        /// <remarks>Default is one second or 1000ms.</remarks>
        /// <param name="systemTick">
        ///     TRUE if ticked unpredictably by underlying operating system, game engine, or potato. FALSE if
        ///     pulsed by game simulation at fixed interval.
        /// </param>
        /// <param name="skipDay">
        ///     Determines if this tick skipped a day of the simulation and force ticked anyway. This is used for
        ///     special events like river crossings, hunting, trading, etc.
        /// </param>
        public override void OnTick(bool systemTick, bool skipDay = false)
        {
            // Skip system ticks.
            if (systemTick)
                return;

            // Grab the current vehicle from the game simulation.
            var vehicle = GameSimulationApp.Instance.Vehicle;

            // Tick the current location, typically this will randomize the possible trades, weather, and advice.
            CurrentLocation?.OnTick(false, skipDay);

            // Update total distance traveled on vehicle if we have not reached the point.
            vehicle.OnTick(false, skipDay);

            // No advancing down the trail when vehicle is parked or force ticked by skipping a day.
            if (vehicle.Status != VehicleStatus.Moving || skipDay)
                return;

            // Check if the player is still working with the location they are currently arrived at.
            if (CurrentLocation?.Status == LocationStatus.Arrived &&
                DistanceToNextLocation <= 0)
                return;

            // Move us towards the next point if not zero.
            DistanceToNextLocation -= vehicle.Mileage;

            // If distance is zero we have arrived at the next location!
            if (DistanceToNextLocation >= 0)
                return;

            // Distance to next point was less than or equal to zero, arrive at next location after setting distance to zero.
            DistanceToNextLocation = 0;
            ArriveAtNextLocation();
        }

        /// <summary>
        ///     Fired by the simulation when it would like to trigger advancement to the next location, doesn't matter when this is
        ///     called could be right in the middle a trail midway between two points and it would still forcefully place the
        ///     vehicle and players at the next location on the trail.
        /// </summary>
        public void ArriveAtNextLocation()
        {
            // Check if we need to keep going or if we have reached the end of the trail.
            if (LocationIndex > Locations.Count)
                return;

            // Setup next travel distance requirement.
            DistanceToNextLocation = CurrentLocation.TotalDistance;

            // Skip incrementing to next location on first turn, we use first turn to setup game world and player position in it.
            if (GameSimulationApp.Instance.TotalTurns > 0)
                LocationIndex++;

            // Set visited flag for location, park the vehicle, and attach Windows the location requires.
            CurrentLocation.Status = LocationStatus.Arrived;

            // Check for end of game if we are at the end of the trail.
            GameSimulationApp.Instance.WindowManager.Add(typeof (Travel));
        }

        /// <summary>Forcefully inserts skip location into location list after current location.</summary>
        /// <param name="skipChoice">Location that the trail module will point to after current location.</param>
        public void InsertLocation(Location skipChoice)
        {
            Trail.InsertLocation(LocationIndex + 1, skipChoice);
        }
    }
}