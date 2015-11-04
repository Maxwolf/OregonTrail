using System;
using System.Text;

namespace TrailEntities
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
        /// <param name="progressCharacter">Character that is used to fill the progress bar.</param>
        /// <returns></returns>
        public static string DrawProgressBar(int value, int maxValue, int barSize, char progressCharacter)
        {
            var output = new StringBuilder();
            var perc = value/(decimal) maxValue;
            var chars = (int) Math.Floor(perc/(1/(decimal) barSize));
            string p1 = string.Empty, p2 = string.Empty;

            for (var i = 0; i < chars; i++) p1 += progressCharacter;
            for (var i = 0; i < barSize - chars; i++) p2 += progressCharacter;

            output.Append(p1);
            output.Append(p2);

            output.AppendFormat(" {0}%", (perc*100).ToString("N2"));
            return output.ToString();
        }
    }
}