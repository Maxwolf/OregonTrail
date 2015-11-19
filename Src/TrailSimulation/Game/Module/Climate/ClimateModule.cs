using System;
using System.Collections.Generic;
using TrailSimulation.Core;

namespace TrailSimulation.Game
{
    public sealed class ClimateModule : SimulationModule
    {
        private List<ClimateData> _averageTemperatures;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailEntities.ClimateModule" /> class.
        /// </summary>
        public ClimateModule(ClimateClassification climateClassificationType)
        {
            ClimateClassificationType = climateClassificationType;

            // Select climate and determine humidity and temperature based on it.
            switch (ClimateClassificationType)
            {
                case ClimateClassification.Polar:
                    _averageTemperatures = new List<ClimateData>
                    {
                        new ClimateData(Months.January, -2.9f, -0.2f, -5.5f, 15,
                            1),
                        new ClimateData(Months.February, -9.5f, -6.3f, -11.6f,
                            21.2f, 2),
                        new ClimateData(Months.March, -18.2f, -14, -21.1f,
                            24.1f, 4),
                        new ClimateData(Months.April, -20.7f, -17.4f, -24.9f,
                            18.4f, 2),
                        new ClimateData(Months.May, -21.7f, -19, -27.1f, 18.4f,
                            1),
                        new ClimateData(Months.June, -23, -19.1f, -27.3f, 24.9f,
                            4),
                        new ClimateData(Months.July, -25.7f, -21.7f, -30.1f,
                            15.6f, 5),
                        new ClimateData(Months.August, -26.1f, -22.8f, -31.8f,
                            11.3f, 3),
                        new ClimateData(Months.September, -24.6f, -20.8f,
                            -29.4f, 11.8f, 2),
                        new ClimateData(Months.October, -18.9f, -15.5f, -23.4f,
                            9.7f, 3),
                        new ClimateData(Months.November, -9.7f, -6.7f, -12.7f,
                            9.5f, 4),
                        new ClimateData(Months.December, -3.4f, -0.8f, -6,
                            15.7f, 2)
                    };
                    break;
                case ClimateClassification.Continental:
                    _averageTemperatures = new List<ClimateData>
                    {
                        new ClimateData(Months.January, 20.5f, 28, 13, 54, 53),
                        new ClimateData(Months.February, 20, 27, 13, 55, 54),
                        new ClimateData(Months.March, 17, 24, 10, 63, 58),
                        new ClimateData(Months.April, 14, 20, 7, 55, 60),
                        new ClimateData(Months.May, 9, 15, 3, 52, 71),
                        new ClimateData(Months.June, 7, 12, 1, 49, 79),
                        new ClimateData(Months.July, 6, 11, 0, 41, 78),
                        new ClimateData(Months.August, 7, 13, 1, 50, 70),
                        new ClimateData(Months.September, 10, 16, 3, 41, 62),
                        new ClimateData(Months.October, 13, 19, 6, 70, 55),
                        new ClimateData(Months.November, 15, 22, 8, 54, 53),
                        new ClimateData(Months.December, 18.5f, 26, 11, 47, 53)
                    };
                    break;
                case ClimateClassification.Moderate:
                    _averageTemperatures = new List<ClimateData>
                    {
                        new ClimateData(Months.January, -6.1f, 8.6f, -8.8f, 40,
                            85),
                        new ClimateData(Months.February, -6f, 10.2f, -8.8f, 31,
                            82),
                        new ClimateData(Months.March, -1.4f, 14.9f, -4.2f, 35,
                            77),
                        new ClimateData(Months.April, 4.4f, 25.3f, 1.0f, 33, 71),
                        new ClimateData(Months.May, 10.9f, 30.9f, 6.6f, 38, 64),
                        new ClimateData(Months.June, 15.8f, 34.6f, 11.8f, 64,
                            66),
                        new ClimateData(Months.July, 18.1f, 35.3f, 14.4f, 78,
                            69),
                        new ClimateData(Months.August, 16.4f, 37.1f, 13, 77, 74),
                        new ClimateData(Months.September, 11.0f, 30.4f, 8.1f,
                            67, 79),
                        new ClimateData(Months.October, 5.6f, 21.0f, 3.4f, 65,
                            82),
                        new ClimateData(Months.November, -0.1f, 12.3f, -2.1f,
                            56, 85),
                        new ClimateData(Months.December, -3.9f, 10.9f, -6.4f,
                            49, 86)
                    };
                    break;
                case ClimateClassification.Dry:
                    _averageTemperatures = new List<ClimateData>
                    {
                        new ClimateData(Months.January, 27, 33, 21, 102, 70),
                        new ClimateData(Months.February, 26.5f, 33, 20, 88, 70),
                        new ClimateData(Months.March, 26, 33, 19, 46, 68),
                        new ClimateData(Months.April, 24, 32, 16, 19, 65),
                        new ClimateData(Months.May, 20.5f, 29, 12, 10, 61),
                        new ClimateData(Months.June, 18, 27, 9, 1, 54),
                        new ClimateData(Months.July, 17.5f, 27, 9, 1, 56),
                        new ClimateData(Months.August, 21, 30, 12, 4, 60),
                        new ClimateData(Months.September, 25, 34, 16, 5, 66),
                        new ClimateData(Months.October, 28, 36, 20, 13, 69),
                        new ClimateData(Months.November, 28, 35, 21, 46, 71),
                        new ClimateData(Months.December, 27.5f, 34, 21, 67, 69)
                    };
                    break;
                case ClimateClassification.Tropical:
                    _averageTemperatures = new List<ClimateData>
                    {
                        new ClimateData(Months.January, 12, 18, 7, 40, 73),
                        new ClimateData(Months.February, 13, 18, 6, 50, 74),
                        new ClimateData(Months.March, 13, 18, 7, 80, 77),
                        new ClimateData(Months.April, 13, 19, 8, 110, 78),
                        new ClimateData(Months.May, 13, 18, 8, 100, 79),
                        new ClimateData(Months.June, 13, 18, 8, 60, 72),
                        new ClimateData(Months.July, 13, 17, 8, 40, 70),
                        new ClimateData(Months.August, 12, 17, 8, 40, 63),
                        new ClimateData(Months.September, 12, 17, 7, 50, 62),
                        new ClimateData(Months.October, 13, 18, 8, 140, 55),
                        new ClimateData(Months.November, 13, 18, 8, 110, 55),
                        new ClimateData(Months.December, 12, 18, 7, 60, 53)
                    };
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public ClimateClassification ClimateClassificationType { get; }

        public float DisasterChance { get; private set; }

        public int InsideTemperature { get; private set; }

        public IEnumerable<ClimateData> AverageTemperatures
        {
            get { return _averageTemperatures; }
        }

        public WeatherCondition CurrentWeather { get; private set; }

        public int GrassAvaliable { get; private set; }

        public int OutsideTemperature { get; private set; }

        public float InsideHumidity { get; private set; }

        public float OutsideHumidity { get; private set; }

        public double NextWeatherChance { get; private set; }

        public void TickClimate()
        {
            // TODO: Fire off events for weather related events so this simulation will directly affect the simulation.

            var possibleClimate = GetTemperatureByMonth(GameSimulationApp.Instance.Time.CurrentMonth);
            var possibleTemperature = GameSimulationApp.Instance.Random.Next((int) possibleClimate.MeanDailyMin,
                (int) possibleClimate.MeanDailyMax);

            // Make it so climate doesn't change every single day (ex. 4 days of clear skies, 2 of rain).
            var someRandom = GameSimulationApp.Instance.Random.NextDouble();
            if (someRandom > NextWeatherChance)
            {
                return;
            }

            // If generated temp is greater than average for this month we consider this a good day!
            OutsideTemperature = possibleTemperature;
            OutsideHumidity = possibleClimate.AverageDailyHumidity;
            if (possibleTemperature > possibleClimate.AverageDailyTemperature)
            {
                // Determine if this should be a very hot day or not for the region.
                if (GameSimulationApp.Instance.Random.NextBool())
                {
                    // It was a very hot day!
                    switch (GameSimulationApp.Instance.Random.Next(5))
                    {
                        case 0:
                            CurrentWeather = WeatherCondition.Clear;
                            NextWeatherChance = 0.10d;
                            break;
                        case 1:
                            CurrentWeather = WeatherCondition.MostlySunny;
                            NextWeatherChance = 0.25d;
                            break;
                        case 2:
                            CurrentWeather = WeatherCondition.PartlySunny;
                            NextWeatherChance = 0.35d;
                            break;
                        case 3:
                            CurrentWeather = WeatherCondition.Sunny;
                            NextWeatherChance = 0.60d;
                            break;
                        case 4:
                            CurrentWeather = WeatherCondition.ChanceOfTStorm;
                            NextWeatherChance = 0.30d;
                            DisasterChance = (float) GameSimulationApp.Instance.Random.NextDouble();
                            break;
                        case 5:
                            CurrentWeather = WeatherCondition.ChanceOfRain;
                            NextWeatherChance = 0.33d;
                            DisasterChance = (float) GameSimulationApp.Instance.Random.NextDouble();
                            break;
                    }
                }
                else
                {
                    // It was a nice day outside!
                    switch (GameSimulationApp.Instance.Random.Next(5))
                    {
                        case 0:
                            CurrentWeather = WeatherCondition.Clear;
                            NextWeatherChance = 0.28d;
                            break;
                        case 1:
                            CurrentWeather = WeatherCondition.MostlySunny;
                            NextWeatherChance = 0.35d;
                            break;
                        case 2:
                            CurrentWeather = WeatherCondition.PartlySunny;
                            NextWeatherChance = 0.28d;
                            break;
                        case 3:
                            CurrentWeather = WeatherCondition.Sunny;
                            NextWeatherChance = 0.24d;
                            break;
                        case 4:
                            CurrentWeather = WeatherCondition.ChanceOfTStorm;
                            NextWeatherChance = 0.60d;
                            DisasterChance = (float) GameSimulationApp.Instance.Random.NextDouble();
                            break;
                        case 5:
                            CurrentWeather = WeatherCondition.ChanceOfRain;
                            NextWeatherChance = 0.56d;
                            DisasterChance = (float) GameSimulationApp.Instance.Random.NextDouble();
                            break;
                    }
                }
            }
            else
            {
                // It was a bad day outside!
                if (possibleClimate.MeanMonthlyRainfall > GameSimulationApp.Instance.Random.NextDouble())
                {
                    // Terrible weather!
                    switch (GameSimulationApp.Instance.Random.Next(8))
                    {
                        case 0:
                            CurrentWeather = WeatherCondition.ScatteredThunderstroms;
                            NextWeatherChance = 0.90d;
                            DisasterChance = (float) GameSimulationApp.Instance.Random.NextDouble();
                            break;
                        case 1:
                            CurrentWeather = WeatherCondition.ScatteredShowers;
                            NextWeatherChance = 0.85d;
                            DisasterChance = (float) GameSimulationApp.Instance.Random.NextDouble();
                            break;
                        case 2:
                            CurrentWeather = WeatherCondition.MostlySunny;
                            NextWeatherChance = 0.50d;
                            break;
                        case 3:
                            CurrentWeather = WeatherCondition.Thunderstorm;
                            NextWeatherChance = 0.90d;
                            DisasterChance = (float) GameSimulationApp.Instance.Random.NextDouble();
                            break;
                        case 4:
                            CurrentWeather = WeatherCondition.Haze;
                            NextWeatherChance = 0.70d;
                            break;
                        case 5:
                            CurrentWeather = WeatherCondition.Fog;
                            NextWeatherChance = 0.78d;
                            break;
                        case 6:
                            CurrentWeather = WeatherCondition.Rain;
                            NextWeatherChance = 0.56d;
                            break;
                        case 7:
                            CurrentWeather = WeatherCondition.Overcast;
                            NextWeatherChance = 0.42d;
                            break;
                        case 8:
                            CurrentWeather = WeatherCondition.Cloudy;
                            NextWeatherChance = 0.30d;
                            break;
                    }
                }
                else
                {
                    // Very cold bad day!
                    switch (GameSimulationApp.Instance.Random.Next(5))
                    {
                        case 0:
                            CurrentWeather = WeatherCondition.Flurries;
                            NextWeatherChance = 0.80d;
                            break;
                        case 1:
                            CurrentWeather = WeatherCondition.SnowShowers;
                            NextWeatherChance = 0.85d;
                            break;
                        case 2:
                            CurrentWeather = WeatherCondition.Snow;
                            NextWeatherChance = 0.75d;
                            break;
                        case 3:
                            CurrentWeather = WeatherCondition.Sleet;
                            NextWeatherChance = 0.90d;
                            break;
                        case 4:
                            CurrentWeather = WeatherCondition.Hail;
                            NextWeatherChance = 0.95d;
                            break;
                        case 5:
                            CurrentWeather = WeatherCondition.Storm;
                            NextWeatherChance = 0.85d;
                            DisasterChance = (float) GameSimulationApp.Instance.Random.NextDouble();
                            break;
                    }
                }

                // If temp is above 10 and there is snow convert it to rain.
                if (OutsideTemperature > 10)
                {
                    if (CurrentWeather == WeatherCondition.Hail ||
                        CurrentWeather == WeatherCondition.LightSnow ||
                        CurrentWeather == WeatherCondition.Flurries ||
                        CurrentWeather == WeatherCondition.SnowShowers ||
                        CurrentWeather == WeatherCondition.Icy ||
                        CurrentWeather == WeatherCondition.Snow ||
                        CurrentWeather == WeatherCondition.Sleet ||
                        CurrentWeather == WeatherCondition.FreezingDrizzle)
                    {
                        // Randomly select another type to replace it with because of temp being to high!
                        switch (GameSimulationApp.Instance.Random.Next(5))
                        {
                            case 0:
                                CurrentWeather = WeatherCondition.Clear;
                                NextWeatherChance = 0.30d;
                                break;
                            case 1:
                                CurrentWeather = WeatherCondition.MostlySunny;
                                NextWeatherChance = 0.25d;
                                break;
                            case 2:
                                CurrentWeather = WeatherCondition.PartlySunny;
                                NextWeatherChance = 0.42d;
                                break;
                            case 3:
                                CurrentWeather = WeatherCondition.Sunny;
                                NextWeatherChance = 0.33d;
                                break;
                            case 4:
                                CurrentWeather = WeatherCondition.ChanceOfTStorm;
                                NextWeatherChance = 0.45d;
                                DisasterChance = (float) GameSimulationApp.Instance.Random.NextDouble();
                                break;
                            case 5:
                                CurrentWeather = WeatherCondition.ChanceOfRain;
                                NextWeatherChance = 0.55d;
                                DisasterChance = (float) GameSimulationApp.Instance.Random.NextDouble();
                                break;
                        }
                    }
                }
            }

            // Adjust temperature levels.
            if (InsideTemperature > OutsideTemperature)
            {
                if (ClimateClassificationType == ClimateClassification.Polar)
                {
                    // Polar regions get a bonus for heat reduction.
                    InsideTemperature -= GameSimulationApp.Instance.Random.Next(1, 3);
                }
                else
                {
                    // Everything else just gets 1 degree per tick.
                    InsideTemperature--;
                }
            }
            else if (InsideTemperature < OutsideTemperature)
            {
                // Inside temp is less than outside temp!
                InsideTemperature++;
            }

            // Adjust humidity levels.
            if (InsideHumidity > OutsideHumidity)
            {
                if (ClimateClassificationType == ClimateClassification.Polar)
                {
                    // Polar regions get a bonus for heat reduction.
                    InsideHumidity -= 0.2f;
                }
                else
                {
                    // Everything else just gets 1 degree per tick.
                    InsideHumidity -= 0.1f;
                }
            }
            else if (InsideHumidity < OutsideHumidity)
            {
                // Inside temp is less than outside temp!
                InsideHumidity += 0.1f;
            }
        }

        /// <summary>
        ///     Returns average temperature for given climate classification and month.
        /// </summary>
        private ClimateData GetTemperatureByMonth(Months whichMonth)
        {
            foreach (var data in AverageTemperatures)
            {
                if (data.ClimateMonth == whichMonth) return data;
            }
            return null;
        }

        /// <summary>
        ///     Fired when the simulation is closing and needs to clear out any data structures that it created so the program can
        ///     exit cleanly.
        /// </summary>
        public override void Destroy()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Fired when the simulation ticks the module that it created inside of itself.
        /// </summary>
        public override void Tick()
        {
            throw new NotImplementedException();
        }
    }
}