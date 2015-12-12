using TrailSimulation.Game;

namespace TrailSimulation.Entity
{
    public class ClimateData
    {
        public ClimateData(Month cMonth, float cAverageTemp, float tempMax, float tempMin,
            float cRainfall, int avgHumidity)
        {
            ClimateMonth = cMonth;
            AverageDailyTemperature = cAverageTemp;
            MeanDailyMax = tempMax;
            MeanDailyMin = tempMin;
            MeanMonthlyRainfall = cRainfall;
            AverageDailyHumidity = avgHumidity;
        }

        public Month ClimateMonth { get; set; }

        public float AverageDailyTemperature { get; set; }

        public float MeanDailyMax { get; set; }

        public float MeanDailyMin { get; set; }

        public float MeanMonthlyRainfall { get; set; }

        public int AverageDailyHumidity { get; set; }
    }
}