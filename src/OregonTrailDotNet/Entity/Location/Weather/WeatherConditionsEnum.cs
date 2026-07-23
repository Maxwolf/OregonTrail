// Created by Maxwolf (bigmaxwolf.com)
// Timestamp 01/03/2016@1:50 AM

using System.ComponentModel;

namespace OregonTrailDotNet.Entity.Location.Weather
{
    /// <summary>
    ///     What the weather is doing today. The first six are the temperature the party is walking through, banded from the
    ///     day's reading; the last four replace that reading entirely when there is something falling out of the sky, since
    ///     what matters then is the rain or the snow rather than the thermometer. Colder days turn rain to snow, and heavy
    ///     weather is worse for the party than light. Values are the band the original indexed its weather names by.
    /// </summary>
    public enum WeatherConditionsEnum
    {
        /// <summary>
        ///     Freezing and dangerous to a party without warm clothes.
        /// </summary>
        [Description("very cold")] VeryCold = 0,

        /// <summary>
        ///     Cold enough that clothing matters.
        /// </summary>
        [Description("cold")] Cold = 1,

        /// <summary>
        ///     Cool but comfortable.
        /// </summary>
        [Description("cool")] Cool = 2,

        /// <summary>
        ///     Warm: the easiest travelling weather there is.
        /// </summary>
        [Description("warm")] Warm = 3,

        /// <summary>
        ///     Hot enough to wear on the party.
        /// </summary>
        [Description("hot")] Hot = 4,

        /// <summary>
        ///     Baking, and as hard on the party as the cold.
        /// </summary>
        [Description("very hot")] VeryHot = 5,

        /// <summary>
        ///     Rain.
        /// </summary>
        [Description("rainy")] Rainy = 6,

        /// <summary>
        ///     Rain that fell as snow because the day was cold.
        /// </summary>
        [Description("snowy")] Snowy = 7,

        /// <summary>
        ///     A downpour, which swells the rivers ahead considerably faster.
        /// </summary>
        [Description("very rainy")] VeryRainy = 8,

        /// <summary>
        ///     A blizzard: the worst travelling weather in the game.
        /// </summary>
        [Description("very snowy")] VerySnowy = 9
    }
}
