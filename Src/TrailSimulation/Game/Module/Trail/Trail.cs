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
            _locations = new List<Location>(locations);
            TrailLength = trailLength;
            FlagLastLocation();
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
        public int TrailLength { get; private set; }

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
            FlagLastLocation();
        }
    }
}