namespace TrailEntities.Game
{
    /// <summary>
    ///     Determines what the environment is like and goes into helping calculate roll chance. For example, experiencing
    ///     stormy weather will mean there is a higher chance of a travel event being triggered.
    /// </summary>
    public enum WeatherCondition
    {
        PartlySunny,
        ScatteredThunderstroms,
        Showers,
        ScatteredShowers,
        RainAndSnow,
        Overcast,
        LightSnow,
        FreezingDrizzle,
        ChanceOfRain,
        Sunny,
        Clear,
        MostlySunny,
        PartlyCloudy,
        MostlyCloudy,
        ChangeOfStorm,
        Rain,
        ChanceOfSnow,
        Cloudy,
        Mist,
        Storm,
        Thunderstorm,
        ChanceOfTStorm,
        Sleet,
        Snow,
        Icy,
        Dust,
        Fog,
        Smoke,
        Haze,
        Flurries,
        LightRain,
        SnowShowers,
        Hail
    }
}