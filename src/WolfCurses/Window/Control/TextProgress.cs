// Created by Ron 'Maxwolf' McDowell (ron.mcdowell@gmail.com) 
// Timestamp 12/31/2015@4:49 AM

using System;
using System.Text;

namespace OregonTrailDotNet.WolfCurses.Window.Control
{
    /// <summary>
    ///     Text manipulation utilities for dealing with displaying progress visually as text in a console application.
    /// </summary>
    public static class TextProgress
    {
        /// <summary>
        ///     Creates text progress bar based on input parameters at specified value with inputted character as progress
        ///     character.
        /// </summary>
        /// <param name="value">Current value of the progress bar, should with within range of max value.</param>
        /// <param name="maxValue">Maximum value that the progress bar can be.</param>
        /// <param name="barSize">Total size of the progress bar.</param>
        /// <returns>The <see cref="string" />.</returns>
        public static string DrawProgressBar(int value, int maxValue, int barSize)
        {
            // Unicode block characters.
            var progressCharacter = Convert.ToChar(9608);
            var barCharacter = Convert.ToChar(9618);

            // Build visual progress representation.
            var output = new StringBuilder();
            var perc = value/(decimal) maxValue;
            var chars = (int) Math.Floor(perc/(1/(decimal) barSize));
            string p1 = string.Empty, p2 = string.Empty;

            // Foreground.
            for (var i = 0; i < chars; i++)
                p1 += progressCharacter;

            // Background.
            for (var i = 0; i < barSize - chars; i++)
                p2 += barCharacter;

            // Print the bar, and then fill it in with progress characters.
            output.Append(p1);
            output.Append(p2);

            // Percentage output at end.
            output.AppendFormat(" {0}%", (perc*100).ToString("N2"));
            return output.ToString();
        }
    }
}