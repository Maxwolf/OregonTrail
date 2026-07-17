// Created by Maxwolf (bigmaxwolf.com)

using OregonTrailDotNet.Entity.Location.Weather;

namespace OregonTrailDotNet.Module.Climate
{
    /// <summary>
    ///     The 1985 original's climate table, recovered from its VAR.BIN data file (see legacy/source/VAR.BIN.txt). Each of
    ///     the five bands of country the trail crosses has a coldest-expected temperature for every month and a daily chance
    ///     of precipitation. The game packed both into a character per figure - the temperature was the character's code less
    ///     fifty, the rain chance three thousandths of the code less thirty - so these numbers are the table as MECC shipped
    ///     it rather than anything reconstructed.
    /// </summary>
    internal static class ClimateRegistry
    {
        /// <summary>
        ///     Coldest expected temperature in Fahrenheit for each zone, by month (January first). A day's reading is drawn
        ///     from a forty degree band starting here, so these are the floor of the month rather than its average.
        /// </summary>
        private static readonly int[][] MonthlyLow =
        {
            new[] {9, 13, 23, 36, 45, 55, 60, 58, 50, 39, 25, 14}, // Missouri valley
            new[] {3, 8, 16, 29, 39, 49, 55, 53, 43, 31, 17, 7}, // Great plains
            new[] {3, 7, 12, 22, 32, 42, 51, 49, 38, 27, 13, 6}, // High country
            new[] {-1, 4, 12, 23, 32, 41, 49, 47, 37, 26, 11, 1}, // Snake river plain
            new[] {10, 16, 22, 30, 38, 45, 54, 52, 43, 32, 20, 12} // Pacific slope
        };

        /// <summary>
        ///     Chance of precipitation on any given day in each zone, by month (January first).
        /// </summary>
        private static readonly double[][] RainChance =
        {
            new[] {0.039, 0.042, 0.078, 0.099, 0.144, 0.144, 0.117, 0.120, 0.126, 0.090, 0.057, 0.045},
            new[] {0.015, 0.015, 0.030, 0.063, 0.090, 0.099, 0.081, 0.066, 0.048, 0.030, 0.015, 0.015},
            new[] {0.015, 0.015, 0.027, 0.048, 0.063, 0.039, 0.030, 0.018, 0.027, 0.027, 0.021, 0.015},
            new[] {0.015, 0.021, 0.036, 0.069, 0.075, 0.039, 0.024, 0.015, 0.033, 0.042, 0.024, 0.018},
            new[] {0.045, 0.039, 0.039, 0.036, 0.036, 0.027, 0.009, 0.009, 0.018, 0.030, 0.039, 0.042}
        };

        /// <summary>
        ///     Coldest expected temperature for a stretch of country in a given month.
        /// </summary>
        /// <param name="climate">Band of country the party is travelling through.</param>
        /// <param name="month">Month of the year, one for January.</param>
        /// <returns>Temperature in Fahrenheit.</returns>
        internal static int LowTemperature(ClimateEnum climate, int month)
        {
            return MonthlyLow[ZoneIndex(climate)][MonthIndex(month)];
        }

        /// <summary>
        ///     Chance of precipitation for a stretch of country on any day of a given month.
        /// </summary>
        /// <param name="climate">Band of country the party is travelling through.</param>
        /// <param name="month">Month of the year, one for January.</param>
        /// <returns>Probability between zero and one.</returns>
        internal static double PrecipitationChance(ClimateEnum climate, int month)
        {
            return RainChance[ZoneIndex(climate)][MonthIndex(month)];
        }

        /// <summary>
        ///     Clamps a climate to a row of the table, so an unmapped value reads as the first band rather than throwing.
        /// </summary>
        private static int ZoneIndex(ClimateEnum climate)
        {
            var zone = (int) climate;
            if (zone < 0)
                return 0;
            return zone >= MonthlyLow.Length ? MonthlyLow.Length - 1 : zone;
        }

        /// <summary>
        ///     Clamps a one-based month to a column of the table.
        /// </summary>
        private static int MonthIndex(int month)
        {
            if (month < 1)
                return 0;
            return month > 12 ? 11 : month - 1;
        }
    }
}
