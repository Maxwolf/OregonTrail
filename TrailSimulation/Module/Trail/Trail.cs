// Created by Ron 'Maxwolf' McDowell (ron.mcdowell@gmail.com) 
// Timestamp 12/31/2015@4:38 AM

namespace TrailSimulation.Module.Trail
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using Entity.Location;
    using Entity.Location.Point;

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
        ///     Keeps track of the total length of the trail as it will be generated after recursing through every possible
        ///     location on the trail.
        /// </summary>
        private int totalTrailLength;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Trail" /> class.
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
        private int CreateRandomLength()
        {
            var generatedLength = GameSimulationApp.Instance.Random.Next(LengthMin, LengthMax);
            return generatedLength;
        }

        /// <summary>
        ///     Since the trail does not directly define the total distance between all the points and only the ceiling for the
        ///     entire trail it is necessary to create a chunk of distance to needs to be traveled from this total mileage.
        /// </summary>
        /// <returns>Total length of the entire trail which will be placed into a property that can be accessed at runtime.</returns>
        private int GenerateDistances()
        {
            // Re-calculation of total trail length begins with zero.
            totalTrailLength = 0;

            // Begins a recursive loop that goes through every location and any locations they may have inside of them.
            GenerateDistancesRecursive(_locations);

            // Returns the total length of the entire trail for record keeping purposes.
            return totalTrailLength;
        }

        /// <summary>
        ///     Grabs all of the locations from the trail, including the skip choices from forks in the road.
        /// </summary>
        /// <param name="locations">All of the locations on the trail we need to inspect.</param>
        /// <param name="locationDepth">
        ///     Level of depth we have gone in regards to locations leading to locations leading to
        ///     locations.
        /// </param>
        private void GenerateDistancesRecursive(IEnumerable<Location> locations, int locationDepth = 0)
        {
            // Loop through every location we have been given access to via parameter.
            foreach (var location in locations)
            {
                // Work on the current item we have.
                location.Depth = locationDepth;
                location.TotalDistance = CreateRandomLength();
                totalTrailLength += location.TotalDistance;

                // Check if the location is a fork in the road and has skip choices.
                if (!(location is ForkInRoad))
                    continue;

                // Cast the location as a fork in the road.
                var forkInRoad = location as ForkInRoad;

                // increment the level of indentation and call the same method for the children
                GenerateDistancesRecursive(forkInRoad.SkipChoices, locationDepth + 1);
            }
        }

        /// <summary>
        ///     Mark the last location being as such, will throw exception is locations list is empty as it should.
        /// </summary>
        private void FlagLastLocation()
        {
            // Remove last flag from all locations before setting it again.
            foreach (var location in _locations)
                location.LastLocation = false;

            // Set the last location flag on the last location in the list.
            var lastLocation = _locations.Last();
            lastLocation.LastLocation = true;
        }

        /// <summary>Forcefully inserts skip location into location list after current location.</summary>
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