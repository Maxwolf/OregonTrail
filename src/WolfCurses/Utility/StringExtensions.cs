// Created by Ron 'Maxwolf' McDowell (ron.mcdowell@gmail.com) 
// Timestamp 12/31/2015@4:49 AM

using System;
using System.Text;

namespace OregonTrailDotNet.WolfCurses.Utility
{
    /// <summary>
    ///     Utility class that contains useful extension methods that make working with strings a little easier and less
    ///     repetitive.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        ///     Cuts off a string given a certain input amount, useful for ensuring the user never enters more than possible into a
        ///     given field.
        /// </summary>
        /// <param name="value">String that needs to be truncated to max length.</param>
        /// <param name="maxLength">Negative values will cause exception.</param>
        /// <returns>Truncated string.</returns>
        public static string Truncate(this string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value)) return value;
            return value.Length <= maxLength ? value : value.Substring(0, maxLength);
        }

        /// <summary>Word wraps the given text to fit within the specified width.</summary>
        /// <remarks>http://www.codeproject.com/Articles/51488/Implementing-Word-Wrap-in-C</remarks>
        /// <param name="text">Text to be word wrapped</param>
        /// <param name="width">
        ///     Width, in characters, to which the text
        ///     should be word wrapped
        /// </param>
        /// <returns>The modified text</returns>
        public static string WordWrap(this string text, int width = 32)
        {
            int pos, next;
            var sb = new StringBuilder();

            // Lucidity check
            if (width < 1)
                return text;

            // Parse each line of text
            for (pos = 0; pos < text.Length; pos = next)
            {
                // Find end of line
                var eol = text.IndexOf(Environment.NewLine, pos, StringComparison.Ordinal);
                if (eol == -1)
                    next = eol = text.Length;
                else
                    next = eol + Environment.NewLine.Length;

                // Copy this line of text, breaking into smaller lines as needed
                if (eol > pos)
                {
                    do
                    {
                        var len = eol - pos;
                        if (len > width)
                            len = BreakLine(text, pos, width);
                        sb.Append(text, pos, len);
                        sb.Append(Environment.NewLine);

                        // Trim whitespace following break
                        pos += len;
                        while (pos < eol && char.IsWhiteSpace(text[pos]))
                            pos++;
                    }
                    while (eol > pos);
                }
                else sb.Append(Environment.NewLine); // Empty line
            }

            return sb.ToString();
        }

        /// <summary>
        ///     Locates position to break the given line so as to avoid
        ///     breaking words.
        /// </summary>
        /// <param name="text">String that contains line of text</param>
        /// <param name="pos">Index where line of text starts</param>
        /// <param name="max">Maximum line length</param>
        /// <returns>The modified line length</returns>
        private static int BreakLine(string text, int pos, int max)
        {
            // Find last whitespace in line
            var i = max;
            while (i >= 0 && !char.IsWhiteSpace(text[pos + i]))
                i--;

            // If no whitespace found, break at maximum length
            if (i < 0)
                return max;

            // Find start of whitespace
            while (i >= 0 && char.IsWhiteSpace(text[pos + i]))
                i--;

            // Return length of text before whitespace
            return i + 1;
        }
    }
}