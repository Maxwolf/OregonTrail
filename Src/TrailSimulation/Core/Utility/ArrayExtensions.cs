using System.Collections.Generic;
using System.Linq;

namespace TrailSimulation.Core
{
    /// <summary>
    ///     Helper methods for working with arrays.
    /// </summary>
    public static class ArrayExtensions
    {
        /// <summary>
        ///     Determines the count of boolean values in an array that are true. Intended to be used in conjunction with count to
        ///     determine if all are true for a entire sequence.
        /// </summary>
        /// <param name="booleans">Array of bool.</param>
        /// <returns>Number of bool values in array that were true.</returns>
        /// <remarks>http://stackoverflow.com/a/378282</remarks>
        public static int TrueCount(this IEnumerable<bool> booleans)
        {
            return booleans.Count(b => b);
        }
    }
}