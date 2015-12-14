using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using TrailSimulation.Entity;

namespace TrailSimulation.Game
{
    /// <summary>
    ///     Wrapper object for the trail, defines all the locations and total trail length. The purpose of this class is to
    ///     support serialization and abstraction of the trail creation process so it can be loaded from files or downloaded as
    ///     JSON from a server.
    /// </summary>
    public sealed class Trail
    {
        /// <summary>
        ///     Reference to all locations in this trail, indexed in the order they should be visited by vehicle.
        /// </summary>
        private List<Location> _locations;

        /// <summary>
        ///     Creates a new trail the simulation can let a vehicle travel on. Requires a list of locations and total length in
        ///     miles.
        /// </summary>
        /// <param name="locations">List of locations indexed in the order they should be visited in simulation.</param>
        /// <param name="trailLength">Total length of the entire trail, simulation will decide length between locations randomly.</param>
        public Trail(IEnumerable<Location> locations, int trailLength)
        {
            // Check if trail given to us is valid.
            if (locations == null)
                throw new ArgumentNullException(nameof(locations), "List of locations for trail was null!");

            // Create a new empty trail dictionary.
            _locations = new List<Location>(locations);

            // Check the trail has minimum of two locations for starting end ending the simulation.
            if (_locations.Count <= 1)
                throw new ArgumentException("List of locations count not greater than or equal to two!");

            // Setup the max length for the trail which we get from 
            TrailLength = trailLength;

            // Marks the last location in the trail, if another location is inserted this will be re-calculated.
            FlagLastLocation();

            // Generate distances blocked out by total trail length defined in loaded trail file.
            GenerateDistanceBetweenLocations();
        }

        /// <summary>
        ///     Reference to all locations in this trail, indexed in the order they should be visited by vehicle.
        /// </summary>
        public ReadOnlyCollection<Location> Locations
        {
            get { return _locations.AsReadOnly(); }
        }

        /// <summary>
        ///     Total length of the entire trail and locations added together. Simulation will decide distance between points
        ///     randomly but keep it within this range.
        /// </summary>
        public int TrailLength { get; }

        /// <summary>
        ///     Since the trail does not directly define the total distance between all the points and only the ceiling for the
        ///     entire trail it is necessary to create a chunk of distance to needs to be traveled from this total mileage.
        /// </summary>
        private void GenerateDistanceBetweenLocations()
        {
            // Total length trail should be, will be subtracted from until zero.
            var lengthRemaining = TrailLength;

            // Complain if trail length is zero.
            if (TrailLength <= 0)
                throw new InvalidOperationException("Trail total length cannot be less than or equal to zero!");

            // Grab all of the locations from the trail, including the skip choices from forks in the road.
            var locationReferences = new List<Location>();
            foreach (var location in _locations)
            {
                // Add the location itself.
                locationReferences.Add(location);

                // Check if the location is a fork in the road and has skip choices.
                if (location.Category == LocationCategory.ForkInRoad && 
                    location.SkipChoices != null)
                    locationReferences.AddRange(location.SkipChoices);
            }

            // Figure out minimum trail length by dividing trail length by location count.
            var minimumTrailLength = TrailLength/locationReferences.Count;

            // Complain if minimum trail length is less than or equal to zero.
            if (minimumTrailLength <= 0)
                throw new ArgumentException("Minimum trail length cannot be less than or equal to zero!" , nameof(minimumTrailLength));

            // Loop through loaded trail and add each location while calculating distances for each block.
            foreach (var location in locationReferences)
            {
                // Randomly generates the block segment for next trail location.
                var blockLength = GameSimulationApp.Instance.Random.Next(minimumTrailLength, lengthRemaining);

                // Simulate removal of the randomly generated length from length remaining.
                var simulatedLengthRemaining = lengthRemaining - blockLength;

                // Check simulated removal is greater than minimum trail length.
                if (simulatedLengthRemaining < minimumTrailLength)
                    simulatedLengthRemaining = minimumTrailLength;

                // Add block distance we generated to the location.
                location.TotalDistance = simulatedLengthRemaining;

                // Update length remaining for next location.
                lengthRemaining = simulatedLengthRemaining;
            }
        }

        /// <summary>
        ///     Mark the last location being as such, will throw exception is locations list is empty as it should.
        /// </summary>
        private void FlagLastLocation()
        {
            // Remove last flag from all locations before setting it again.
            foreach (var location in _locations)
                location.IsLast = false;

            // Set the last location flag on the last location in the list.
            var lastLocation = _locations.Last();
            lastLocation.IsLast = true;
        }

        /// <summary>
        ///     Forcefully inserts skip location into location list after current location.
        /// </summary>
        /// <param name="skipIndex">Index in the location list we will add the new one.</param>
        /// <param name="skipLocation">Location that the trail module will point to after current location.</param>
        public void InsertLocation(int skipIndex, Location skipLocation)
        {
            _locations.Insert(skipIndex, skipLocation);

            // Marks the last location in the trail, if another location is inserted this will be re-calculated.
            FlagLastLocation();
        }
    }
}