using System;
using System.Collections.Generic;
using TrailSimulation.Core;
using TrailSimulation.Entity;

namespace TrailSimulation.Game
{
    /// <summary>
    ///     Holds all the points of interest that make up the entire trail the players vehicle will be traveling along. Keeps
    ///     track of the vehicles current position on the trail and provides helper methods to quickly access it.
    /// </summary>
    public sealed class TrailModuleProduct : IModule
    {
        public TrailModuleProduct()
        {
            // Builds the trail passed on parameter, sets location to negative one for startup.
            Locations = new List<Location>(TrailRegistry.OregonTrail());

            // Startup location on the trail and distance to next point so it triggers immediately when we tick the first day.
            LocationIndex = 0;
            DistanceToNextLocation = 0;
        }

        /// <summary>
        ///     Distance in miles the player needs to travel before they are considered arrived at next point.
        /// </summary>
        public int DistanceToNextLocation { get; private set; }

        /// <summary>
        ///     Current location of the players vehicle as index of points of interest list.
        /// </summary>
        public int LocationIndex { get; private set; }

        /// <summary>
        ///     List of all of the points of interest that make up the entire trail.
        /// </summary>
        public List<Location> Locations { get; }

        /// <summary>
        ///     Determines if the player is currently midway between two location points on the trail.
        /// </summary>
        public bool ReachedNextPoint
        {
            get
            {
                return DistanceToNextLocation <= 0 &&
                       !CurrentLocation.HasVisited;
            }
        }

        /// <summary>
        ///     Returns the current point of interest the players vehicle is on. Lazy initialization of path when accessed by first
        ///     attached mode getting current point.
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
                return nextPointIndex > Locations.Count ? null : Locations[nextPointIndex];
            }
        }

        /// <summary>
        ///     Determines if the current point of interest is indeed the first one of the game, makes it easier for game modes and
        ///     states to check this for doing special actions on the first move.
        /// </summary>
        /// <returns>TRUE if first point on trail, FALSE if not.</returns>
        public bool IsFirstLocation
        {
            get
            {
                return LocationIndex <= 0 &&
                       GameSimulationApp.Instance.TotalTurns <= 0 &&
                       GameSimulationApp.Instance.ModeManager.RunCount[Mode.Store] <= 1;
            }
        }

        /// <summary>
        ///     Fired when the simulation is closing and needs to clear out any data structures that it created so the program can
        ///     exit cleanly.
        /// </summary>
        public void Destroy()
        {
            DistanceToNextLocation = 0;
            LocationIndex = 0;
            Locations.Clear();
        }

        /// <summary>
        ///     Advances the vehicle to the next point of interest on the path. Returns TRUE if we have arrived at the next point,
        ///     FALSE if this method should be called more to advance vehicle down the trail.
        /// </summary>
        public void Tick()
        {
            // Simulate the mileage being done.
            var simulatedDistanceChange = DistanceToNextLocation - GameSimulationApp.Instance.Vehicle.Mileage;

            // If distance is zero we have arrived at the next location!
            if (simulatedDistanceChange <= 0)
            {
                ArriveAtNextLocation();
                simulatedDistanceChange = 0;
            }

            // Move us towards the next point if not zero.
            DistanceToNextLocation = simulatedDistanceChange;
        }

        /// <summary>
        ///     Fired by the simulation when it would like to trigger advancement to the next location, doesn't matter when this is
        ///     called could be right in the middle a trail midway between two points and it would still forcefully place the
        ///     vehicle and players at the next location on the trail.
        /// </summary>
        public void ArriveAtNextLocation()
        {
            // Check if we need to keep going or if we have reached the end of the trail.
            if (LocationIndex <= Locations.Count)
            {
                // Setup next travel distance requirement.
                DistanceToNextLocation = CalculateNextPointDistance();

                // Called when we decide to continue on the trail from a location on it.
                LocationIndex++;

                // Set visited flag for location, attach mode it requires, and fire event for subscribers.
                CurrentLocation.SetVisited();
                GameSimulationApp.Instance.ModeManager.AddMode(CurrentLocation.Mode);
            }
            else if (GameSimulationApp.Instance.Vehicle.Odometer >= GameSimulationApp.TRAIL_LENGTH)
            {
                // Check for end of game in miles.
                GameSimulationApp.Instance.ModeManager.AddMode(Mode.EndGame);
            }
        }

        /// <summary>
        ///     In general, you will travel 200 miles plus some additional distance which depends upon the quality of your team of
        ///     oxen. This mileage figure is an ideal, assuming nothing goes wrong. If you run into problems, mileage is subtracted
        ///     from this ideal figure; the revised total is printed at the start of the next trip segment.
        /// </summary>
        /// <returns>The expected mileage over the next two week segment.</returns>
        private static int CalculateNextPointDistance()
        {
            // GetModule the total amount of monies the player has spent on animals to pull their vehicle.
            var cost_animals = GameSimulationApp.Instance.Vehicle.Inventory[SimEntity.Animal].TotalValue;

            // Variables that will hold the distance we should travel in the next day.
            var total_miles = GameSimulationApp.Instance.Vehicle.Mileage +
                              GameSimulationApp.Instance.Trail.DistanceToNextLocation + (cost_animals - 110)/2.5 +
                              10*GameSimulationApp.Instance.Random.NextDouble();

            return (int) Math.Abs(total_miles);
        }
    }
}