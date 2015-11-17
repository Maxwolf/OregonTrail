using System;
using System.Collections.Generic;
using System.Linq;
using TrailSimulation.Entity;

namespace TrailSimulation.Game
{
    /// <summary>
    ///     Holds all the points of interest that make up the entire trail the players vehicle will be traveling along. Keeps
    ///     track of the vehicles current position on the trail and provides helper methods to quickly access it.
    /// </summary>
    public sealed class TrailMod
    {
        /// <summary>
        ///     Delegate that passes along the next point of interest that was reached to the event and any subscribers to it.
        /// </summary>
        /// <param name="nextPoint">Next point of interest that will be attached to game simulation.</param>
        public delegate void PointOfInterestReached(Location nextPoint);

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailEntities.Trail" /> class.
        /// </summary>
        /// <param name="trail">Collection of points of interest which make up the trail the player is going to travel.</param>
        public TrailMod(IEnumerable<Location> trail)
        {
            // Builds the trail passed on parameter, sets location to negative one for startup.
            Locations = new List<Location>(trail);

            // Startup location on the trail and distance to next point so it triggers immediately when we tick the first day.
            LocationIndex = -1;
            DistanceToNextLocation = 0;
        }

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
        ///     Fired by the simulation when it would like to trigger advancement to the next location, doesn't matter when this is
        ///     called could be right in the middle a trail midway between two points and it would still forcefully place the
        ///     vehicle and players at the next location on the trail.
        /// </summary>
        public void ArriveAtNextLocation()
        {
            // Get the current and next locations.
            var currentPoint = GetCurrentLocation();

            // Check if we need to keep going or if we have reached the end of the trail.
            if (LocationIndex <= Locations.Count)
            {
                // Setup next travel distance requirement.
                DistanceToNextLocation = CalculateNextPointDistance();

                // Called when we decide to continue on the trail from a location on it.
                LocationIndex++;

                // Fire method to do some work and attach game modes based on this.
                OnReachedPointOfInterest(currentPoint);
            }
            else if (GameSimulationApp.Instance.Vehicle.Odometer >= GameSimulationApp.TRAIL_LENGTH)
            {
                // Check for end of game in miles.
                GameSimulationApp.Instance.WindowManager.AddMode(GameMode.EndGame);
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
            // Get the total amount of monies the player has spent on animals to pull their vehicle.
            var cost_animals = GameSimulationApp.Instance.Vehicle.Inventory[SimEntity.Animal].TotalValue;

            // Variables that will hold the distance we should travel in the next day.
            var total_miles = GameSimulationApp.Instance.Vehicle.Mileage +
                              GameSimulationApp.Instance.Trail.DistanceToNextLocation + (cost_animals - 110)/2.5 +
                              10*GameSimulationApp.Instance.Random.NextDouble();

            return (int) Math.Abs(total_miles);
        }

        /// <summary>
        ///     Advances the vehicle to the next point of interest on the path. Returns TRUE if we have arrived at the next point,
        ///     FALSE if this method should be called more to advance vehicle down the trail.
        /// </summary>
        public void DecreaseDistanceToNextLocation()
        {
            // Move us towards the next point.
            DistanceToNextLocation -= GameSimulationApp.Instance.Vehicle.Mileage;

            // If distance to next area reaches zero or below we will arrive at next location.
            if (DistanceToNextLocation > 0)
                return;

            DistanceToNextLocation = 0;
        }

        /// <summary>
        ///     Locates the next point of interest if it exists in the list, if this method returns NULL then that means the next
        ///     point of interest is the end of the game when the distance to point reaches zero.
        /// </summary>
        public Location GetNextLocation()
        {
            // Build next point index from current point, even if startup value with -1 we add 1 so will always get first point.
            var nextPointIndex = LocationIndex + 1;

            // Check if the next point is greater than point count, then get next point of interest if within bounds.
            return nextPointIndex > Locations.Count ? null : Locations.ElementAt(nextPointIndex);
        }

        /// <summary>
        ///     Returns the current point of interest the players vehicle is on.
        /// </summary>
        public Location GetCurrentLocation()
        {
            return LocationIndex <= -1
                ? Locations.First()
                : Locations.ElementAt(LocationIndex);
        }

        /// <summary>
        ///     Event that will be fired when the next point of interest has been reached on the trail.
        /// </summary>
        public event PointOfInterestReached OnReachPointOfInterest;

        /// <summary>
        ///     Determines if the current point of interest is indeed the first one of the game, makes it easier for game modes and
        ///     states to check this for doing special actions on the first move.
        /// </summary>
        /// <returns>TRUE if first point on trail, FALSE if not.</returns>
        public bool IsFirstLocation()
        {
            return LocationIndex <= 0 &&
                   GameSimulationApp.Instance.TotalTurns <= 0 &&
                   GameSimulationApp.Instance.WindowManager.RunCount[GameMode.Store] <= 1;
        }

        /// <summary>
        ///     Fired when the players vehicle reaches the next point of interest on the trail.
        /// </summary>
        /// <param name="nextPoint">
        ///     Next point of interest that was peeked in the trail list. If this is null then next point is
        ///     end of game.
        /// </param>
        private void OnReachedPointOfInterest(Location nextPoint)
        {
            // Set the flag indicating the player has been here now.
            nextPoint.SetVisited();

            // Attach some game mode based on the relevance of the next point type.
            GameSimulationApp.Instance.WindowManager.AddMode(nextPoint.GameMode);

            // Fire event here for API subscribers to know point was reached. 
            OnReachPointOfInterest?.Invoke(nextPoint);
        }

        /// <summary>
        ///     Determines if the player is currently midway between two location points on the trail.
        /// </summary>
        public bool ReachedNextPoint()
        {
            return DistanceToNextLocation <= 0;
        }
    }
}