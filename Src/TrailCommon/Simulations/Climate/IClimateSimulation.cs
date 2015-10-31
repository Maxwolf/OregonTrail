using System.Collections.Generic;

namespace TrailCommon
{
    public interface IClimateSimulation
    {
        ClimateClassification ClimateClassificationType { get; }
        float DisasterChance { get; }
        int InsideTemperature { get; }
        IEnumerable<ClimateData> AverageTemperatures { get; }
        WeatherCondition CurrentWeather { get; }
        uint GrassAvaliable { get; }
        int OutsideTemperature { get; }
        float InsideHumidity { get; }
        float OutsideHumidity { get; }
        double NextWeatherChance { get; }
        void TickClimate();
    }
}