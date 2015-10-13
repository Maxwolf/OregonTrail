namespace TrailCommon
{
    /// <summary>
    ///     Determines what the environment is like and goes into helping calculate roll chance. For example, experiencing
    ///     stormy weather will mean there is a higher chance of a travel event being triggered.
    /// </summary>
    public enum WeatherTier
    {
        Hot,
        Warm,
        Cold,
        Cool,
        Rainy,
        Stormy,
        Freezing
    }
}