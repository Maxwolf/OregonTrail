using System.Collections.Generic;
using System.Linq;
using TrailCommon;

namespace TrailEntities
{
    /// <summary>
    ///     Holds all the points of interest that make up the entire trail the players vehicle will be traveling along. Keeps
    ///     track of the vehicles current position on the trail and provides helper methods to quickly access it.
    /// </summary>
    public sealed class TrailSim
    {
        /// <summary>
        ///     Delegate that passes along the next point of interest that was reached to the event and any subscribers to it.
        /// </summary>
        /// <param name="nextPoint">Next point of interest that will be attached to game simulation.</param>
        public delegate void PointOfInterestReached(PointOfInterest nextPoint);

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailEntities.TrailSim" /> class.
        /// </summary>
        /// <param name="trail">Collection of points of interest which make up the trail the player is going to travel.</param>
        public TrailSim(IEnumerable<PointOfInterest> trail)
        {
            // Builds the trail passed on parameter, sets location to negative one for startup.
            PointsOfInterest = trail;
            VehicleLocation = -1;
            DistanceToNextPoint = 0;
        }

        /// <summary>
        ///     Reference to how many ticks are between the players vehicle and the next point of interest.
        /// </summary>
        private ulong DistanceToNextPoint { get; set; }

        /// <summary>
        ///     Current location of the players vehicle as index of points of interest list.
        /// </summary>
        private int VehicleLocation { get; set; }

        /// <summary>
        ///     List of all of the points of interest that make up the entire trail.
        /// </summary>
        private IEnumerable<PointOfInterest> PointsOfInterest { get; }

        /// <summary>
        ///     Advances the vehicle to the next point of interest on the path.
        /// </summary>
        public void ReachedPointOfInterest()
        {
            // Figure out if this is first step, or one of many after the start.
            if (VehicleLocation == -1)
            {
                // Startup advancement to get things started.
                VehicleLocation = 0;

                // Grab some data about our travels on the trail.
                var nextPoint = GetNextPointOfInterest();
                var currentPoint = GetCurrentPointOfInterest();

                // Check to make sure we are really at the next point based on all available data.
                if (DistanceToNextPoint > 0 || (nextPoint == null || nextPoint.DistanceLength <= 0))
                    return;

                // Setup next travel distance requirement.
                DistanceToNextPoint = nextPoint.DistanceLength;

                // Fire method to do some work and attach game modes based on this.
                OnReachedPointOfInterest(currentPoint);
            }
            else if (VehicleLocation < PointsOfInterest.Count())
            {
                // This is a normal advancement on the trail.
                VehicleLocation++;
                DistanceToNextPoint--;
            }
        }

        /// <summary>
        ///     Locates the next point of interest if it exists in the list, if this method returns NULL then that means the next
        ///     point of interest is the end of the game when the distance to point reaches zero.
        /// </summary>
        public PointOfInterest GetNextPointOfInterest()
        {
            // Build next point index from current point, even if startup value with -1 we add 1 so will always get first point.
            var nextPointIndex = VehicleLocation + 1;

            // Check if the next point is greater than point count, then get next point of interest if within bounds.
            return nextPointIndex > PointsOfInterest.Count() ? null : PointsOfInterest.ElementAt(nextPointIndex);
        }

        /// <summary>
        ///     Returns the current point of interest the players vehicle is on.
        /// </summary>
        public PointOfInterest GetCurrentPointOfInterest()
        {
            return VehicleLocation <= -1
                ? PointsOfInterest.First()
                : PointsOfInterest.ElementAt(VehicleLocation);
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
        public bool IsFirstPointOfInterest()
        {
            return VehicleLocation <= 0 && GameSimulationApp.Instance.TotalTurns <= 0;
        }

        /// <summary>
        ///     Fired when the players vehicle reaches the next point of interest on the trail.
        /// </summary>
        /// <param name="nextPoint">
        ///     Next point of interest that was peeked in the trail list. If this is null then next point is
        ///     end of game.
        /// </param>
        private void OnReachedPointOfInterest(PointOfInterest nextPoint)
        {
            // Fire event here for API subscribers to know point was reached. 
            OnReachPointOfInterest?.Invoke(nextPoint);

            // Attach some game mode based on the relevance of the next point type.
            GameSimulationApp.Instance.AddMode(nextPoint.ModeType);
        }
    }
}