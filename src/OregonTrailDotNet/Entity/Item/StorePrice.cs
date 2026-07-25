// Created by Maxwolf (bigmaxwolf.com)
// Timestamp 01/03/2016@1:50 AM

using System;
using System.Linq;
using OregonTrailDotNet.Entity.Location;

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
        ///     How many of the original's six price thresholds the party has passed, keyed to WHERE they are on the
        ///     trail rather than to which forts they happened to stop at.
        ///     <para>
        ///         The original counts trail position outright — <c>Q = (LM&gt;2)+(LM&gt;4)+(LM&gt;7)+(LM&gt;10)+(LM&gt;12)+(LM&gt;13)</c>
        ///         at <c>BUY.LIB:50003</c> — so the markup is the same however you got there. Counting departed forts
        ///         instead let a fork dodge the price rise: taking Green River rather than Fort Bridger never splices a
        ///         settlement into the trail, so every store for the rest of the journey stayed a step cheap (Fort Hall
        ///         1.75x instead of 2.00x, and so on down to Walla Walla) on the branch that already saves 86 miles.
        ///         Naming the thresholds fixes that, because a name does not move when the trail is re-spliced.
        ///     </para>
        /// </summary>
        internal static int MarkupSteps
        {
            get
            {
                var locations = GameSimulationApp.Instance?.Trail?.Locations;
                if (locations == null)
                    return 0;

                // Each fort is one threshold, in trail order. A threshold is behind the party when the fort itself has
                // been reached or left behind.
                var passed = new bool[Thresholds.Length];
                var onTrail = new bool[Thresholds.Length];
                for (var i = 0; i < Thresholds.Length; i++)
                {
                    var fort = locations.FirstOrDefault(location =>
                        string.Equals(location.Name, Thresholds[i], StringComparison.OrdinalIgnoreCase));

                    onTrail[i] = fort != null;
                    passed[i] = fort != null && fort.Status != LocationStatusEnum.Unreached;
                }

                // A fort a fork spliced out of the trail (Fort Bridger when the party takes Green River, Fort Walla
                // Walla when it takes the Barlow road) is never reached and so never marks itself passed. But the
                // party got past that stretch of country all the same, and the original charged them for it — its Q
                // counts trail position, not shopping trips. So an absent threshold inherits from the one before it,
                // forward, which lets a run of skipped forts propagate. This is what stops a fork from buying at a
                // permanently cheaper price than the branch that keeps its fort. Nothing inherits at Independence,
                // where no threshold has been passed yet, so the opening store still quotes base prices.
                for (var i = 1; i < Thresholds.Length; i++)
                    if (!onTrail[i] && passed[i - 1])
                        passed[i] = true;

                return passed.Count(step => step);
            }
        }

        /// <summary>
        ///     The six forts whose price thresholds the original hard-coded, in trail order. Named rather than counted
        ///     so a re-spliced trail cannot shift them.
        /// </summary>
        private static readonly string[] Thresholds =
        {
            "Fort Kearney", "Fort Laramie", "Fort Bridger", "Fort Hall", "Fort Boise", "Fort Walla Walla"
        };

        /// <summary>
        ///     Returns the base cost of an item marked up by a quarter of that base for each fort the party has left behind,
        ///     capped at the original's six thresholds.
        /// </summary>
        /// <param name="baseCost">Cost of the item at Matt's General Store in Independence.</param>
        /// <returns>The scaled cost.</returns>
        internal static float Scaled(float baseCost)
        {
            var steps = MarkupSteps;
            if (steps > MaxMarkupSteps)
                steps = MaxMarkupSteps;

            return baseCost*(1f + MarkupPerFort*steps);
        }
    }
}
