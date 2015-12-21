// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Weather.cs" company="Ron 'Maxwolf' McDowell">
//   ron.mcdowell@gmail.com
// </copyright>
// <summary>
//   Determines what the environment is like and goes into helping calculate roll chance. For example, experiencing
//   stormy weather will mean there is a higher chance of a travel event being triggered.
// </summary>
// --------------------------------------------------------------------------------------------------------------------



using System.ComponentModel;

namespace TrailSimulation.Entity
{
    /// <summary>
    ///     Determines what the environment is like and goes into helping calculate roll chance. For example, experiencing
    ///     stormy weather will mean there is a higher chance of a travel event being triggered.
    /// </summary>
    public enum Weather
    {
        /// <summary>
        /// The partly sunny.
        /// </summary>
        [Description("Partly Sunny")]
        PartlySunny, 

        /// <summary>
        /// The scattered thunderstorms.
        /// </summary>
        [Description("Scattered Thunderstorms")]
        ScatteredThunderstorms, 

        /// <summary>
        /// The scattered showers.
        /// </summary>
        [Description("Scattered Showers")]
        ScatteredShowers, 

        /// <summary>
        /// The overcast.
        /// </summary>
        [Description("Overcast")]
        Overcast, 

        /// <summary>
        /// The light snow.
        /// </summary>
        [Description("Light Snow")]
        LightSnow, 

        /// <summary>
        /// The freezing drizzle.
        /// </summary>
        [Description("Freezing Drizzle")]
        FreezingDrizzle, 

        /// <summary>
        /// The chance of rain.
        /// </summary>
        [Description("Chance Of Rain")]
        ChanceOfRain, 

        /// <summary>
        /// The sunny.
        /// </summary>
        [Description("Sunny")]
        Sunny, 

        /// <summary>
        /// The clear.
        /// </summary>
        [Description("Clear")]
        Clear, 

        /// <summary>
        /// The mostly sunny.
        /// </summary>
        [Description("Mostly Sunny")]
        MostlySunny, 

        /// <summary>
        /// The rain.
        /// </summary>
        [Description("Rain")]
        Rain, 

        /// <summary>
        /// The cloudy.
        /// </summary>
        [Description("Cloudy")]
        Cloudy, 

        /// <summary>
        /// The storm.
        /// </summary>
        [Description("Storm")]
        Storm, 

        /// <summary>
        /// The thunderstorm.
        /// </summary>
        [Description("Thunderstorm")]
        Thunderstorm, 

        /// <summary>
        /// The chance of thunderstorm.
        /// </summary>
        [Description("Chance Of Thunderstorm")]
        ChanceOfThunderstorm, 

        /// <summary>
        /// The sleet.
        /// </summary>
        [Description("Sleet")]
        Sleet, 

        /// <summary>
        /// The snow.
        /// </summary>
        [Description("Snow")]
        Snow, 

        /// <summary>
        /// The icy.
        /// </summary>
        [Description("Icy")]
        Icy, 

        /// <summary>
        /// The fog.
        /// </summary>
        [Description("Fog")]
        Fog, 

        /// <summary>
        /// The haze.
        /// </summary>
        [Description("Haze")]
        Haze, 

        /// <summary>
        /// The flurries.
        /// </summary>
        [Description("Flurries")]
        Flurries, 

        /// <summary>
        /// The snow showers.
        /// </summary>
        [Description("Snow Showers")]
        SnowShowers, 

        /// <summary>
        /// The hail.
        /// </summary>
        [Description("Hail")]
        Hail
    }
}