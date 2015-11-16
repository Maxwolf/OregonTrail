namespace TrailSimulation.Game
{
    public class ClimateData
    {
        public ClimateData(Months cMonths, float cAverageTemp, float tempMax, float tempMin,
            float cRainfall, int avgHumidity)
        {
            ClimateMonth = cMonths;
            AverageDailyTemperature = cAverageTemp;
            MeanDailyMax = tempMax;
            MeanDailyMin = tempMin;
            MeanMonthlyRainfall = cRainfall;
            AverageDailyHumidity = avgHumidity;
        }

        public Months ClimateMonth { get; set; }

        public float AverageDailyTemperature { get; set; }

        public float MeanDailyMax { get; set; }

        public float MeanDailyMin { get; set; }

        public float MeanMonthlyRainfall { get; set; }

        public int AverageDailyHumidity { get; set; }
    }
}