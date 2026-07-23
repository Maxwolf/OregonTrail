// Created by Maxwolf (bigmaxwolf.com)
// Timestamp 01/03/2016@1:50 AM

using System.Linq;
using OregonTrailDotNet.Entity.Location;
using OregonTrailDotNet.Entity.Location.Point;

namespace OregonTrailDotNet.Entity.Item
{
    /// <summary>
    ///     Helper for scaling store item prices as the party travels further along the trail. In the original game the cost of
    ///     goods climbed the further west you got, by a flat quarter of the item's base price at each fort past Independence:
    ///     the store multiplied every quoted price by (1 + .25 * Q), where Q counted how many of six fixed points along the
    ///     trail the party had passed. Those six points sit immediately before each successive fort, so Q is simply the number
    ///     of forts reached beyond Independence, and prices top out at two and a half times base. The vehicle's own inventory
    ///     keeps its creation-time base cost, so only newly generated store items reflect the higher prices.
    /// </summary>
    internal static class StorePrice
    {
        /// <summary>
        ///     Fraction of an item's base price added at each fort reached beyond Independence.
        /// </summary>
        private const float MarkupPerFort = 0.25f;

        /// <summary>
        ///     The original counted six price thresholds along the trail, so the markup stops climbing after the sixth fort.
        /// </summary>
        private const int MaxMarkupSteps = 6;

        /// <summary>
        ///     Number of settlement (fort) locations the party has already departed. Standing at a fort, every earlier fort
        ///     (including Independence) has been departed, which makes this count line up with the original's threshold count.
        ///     Null-guards the singleton and trail so it is safe to call during early construction and teardown, returning zero
        ///     when nothing is available yet.
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
        ///     Returns the base cost of an item marked up by a quarter of that base for each fort the party has left behind,
        ///     capped at the original's six thresholds.
        /// </summary>
        /// <param name="baseCost">Cost of the item at Matt's General Store in Independence.</param>
        /// <returns>The scaled cost.</returns>
        internal static float Scaled(float baseCost)
        {
            var steps = FortsDeparted;
            if (steps > MaxMarkupSteps)
                steps = MaxMarkupSteps;

            return baseCost*(1f + MarkupPerFort*steps);
        }
    }
}
