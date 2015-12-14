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
        /// <param name="lengthMin">Minimum length of any given trail segment.</param>
        /// <param name="lengthMax">Maximum length of any given trail segment.</param>
        public Trail(IEnumerable<Location> locations, int lengthMin, int lengthMax)
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
            LengthMin = lengthMin;
            LengthMax = lengthMax;

            // Marks the last location in the trail, if another location is inserted this will be re-calculated.
            FlagLastLocation();

            // Generate distances blocked out by total trail length defined in loaded trail file.
            Length = GenerateDistances();
        }

        /// <summary>
        ///     Total length of the entire trail and locations added together. Simulation will decide distance between points
        ///     randomly but keep it within this range.
        /// </summary>
        public int Length { get; }

        /// <summary>
        ///     Maximum length of any given trail segment.
        /// </summary>
        private int LengthMax { get; }

        /// <summary>
        ///     Reference to all locations in this trail, indexed in the order they should be visited by vehicle.
        /// </summary>
        public ReadOnlyCollection<Location> Locations
        {
            get { return _locations.AsReadOnly(); }
        }

        /// <summary>
        ///     Minimum length of any given trail segment.
        /// </summary>
        private int LengthMin { get; }

        /// <summary>
        ///     Generate random length for tail within range, or use remaining amount when all out of space.
        /// </summary>
        /// <returns>Trail length for this location, distance vehicle will need to travel before arrival at next location.</returns>
        private int CreateRandomLength
        {
            get
            {
                var generatedLength = GameSimulationApp.Instance.Random.Next(LengthMin, LengthMax);
                return generatedLength;
            }
        }

        /// <summary>
        ///     Since the trail does not directly define the total distance between all the points and only the ceiling for the
        ///     entire trail it is necessary to create a chunk of distance to needs to be traveled from this total mileage.
        /// </summary>
        /// <returns>Total length of the entire trail which will be placed into a property that can be accessed at runtime.</returns>
        private int GenerateDistances()
        {
            // Grab all of the locations from the trail, including the skip choices from forks in the road.
            var totalTrailLength = 0;
            foreach (var location in _locations)
            {
                // Check if the location is a fork in the road and has skip choices.
                if (location.Category == LocationCategory.ForkInRoad &&
                    location.SkipChoices != null)
                {
                    foreach (var skipChoice in location.SkipChoices)
                    {
                        skipChoice.TotalDistance = CreateRandomLength;
                        totalTrailLength += skipChoice.TotalDistance;
                    }
                }

                location.TotalDistance = CreateRandomLength;
                totalTrailLength += location.TotalDistance;
            }

            // Returns the total length of the entire trail for record keeping purposes.
            return totalTrailLength;
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