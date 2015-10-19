using System;
using System.Collections.Generic;
using System.Linq;

namespace TrailCommon.CommandLine_Parser
{
    /// <summary>
    ///     It annoys me that there's no function to split a string based on a function that examines each character.
    /// </summary>
    /// <remarks>
    ///     http://stackoverflow.com/questions/298830/split-string-containing-command-line-parameters-into-string-in-c-sharp/298990
    /// </remarks>
    public static class ArgumentSplitter
    {
        /// <summary>
        ///     Splits up a string into an array of strings based on the idea that each space is a separator for another program
        ///     argument.
        /// </summary>
        /// <param name="commandLine"></param>
        /// <returns></returns>
        public static IEnumerable<string> SplitCommandLine(string commandLine)
        {
            var inQuotes = false;
            return commandLine.Split(c =>
            {
                if (c == '\"')
                    inQuotes = !inQuotes;

                return !inQuotes && c == ' ';
            })
                //.Select(arg => arg.Trim().TrimMatchingQuotes('\"'))
                .Select(arg => arg.Trim().Replace("\"", ""))
                .Where(arg => !string.IsNullOrEmpty(arg));
        }

        /// <summary>
        ///     My own version of Split that takes a function that has to decide whether the specified character should split the
        ///     string.
        /// </summary>
        /// <remarks>Will not remove final quotation mark, only way to ensure is with blanket replace all instead.</remarks>
        public static IEnumerable<string> Split(this string str,
            Func<char, bool> controller)
        {
            var nextPiece = 0;
            for (var c = 0; c < str.Length; c++)
            {
                if (!controller(str[c]))
                    continue;

                yield return str.Substring(nextPiece, c - nextPiece);
                nextPiece = c + 1;
            }

            yield return str.Substring(nextPiece);
        }

        /// <summary>
        ///     Helper method that will trim a matching pair of quotes from the start and end of a string.
        /// </summary>
        public static string TrimMatchingQuotes(this string input, char quote)
        {
            if ((input.Length >= 2) &&
                (input[0] == quote) && (input[input.Length - 1] == quote))
                return input.Substring(1, input.Length - 2);

            return input;
        }
    }
}