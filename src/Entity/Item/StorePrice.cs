// Created by Maxwolf (bigmaxwolf.com)
// Timestamp 01/03/2016@1:50 AM

using System.Linq;
using OregonTrailDotNet.Entity.Location;
using OregonTrailDotNet.Entity.Location.Point;

namespace OregonTrailDotNet.Entity.Item
{
    /// <summary>
    ///     Helper for scaling store item prices as the party travels further along the trail. In the original game the cost of
    ///     goods climbed at every fort the further west you got; this reproduces that by adding a per-fort delta to an item's
    ///     base cost for every settlement (fort) the party has already left behind. The vehicle's own inventory keeps its
    ///     creation-time base cost, so only newly generated store items reflect the higher prices.
    /// </summary>
    internal static class StorePrice
    {
        /// <summary>
        ///     Number of settlement (fort) locations the party has already departed. Null-guards the singleton and trail so it
        ///     is safe to call during early construction and teardown, returning zero when nothing is available yet.
        /// </summary>
        internal static int FortsDeparted
        {
            get
            {
                var locations = GameSimulationApp.Instance?.Trail?.Locations;
                if (locations == null)
                    return 0;

                return locations.Count(location =>
                    location is Settlement && location.Status == LocationStatusEnum.Departed);
            }
        }

        /// <summary>
        ///     Returns the base cost of an item bumped by a per-fort delta for every fort the party has already departed.
        /// </summary>
        /// <param name="baseCost">Cost of the item at the very start of the trail.</param>
        /// <param name="perFortDelta">Amount the price rises for each fort departed.</param>
        /// <returns>The scaled cost.</returns>
        internal static float Scaled(float baseCost, float perFortDelta)
        {
            return baseCost + perFortDelta*FortsDeparted;
        }
    }
}
