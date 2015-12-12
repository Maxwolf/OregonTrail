using System;
using System.Collections.Generic;
using TrailSimulation.Event;
using TrailSimulation.Game;

namespace TrailSimulation.Entity
{
    /// <summary>
    ///     Controls the weather, temperature, environment for getting food, illness probability, and various other factors
    ///     related to the players current location in the game world.
    /// </summary>
    public sealed class WeatherManager
    {
        /// <summary>
        ///     Defines the type of climate this weather manager is currently simulating.
        /// </summary>
        private readonly Climate _climateType;

        /// <summary>
        ///     Contains of all the average temperatures that we loaded from the static climate registry.
        /// </summary>
        private List<ClimateData> _averageTemperatures;

        /// <summary>
        ///     Chance that a weather event will occur such as storm, tornado, blizzard, etc.
        /// </summary>
        private double _disasterChance;

        /// <summary>
        ///     Next change that the weather might change.
        /// </summary>
        private double _nextWeatherChance;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailSimulation.Core.ModuleProduct" /> class.
        /// </summary>
        public WeatherManager(Climate climateType)
        {
            // Sets up the climate type which this weather manager is responsible for ticking.
            _climateType = climateType;

            // Select climate and determine humidity and temperature based on it.
            switch (_climateType)
            {
                case Climate.Polar:
                    _averageTemperatures = new List<ClimateData>(ClimateRegistry.Polar);
                    break;
                case Climate.Continental:
                    _averageTemperatures = new List<ClimateData>(ClimateRegistry.Continental);
                    break;
                case Climate.Moderate:
                    _averageTemperatures = new List<ClimateData>(ClimateRegistry.Moderate);
                    break;
                case Climate.Dry:
                    _averageTemperatures = new List<ClimateData>(ClimateRegistry.Dry);
                    break;
                case Climate.Tropical:
                    _averageTemperatures = new List<ClimateData>(ClimateRegistry.Tropical);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        ///     Current temperature in Celsius for inside a building with four walls and a roof.
        /// </summary>
        public int InsideTemperature { get; private set; }

        /// <summary>
        ///     Current weather condition this location is experiencing.
        /// </summary>
        public WeatherCondition Condition { get; private set; }

        /// <summary>
        ///     Current temperature in Celsius for outside a building, exposed to the elements.
        /// </summary>
        public int OutsideTemperature { get; private set; }

        /// <summary>
        ///     Current humidity for inside a building with four walls and a roof.
        /// </summary>
        public float InsideHumidity { get; private set; }

        /// <summary>
        ///     Current humidity for outside a building, exposed to the elements.
        /// </summary>
        public float OutsideHumidity { get; private set; }

        /// <summary>
        ///     Processes the weather based on climate type, and fires off weather related events so this module and thus weather
        ///     will affect the simulation.
        /// </summary>
        public void Tick()
        {
            // Grab instance of the game simulation.
            var game = GameSimulationApp.Instance;

            // Fire off weather related events so this module and thus weather will affect the simulation.
            if (_disasterChance > 0 && game.Random.NextDouble() >= _disasterChance)
            {
                game.EventDirector.TriggerEventByType(game.Trail.CurrentLocation, EventCategory.Weather);

                // Resets the disaster chance after firing event for it.
                _disasterChance = 0;
                return;
            }

            var possibleClimate = GetTemperatureByMonth(game.Time.CurrentMonth);
            var possibleTemperature = game.Random.Next((int) possibleClimate.MeanDailyMin,
                (int) possibleClimate.MeanDailyMax);

            // Make it so climate doesn't change every single day (ex. 4 days of clear skies, 2 of rain).
            var someRandom = game.Random.NextDouble();
            if (someRandom > _nextWeatherChance)
                return;

            // If generated temp is greater than average for this month we consider this a good day!
            OutsideTemperature = possibleTemperature;
            OutsideHumidity = possibleClimate.AverageDailyHumidity;
            if (possibleTemperature > possibleClimate.AverageDailyTemperature)
            {
                // Determine if this should be a very hot day or not for the region.
                if (game.Random.NextBool())
                    HotDay();
                else
                    NiceDay();
            }
            else
            {
                // It was a bad day outside!
                if (possibleClimate.MeanMonthlyRainfall > game.Random.NextDouble())
                    RainyDay();
                else
                    ColdDay();

                // If temp is above 10 and there is snow convert it to rain.
                ConvertSnowIntoRain();
            }

            // Adjust both temperature and humidity.
            AdjustTemperature();
            AdjustHumidity();
        }

        /// <summary>
        ///     Adjust humidity levels.
        /// </summary>
        private void AdjustHumidity()
        {
            if (InsideHumidity > OutsideHumidity)
            {
                if (_climateType == Climate.Polar)
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
        ///     Adjust temperature levels.
        /// </summary>
        private void AdjustTemperature()
        {
            if (InsideTemperature > OutsideTemperature)
            {
                if (_climateType == Climate.Polar)
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
        }

        /// <summary>
        ///     If temp is above 10 and there is snow convert it to rain.
        /// </summary>
        private void ConvertSnowIntoRain()
        {
            if (OutsideTemperature <= 10 ||
                (Condition != WeatherCondition.Hail && Condition != WeatherCondition.LightSnow &&
                 Condition != WeatherCondition.Flurries && Condition != WeatherCondition.SnowShowers &&
                 Condition != WeatherCondition.Icy && Condition != WeatherCondition.Snow &&
                 Condition != WeatherCondition.Sleet && Condition != WeatherCondition.FreezingDrizzle))
                return;

            // Randomly select another type to replace it with because of temp being to high!
            switch (GameSimulationApp.Instance.Random.Next(5))
            {
                case 0:
                    Condition = WeatherCondition.Clear;
                    _nextWeatherChance = 0.30d;
                    break;
                case 1:
                    Condition = WeatherCondition.MostlySunny;
                    _nextWeatherChance = 0.25d;
                    break;
                case 2:
                    Condition = WeatherCondition.PartlySunny;
                    _nextWeatherChance = 0.42d;
                    break;
                case 3:
                    Condition = WeatherCondition.Sunny;
                    _nextWeatherChance = 0.33d;
                    break;
                case 4:
                    Condition = WeatherCondition.ChanceOfThunderstorm;
                    _nextWeatherChance = 0.45d;
                    _disasterChance = GameSimulationApp.Instance.Random.NextDouble();
                    break;
                case 5:
                    Condition = WeatherCondition.ChanceOfRain;
                    _nextWeatherChance = 0.55d;
                    _disasterChance = GameSimulationApp.Instance.Random.NextDouble();
                    break;
                default:
                    Condition = WeatherCondition.Clear;
                    _nextWeatherChance = 0.30d;
                    break;
            }
        }

        /// <summary>
        ///     Very cold bad day!
        /// </summary>
        private void ColdDay()
        {
            switch (GameSimulationApp.Instance.Random.Next(5))
            {
                case 0:
                    Condition = WeatherCondition.Flurries;
                    _nextWeatherChance = 0.80d;
                    break;
                case 1:
                    Condition = WeatherCondition.SnowShowers;
                    _nextWeatherChance = 0.85d;
                    break;
                case 2:
                    Condition = WeatherCondition.Snow;
                    _nextWeatherChance = 0.75d;
                    break;
                case 3:
                    Condition = WeatherCondition.Sleet;
                    _nextWeatherChance = 0.90d;
                    break;
                case 4:
                    Condition = WeatherCondition.Hail;
                    _nextWeatherChance = 0.95d;
                    break;
                case 5:
                    Condition = WeatherCondition.Storm;
                    _nextWeatherChance = 0.85d;
                    _disasterChance = (float) GameSimulationApp.Instance.Random.NextDouble();
                    break;
                default:
                    Condition = WeatherCondition.Snow;
                    _nextWeatherChance = 0.75d;
                    break;
            }
        }

        /// <summary>
        ///     Terrible weather!
        /// </summary>
        private void RainyDay()
        {
            switch (GameSimulationApp.Instance.Random.Next(8))
            {
                case 0:
                    Condition = WeatherCondition.ScatteredThunderstorms;
                    _nextWeatherChance = 0.90d;
                    _disasterChance = (float) GameSimulationApp.Instance.Random.NextDouble();
                    break;
                case 1:
                    Condition = WeatherCondition.ScatteredShowers;
                    _nextWeatherChance = 0.85d;
                    _disasterChance = (float) GameSimulationApp.Instance.Random.NextDouble();
                    break;
                case 2:
                    Condition = WeatherCondition.MostlySunny;
                    _nextWeatherChance = 0.50d;
                    break;
                case 3:
                    Condition = WeatherCondition.Thunderstorm;
                    _nextWeatherChance = 0.90d;
                    _disasterChance = (float) GameSimulationApp.Instance.Random.NextDouble();
                    break;
                case 4:
                    Condition = WeatherCondition.Haze;
                    _nextWeatherChance = 0.70d;
                    break;
                case 5:
                    Condition = WeatherCondition.Fog;
                    _nextWeatherChance = 0.78d;
                    break;
                case 6:
                    Condition = WeatherCondition.Rain;
                    _nextWeatherChance = 0.56d;
                    break;
                case 7:
                    Condition = WeatherCondition.Overcast;
                    _nextWeatherChance = 0.42d;
                    break;
                case 8:
                    Condition = WeatherCondition.Cloudy;
                    _nextWeatherChance = 0.30d;
                    break;
                default:
                    Condition = WeatherCondition.MostlySunny;
                    _nextWeatherChance = 0.50d;
                    break;
            }
        }

        /// <summary>
        ///     It was a nice day outside!
        /// </summary>
        private void NiceDay()
        {
            switch (GameSimulationApp.Instance.Random.Next(5))
            {
                case 0:
                    Condition = WeatherCondition.Clear;
                    _nextWeatherChance = 0.28d;
                    break;
                case 1:
                    Condition = WeatherCondition.MostlySunny;
                    _nextWeatherChance = 0.35d;
                    break;
                case 2:
                    Condition = WeatherCondition.PartlySunny;
                    _nextWeatherChance = 0.28d;
                    break;
                case 3:
                    Condition = WeatherCondition.Sunny;
                    _nextWeatherChance = 0.24d;
                    break;
                case 4:
                    Condition = WeatherCondition.ChanceOfThunderstorm;
                    _nextWeatherChance = 0.60d;
                    _disasterChance = (float) GameSimulationApp.Instance.Random.NextDouble();
                    break;
                case 5:
                    Condition = WeatherCondition.ChanceOfRain;
                    _nextWeatherChance = 0.56d;
                    _disasterChance = (float) GameSimulationApp.Instance.Random.NextDouble();
                    break;
                default:
                    Condition = WeatherCondition.Clear;
                    _nextWeatherChance = 0.28d;
                    break;
            }
        }

        /// <summary>
        ///     It was a very hot day!
        /// </summary>
        private void HotDay()
        {
            switch (GameSimulationApp.Instance.Random.Next(5))
            {
                case 0:
                    Condition = WeatherCondition.Clear;
                    _nextWeatherChance = 0.10d;
                    break;
                case 1:
                    Condition = WeatherCondition.MostlySunny;
                    _nextWeatherChance = 0.25d;
                    break;
                case 2:
                    Condition = WeatherCondition.PartlySunny;
                    _nextWeatherChance = 0.35d;
                    break;
                case 3:
                    Condition = WeatherCondition.Sunny;
                    _nextWeatherChance = 0.60d;
                    break;
                case 4:
                    Condition = WeatherCondition.ChanceOfThunderstorm;
                    _nextWeatherChance = 0.30d;
                    _disasterChance = (float) GameSimulationApp.Instance.Random.NextDouble();
                    break;
                case 5:
                    Condition = WeatherCondition.ChanceOfRain;
                    _nextWeatherChance = 0.33d;
                    _disasterChance = (float) GameSimulationApp.Instance.Random.NextDouble();
                    break;
                default:
                    Condition = WeatherCondition.Clear;
                    _nextWeatherChance = 0.10d;
                    break;
            }
        }

        /// <summary>
        ///     Returns average temperature for given climate classification and month.
        /// </summary>
        private ClimateData GetTemperatureByMonth(Month whichMonth)
        {
            foreach (var data in _averageTemperatures)
            {
                if (data.ClimateMonth == whichMonth) return data;
            }

            return null;
        }
    }
}