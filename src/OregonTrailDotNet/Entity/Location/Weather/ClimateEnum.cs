// Created by Maxwolf (bigmaxwolf.com)
// Timestamp 01/03/2016@1:50 AM

namespace OregonTrailDotNet.Entity.Location.Weather
{
    /// <summary>
    ///     The five bands of country the trail passes through, each with its own temperatures and its own odds of rain.
    ///     The original game kept one climate table per band and picked the band purely from how far along the trail you
    ///     were, which is why these are stretches of the journey rather than climate types in the abstract: the same weather
    ///     model has to cover a Missouri summer and a winter in the Blue Mountains. Values are the table index.
    /// </summary>
    public enum ClimateEnum
    {
        /// <summary>
        ///     Missouri and the Kansas river country, from Independence to the Big Blue. The mildest and the wettest of the
        ///     plains country.
        /// </summary>
        MissouriValley = 0,

        /// <summary>
        ///     The open plains from Fort Kearney out to Fort Laramie. Colder than the country behind it and drier.
        /// </summary>
        GreatPlains = 1,

        /// <summary>
        ///     The high country either side of South Pass, from Independence Rock through to Soda Springs. Dry, and cold for
        ///     how far south it sits.
        /// </summary>
        HighCountry = 2,

        /// <summary>
        ///     The Snake river country, Fort Hall to Fort Boise. The coldest stretch of the trail - the only one that freezes
        ///     in January.
        /// </summary>
        SnakeRiverPlain = 3,

        /// <summary>
        ///     The Blue Mountains and the run down to the valley. Milder than the Snake country but wetter.
        /// </summary>
        PacificSlope = 4
    }
}
