// Created by Ron 'Maxwolf' McDowell (ron.mcdowell@gmail.com) 
// Timestamp 12/31/2015@4:49 AM

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace OregonTrailDotNet.WolfCurses.Window.Control
{
    /// <summary>
    ///     Converts lists of objects into string table representations of themselves.
    /// </summary>
    public static class TextTableExtensions
    {
        /// <summary>The to string table.</summary>
        /// <param name="values">The values.</param>
        /// <param name="columnHeaders">The column headers.</param>
        /// <param name="valueSelectors">The value selectors.</param>
        /// <typeparam name="T"></typeparam>
        /// <returns>The <see cref="string" />.</returns>
        public static string ToStringTable<T>(this IEnumerable<T> values, string[] columnHeaders,
            params Func<T, object>[] valueSelectors)
        {
            return ToStringTable(values.ToArray(), columnHeaders, valueSelectors);
        }

        /// <summary>The to string table.</summary>
        /// <param name="values">The values.</param>
        /// <param name="columnHeaders">The column headers.</param>
        /// <param name="valueSelectors">The value selectors.</param>
        /// <typeparam name="T"></typeparam>
        /// <returns>The <see cref="string" />.</returns>
        public static string ToStringTable<T>(this T[] values, string[] columnHeaders,
            params Func<T, object>[] valueSelectors)
        {
            Debug.Assert(columnHeaders.Length == valueSelectors.Length);

            var arrValues = new string[values.Length + 1, valueSelectors.Length];

            // Fill headers
            for (var colIndex = 0; colIndex < arrValues.GetLength(1); colIndex++)
            {
                arrValues[0, colIndex] = columnHeaders[colIndex];
            }

            // Fill table rows
            for (var rowIndex = 1; rowIndex < arrValues.GetLength(0); rowIndex++)
            {
                for (var colIndex = 0; colIndex < arrValues.GetLength(1); colIndex++)
                {
                    var value = valueSelectors[colIndex].Invoke(values[rowIndex - 1]);

                    arrValues[rowIndex, colIndex] = value?.ToString() ?? "null";
                }
            }

            return ToStringTable(arrValues);
        }

        /// <summary>The to string table.</summary>
        /// <param name="arrValues">The arr values.</param>
        /// <returns>The <see cref="string" />.</returns>
        public static string ToStringTable(this string[,] arrValues)
        {
            var maxColumnsWidth = GetMaxColumnsWidth(arrValues);
            var headerSpliter = new string('-', maxColumnsWidth.Sum(i => i + 3) - 1);

            var sb = new StringBuilder();
            for (var rowIndex = 0; rowIndex < arrValues.GetLength(0); rowIndex++)
            {
                for (var colIndex = 0; colIndex < arrValues.GetLength(1); colIndex++)
                {
                    // Print cell
                    var cell = arrValues[rowIndex, colIndex];
                    cell = cell.PadRight(maxColumnsWidth[colIndex]);
                    sb.Append(" | ");
                    sb.Append(cell);
                }

                // Print end of line
                sb.Append(" | ");
                sb.AppendLine();

                // Print splitter
                if (rowIndex == 0)
                {
                    sb.AppendFormat(" |{0}| ", headerSpliter);
                    sb.AppendLine();
                }
            }

            return sb.ToString();
        }

        /// <summary>The get max columns width.</summary>
        /// <param name="arrValues">The arr values.</param>
        /// <returns>The <see cref="int[]" />.</returns>
        private static int[] GetMaxColumnsWidth(string[,] arrValues)
        {
            var maxColumnsWidth = new int[arrValues.GetLength(1)];
            for (var colIndex = 0; colIndex < arrValues.GetLength(1); colIndex++)
            {
                for (var rowIndex = 0; rowIndex < arrValues.GetLength(0); rowIndex++)
                {
                    var newLength = arrValues[rowIndex, colIndex].Length;
                    var oldLength = maxColumnsWidth[colIndex];

                    if (newLength > oldLength)
                    {
                        maxColumnsWidth[colIndex] = newLength;
                    }
                }
            }

            return maxColumnsWidth;
        }

        /// <summary>The to string table.</summary>
        /// <param name="values">The values.</param>
        /// <param name="valueSelectors">The value selectors.</param>
        /// <typeparam name="T"></typeparam>
        /// <returns>The <see cref="string" />.</returns>
        public static string ToStringTable<T>(this IEnumerable<T> values,
            params Expression<Func<T, object>>[] valueSelectors)
        {
            var list = new List<string>();
            foreach (var func in valueSelectors)
                list.Add(GetProperty(func).Name);

            var headers = list.ToArray();
            var list1 = new List<Func<T, object>>();
            foreach (var exp in valueSelectors)
                list1.Add(exp.Compile());

            var selectors = list1.ToArray();
            return ToStringTable(values, headers, selectors);
        }

        /// <summary>The get property.</summary>
        /// <param name="expresstion">The expression.</param>
        /// <typeparam name="T"></typeparam>
        /// <returns>The <see cref="PropertyInfo" />.</returns>
        private static PropertyInfo GetProperty<T>(Expression<Func<T, object>> expresstion)
        {
            if ((expresstion.Body as UnaryExpression)?.Operand is MemberExpression)
            {
                return (((UnaryExpression) expresstion.Body).Operand as MemberExpression)?.Member as PropertyInfo;
            }

            return (expresstion.Body as MemberExpression)?.Member as PropertyInfo;
        }
    }
}