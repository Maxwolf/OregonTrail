// Created by Ron 'Maxwolf' McDowell (ron.mcdowell@gmail.com) 
// Timestamp 12/31/2015@4:49 AM

using System;
using System.Collections.Generic;
using System.Linq;

namespace OregonTrailDotNet.WolfCurses.Utility
{
    /// <summary>
    ///     Collection of extension methods used for manipulating a enumerable collection of objects.
    /// </summary>
    /// <remarks>http://stackoverflow.com/a/2019433</remarks>
    public static class EnumerableExtension
    {
        /// <summary>Picks a random element from the list.</summary>
        /// <typeparam name="T">Type of list.</typeparam>
        /// <param name="source">Source collection.</param>
        /// <returns>Random element from list<see cref="T" />.</returns>
        public static T PickRandom<T>(this IEnumerable<T> source)
        {
            return source.PickRandom(1).Single();
        }

        /// <summary>Picks a random element from the list.</summary>
        /// <typeparam name="T">Type of list.</typeparam>
        /// <param name="source">Source collection.</param>
        /// <param name="count">Total number of elements in the collection.</param>
        /// <returns>Shuffled list of elements<see cref="IEnumerable" />.</returns>
        public static IEnumerable<T> PickRandom<T>(this IEnumerable<T> source, int count)
        {
            return source.Shuffle().Take(count);
        }

        /// <summary>Bubble sorts all the elements in the collection.</summary>
        /// <typeparam name="T">Type of list.</typeparam>
        /// <param name="source">Source collection.</param>
        /// <returns>Shuffled list of collection elements<see cref="IEnumerable" />.</returns>
        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source)
        {
            return source.OrderBy(x => Guid.NewGuid());
        }
    }
}