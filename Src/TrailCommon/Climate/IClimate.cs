namespace TrailCommon
{
    public interface IClimate
    {
        Date Date { get; }
        uint GrassAvaliable { get; }
        WeatherCondition CalculateWeather();
        void UpdateClimate();
    }
}