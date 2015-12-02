using System.ComponentModel;

namespace TrailSimulation.Game
{
    /// <summary>
    ///     Determines what the environment is like and goes into helping calculate roll chance. For example, experiencing
    ///     stormy weather will mean there is a higher chance of a travel event being triggered.
    /// </summary>
    public enum WeatherCondition
    {
        [Description("Partly Sunny")]
        PartlySunny,

        [Description("Scattered Thunderstorms")]
        ScatteredThunderstorms,

        [Description("Scattered Showers")]
        ScatteredShowers,

        [Description("Overcast")]
        Overcast,

        [Description("Light Snow")]
        LightSnow,

        [Description("Freezing Drizzle")]
        FreezingDrizzle,

        [Description("Chance Of Rain")]
        ChanceOfRain,

        [Description("Sunny")]
        Sunny,

        [Description("Clear")]
        Clear,

        [Description("Mostly Sunny")]
        MostlySunny,

        [Description("Rain")]
        Rain,

        [Description("Cloudy")]
        Cloudy,

        [Description("Storm")]
        Storm,

        [Description("Thunderstorm")]
        Thunderstorm,

        [Description("Chance Of Thunderstorm")]
        ChanceOfThunderstorm,

        [Description("Sleet")]
        Sleet,

        [Description("Snow")]
        Snow,

        [Description("Icy")]
        Icy,

        [Description("Fog")]
        Fog,

        [Description("Haze")]
        Haze,

        [Description("Flurries")]
        Flurries,

        [Description("Snow Showers")]
        SnowShowers,

        [Description("Hail")]
        Hail
    }
}