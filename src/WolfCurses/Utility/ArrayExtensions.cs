// Created by Ron 'Maxwolf' McDowell (ron.mcdowell@gmail.com) 
// Timestamp 12/31/2015@4:49 AM

using System;
using System.Collections.Generic;
using System.Linq;

namespace OregonTrailDotNet.WolfCurses.Utility
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

        /// <summary>
        ///     NB Method will return int. MaxValue for a sequence containing no elements. Intended to be used to match int value
        ///     to enumeration but without directly casting it, instead looking for closest match to target value.
        /// </summary>
        /// <param name="collection">Enumerable collection of integers that make up our collection.</param>
        /// <param name="target">Target value which needs to be compared against collection values.</param>
        /// <returns>Int closest matching in collection to target value.</returns>
        /// <remarks>http://stackoverflow.com/a/10120982</remarks>
        public static int ClosestTo(this IEnumerable<int> collection, int target)
        {
            var closest = int.MaxValue;
            var minDifference = int.MaxValue;
            foreach (var element in collection)
            {
                var difference = Math.Abs((long) element - target);
                if (minDifference <= difference)
                    continue;

                minDifference = (int) difference;
                closest = element;
            }

            return closest;
        }
    }
}