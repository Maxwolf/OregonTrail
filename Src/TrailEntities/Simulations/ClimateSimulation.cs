using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using TrailCommon;

namespace TrailEntities
{
    public class ClimateSimulation : IClimateSimulation
    {
        private double _nextWeatherChance;
        private float _outsideHumidity;
        private float _insideHumidity;
        private int _outsideTemperature;
        private uint _grassAvaliable;
        private WeatherCondition _currentWeather;
        private List<ClimateData> _averageTemperatures;
        private int _insideTemperature;
        protected IGameSimulation _game;
        private float _disasterChance;
        private ClimateClassification _climateClassificationType;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:TrailEntities.ClimateSimulation"/> class.
        /// </summary>
        public ClimateSimulation(IGameSimulation simulationReference, ClimateClassification climateClassificationType)
        {
            _game = simulationReference;
            _climateClassificationType = climateClassificationType;

            // Select climate and determine humidity and temperature based on it.
            switch (_climateClassificationType)
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

        public void UpdateClimate()
        {
            _grassAvaliable = 42;
        }

        public ClimateClassification ClimateClassificationType
        {
            get { return _climateClassificationType; }
        }

        public float DisasterChance
        {
            get { return _disasterChance; }
        }

        public int InsideTemperature
        {
            get { return _insideTemperature; }
        }

        public ReadOnlyCollection<ClimateData> AverageTemperatures
        {
            get { return new ReadOnlyCollection<ClimateData>(_averageTemperatures); }
        }

        public WeatherCondition CurrentWeather
        {
            get { return _currentWeather; }
        }

        public uint GrassAvaliable
        {
            get { return _grassAvaliable; }
        }

        public int OutsideTemperature
        {
            get { return _outsideTemperature; }
        }

        public float InsideHumidity
        {
            get { return _insideHumidity; }
        }

        public float OutsideHumidity
        {
            get { return _outsideHumidity; }
        }

        public double NextWeatherChance
        {
            get { return _nextWeatherChance; }
        }

        /// <summary>
        /// Returns average temperature for given climate classification and month.
        /// </summary>
        private ClimateData GetTemperatureByMonth(Months whichMonth)
        {
            foreach (ClimateData data in AverageTemperatures)
            {
                if (data.ClimateMonth == whichMonth) return data;
            }
            return null;
        }

        public void TickClimate()
        {
            ClimateData possibleClimate = GetTemperatureByMonth(_game.Time.CurrentMonth);
            int possibleTemperature = _game.Random.Next((int) possibleClimate.MeanDailyMin, (int) possibleClimate.MeanDailyMax);

            // Make it so climate doesn't change every single day (ex. 4 days of clear skies, 2 of rain).
            double someRandom = _game.Random.NextDouble();
            if (someRandom > NextWeatherChance)
            {
                return;
            }

            // If generated temp is greater than average for this month we consider this a good day!
            _outsideTemperature = possibleTemperature;
            _outsideHumidity = possibleClimate.AverageDailyHumidity;
            if (possibleTemperature > possibleClimate.AverageDailyTemperature)
            {
                // Determine if this should be a very hot day or not for the region.
                if (_game.Random.GetRandomBoolean())
                {
                    // It was a very hot day!
                    switch (_game.Random.Next(5))
                    {
                        case 0:
                            _currentWeather = WeatherCondition.Clear;
                            _nextWeatherChance = 0.10d;
                            break;
                        case 1:
                            _currentWeather = WeatherCondition.MostlySunny;
                            _nextWeatherChance = 0.25d;
                            break;
                        case 2:
                            _currentWeather = WeatherCondition.PartlySunny;
                            _nextWeatherChance = 0.35d;
                            break;
                        case 3:
                            _currentWeather = WeatherCondition.Sunny;
                            _nextWeatherChance = 0.60d;
                            break;
                        case 4:
                            _currentWeather = WeatherCondition.ChanceOfTStorm;
                            _nextWeatherChance = 0.30d;
                            _disasterChance = (float) _game.Random.NextDouble();
                            break;
                        case 5:
                            _currentWeather = WeatherCondition.ChanceOfRain;
                            _nextWeatherChance = 0.33d;
                            _disasterChance = (float) _game.Random.NextDouble();
                            break;
                    }
                }
                else
                {
                    // It was a nice day outside!
                    switch (_game.Random.Next(5))
                    {
                        case 0:
                            _currentWeather = WeatherCondition.Clear;
                            _nextWeatherChance = 0.28d;
                            break;
                        case 1:
                            _currentWeather = WeatherCondition.MostlySunny;
                            _nextWeatherChance = 0.35d;
                            break;
                        case 2:
                            _currentWeather = WeatherCondition.PartlySunny;
                            _nextWeatherChance = 0.28d;
                            break;
                        case 3:
                            _currentWeather = WeatherCondition.Sunny;
                            _nextWeatherChance = 0.24d;
                            break;
                        case 4:
                            _currentWeather = WeatherCondition.ChanceOfTStorm;
                            _nextWeatherChance = 0.60d;
                            _disasterChance = (float) _game.Random.NextDouble();
                            break;
                        case 5:
                            _currentWeather = WeatherCondition.ChanceOfRain;
                            _nextWeatherChance = 0.56d;
                            _disasterChance = (float) _game.Random.NextDouble();
                            break;
                    }
                }
            }
            else
            {
                // It was a bad day outside!
                if (possibleClimate.MeanMonthlyRainfall > _game.Random.NextDouble())
                {
                    // Terrible weather!
                    switch (_game.Random.Next(8))
                    {
                        case 0:
                            _currentWeather = WeatherCondition.ScatteredThunderstroms;
                            _nextWeatherChance = 0.90d;
                            _disasterChance = (float) _game.Random.NextDouble();
                            break;
                        case 1:
                            _currentWeather = WeatherCondition.ScatteredShowers;
                            _nextWeatherChance = 0.85d;
                            _disasterChance = (float) _game.Random.NextDouble();
                            break;
                        case 2:
                            _currentWeather = WeatherCondition.MostlySunny;
                            _nextWeatherChance = 0.50d;
                            break;
                        case 3:
                            _currentWeather = WeatherCondition.Thunderstorm;
                            _nextWeatherChance = 0.90d;
                            _disasterChance = (float) _game.Random.NextDouble();
                            break;
                        case 4:
                            _currentWeather = WeatherCondition.Haze;
                            _nextWeatherChance = 0.70d;
                            break;
                        case 5:
                            _currentWeather = WeatherCondition.Fog;
                            _nextWeatherChance = 0.78d;
                            break;
                        case 6:
                            _currentWeather = WeatherCondition.Rain;
                            _nextWeatherChance = 0.56d;
                            break;
                        case 7:
                            _currentWeather = WeatherCondition.Overcast;
                            _nextWeatherChance = 0.42d;
                            break;
                        case 8:
                            _currentWeather = WeatherCondition.Cloudy;
                            _nextWeatherChance = 0.30d;
                            break;
                    }
                }
                else
                {
                    // Very cold bad day!
                    switch (_game.Random.Next(5))
                    {
                        case 0:
                            _currentWeather = WeatherCondition.Flurries;
                            _nextWeatherChance = 0.80d;
                            break;
                        case 1:
                            _currentWeather = WeatherCondition.SnowShowers;
                            _nextWeatherChance = 0.85d;
                            break;
                        case 2:
                            _currentWeather = WeatherCondition.Snow;
                            _nextWeatherChance = 0.75d;
                            break;
                        case 3:
                            _currentWeather = WeatherCondition.Sleet;
                            _nextWeatherChance = 0.90d;
                            break;
                        case 4:
                            _currentWeather = WeatherCondition.Hail;
                            _nextWeatherChance = 0.95d;
                            break;
                        case 5:
                            _currentWeather = WeatherCondition.Storm;
                            _nextWeatherChance = 0.85d;
                            _disasterChance = (float) _game.Random.NextDouble();
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
                        switch (_game.Random.Next(5))
                        {
                            case 0:
                                _currentWeather = WeatherCondition.Clear;
                                _nextWeatherChance = 0.30d;
                                break;
                            case 1:
                                _currentWeather = WeatherCondition.MostlySunny;
                                _nextWeatherChance = 0.25d;
                                break;
                            case 2:
                                _currentWeather = WeatherCondition.PartlySunny;
                                _nextWeatherChance = 0.42d;
                                break;
                            case 3:
                                _currentWeather = WeatherCondition.Sunny;
                                _nextWeatherChance = 0.33d;
                                break;
                            case 4:
                                _currentWeather = WeatherCondition.ChanceOfTStorm;
                                _nextWeatherChance = 0.45d;
                                _disasterChance = (float) _game.Random.NextDouble();
                                break;
                            case 5:
                                _currentWeather = WeatherCondition.ChanceOfRain;
                                _nextWeatherChance = 0.55d;
                                _disasterChance = (float) _game.Random.NextDouble();
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
                    _insideTemperature -= _game.Random.Next(1, 3);
                }
                else
                {
                    // Everything else just gets 1 degree per tick.
                    _insideTemperature--;
                }
            }
            else if (InsideTemperature < OutsideTemperature)
            {
                // Inside temp is less than outside temp!
                _insideTemperature++;
            }

            // Adjust humidity levels.
            if (InsideHumidity > OutsideHumidity)
            {
                if (ClimateClassificationType == ClimateClassification.Polar)
                {
                    // Polar regions get a bonus for heat reduction.
                    _insideHumidity -= 0.2f;
                }
                else
                {
                    // Everything else just gets 1 degree per tick.
                    _insideHumidity -= 0.1f;
                }
            }
            else if (InsideHumidity < OutsideHumidity)
            {
                // Inside temp is less than outside temp!
                _insideHumidity += 0.1f;
            }
        }
    }
}
