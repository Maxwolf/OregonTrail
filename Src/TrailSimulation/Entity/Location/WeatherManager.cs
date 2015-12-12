using System;
using System.Collections.Generic;
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
        ///     Contains of all the average temperatures that we loaded from the static climate registry.
        /// </summary>
        private List<ClimateData> _averageTemperatures;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailSimulation.Core.ModuleProduct" /> class.
        /// </summary>
        public WeatherManager(Climate climateType)
        {
            // Sets up the climate type which this weather manager is responsible for ticking.
            ClimateType = climateType;

            // Select climate and determine humidity and temperature based on it.
            switch (ClimateType)
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

        public Climate ClimateType { get; set; }

        public float DisasterChance { get; private set; }

        public int InsideTemperature { get; private set; }

        public IEnumerable<ClimateData> AverageTemperatures
        {
            get { return _averageTemperatures; }
        }

        public WeatherCondition Condition { get; private set; }

        public int GrassAvaliable { get; private set; }

        public int OutsideTemperature { get; private set; }

        public float InsideHumidity { get; private set; }

        public float OutsideHumidity { get; private set; }

        public double NextWeatherChance { get; private set; }

        /// <summary>
        ///     Processes the weather based on climate type, and fires off weather related events so this module and thus weather
        ///     will affect the simulation.
        /// </summary>
        public void Tick()
        {
            // TODO: Fire off weather related events so this module and thus weather will affect the simulation.

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
                    HotDay();
                }
                else
                {
                    NiceDay();
                }
            }
            else
            {
                // It was a bad day outside!
                if (possibleClimate.MeanMonthlyRainfall > GameSimulationApp.Instance.Random.NextDouble())
                {
                    RainyDay();
                }
                else
                {
                    ColdDay();
                }

                // If temp is above 10 and there is snow convert it to rain.
                ConvertSnowIntoRain();
            }

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
                if (ClimateType == Climate.Polar)
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
                if (ClimateType == Climate.Polar)
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
                    NextWeatherChance = 0.30d;
                    break;
                case 1:
                    Condition = WeatherCondition.MostlySunny;
                    NextWeatherChance = 0.25d;
                    break;
                case 2:
                    Condition = WeatherCondition.PartlySunny;
                    NextWeatherChance = 0.42d;
                    break;
                case 3:
                    Condition = WeatherCondition.Sunny;
                    NextWeatherChance = 0.33d;
                    break;
                case 4:
                    Condition = WeatherCondition.ChanceOfThunderstorm;
                    NextWeatherChance = 0.45d;
                    DisasterChance = (float) GameSimulationApp.Instance.Random.NextDouble();
                    break;
                case 5:
                    Condition = WeatherCondition.ChanceOfRain;
                    NextWeatherChance = 0.55d;
                    DisasterChance = (float) GameSimulationApp.Instance.Random.NextDouble();
                    break;
                default:
                    Condition = WeatherCondition.Clear;
                    NextWeatherChance = 0.30d;
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
                    NextWeatherChance = 0.80d;
                    break;
                case 1:
                    Condition = WeatherCondition.SnowShowers;
                    NextWeatherChance = 0.85d;
                    break;
                case 2:
                    Condition = WeatherCondition.Snow;
                    NextWeatherChance = 0.75d;
                    break;
                case 3:
                    Condition = WeatherCondition.Sleet;
                    NextWeatherChance = 0.90d;
                    break;
                case 4:
                    Condition = WeatherCondition.Hail;
                    NextWeatherChance = 0.95d;
                    break;
                case 5:
                    Condition = WeatherCondition.Storm;
                    NextWeatherChance = 0.85d;
                    DisasterChance = (float) GameSimulationApp.Instance.Random.NextDouble();
                    break;
                default:
                    Condition = WeatherCondition.Snow;
                    NextWeatherChance = 0.75d;
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
                    NextWeatherChance = 0.90d;
                    DisasterChance = (float) GameSimulationApp.Instance.Random.NextDouble();
                    break;
                case 1:
                    Condition = WeatherCondition.ScatteredShowers;
                    NextWeatherChance = 0.85d;
                    DisasterChance = (float) GameSimulationApp.Instance.Random.NextDouble();
                    break;
                case 2:
                    Condition = WeatherCondition.MostlySunny;
                    NextWeatherChance = 0.50d;
                    break;
                case 3:
                    Condition = WeatherCondition.Thunderstorm;
                    NextWeatherChance = 0.90d;
                    DisasterChance = (float) GameSimulationApp.Instance.Random.NextDouble();
                    break;
                case 4:
                    Condition = WeatherCondition.Haze;
                    NextWeatherChance = 0.70d;
                    break;
                case 5:
                    Condition = WeatherCondition.Fog;
                    NextWeatherChance = 0.78d;
                    break;
                case 6:
                    Condition = WeatherCondition.Rain;
                    NextWeatherChance = 0.56d;
                    break;
                case 7:
                    Condition = WeatherCondition.Overcast;
                    NextWeatherChance = 0.42d;
                    break;
                case 8:
                    Condition = WeatherCondition.Cloudy;
                    NextWeatherChance = 0.30d;
                    break;
                default:
                    Condition = WeatherCondition.MostlySunny;
                    NextWeatherChance = 0.50d;
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
                    NextWeatherChance = 0.28d;
                    break;
                case 1:
                    Condition = WeatherCondition.MostlySunny;
                    NextWeatherChance = 0.35d;
                    break;
                case 2:
                    Condition = WeatherCondition.PartlySunny;
                    NextWeatherChance = 0.28d;
                    break;
                case 3:
                    Condition = WeatherCondition.Sunny;
                    NextWeatherChance = 0.24d;
                    break;
                case 4:
                    Condition = WeatherCondition.ChanceOfThunderstorm;
                    NextWeatherChance = 0.60d;
                    DisasterChance = (float) GameSimulationApp.Instance.Random.NextDouble();
                    break;
                case 5:
                    Condition = WeatherCondition.ChanceOfRain;
                    NextWeatherChance = 0.56d;
                    DisasterChance = (float) GameSimulationApp.Instance.Random.NextDouble();
                    break;
                default:
                    Condition = WeatherCondition.Clear;
                    NextWeatherChance = 0.28d;
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
                    NextWeatherChance = 0.10d;
                    break;
                case 1:
                    Condition = WeatherCondition.MostlySunny;
                    NextWeatherChance = 0.25d;
                    break;
                case 2:
                    Condition = WeatherCondition.PartlySunny;
                    NextWeatherChance = 0.35d;
                    break;
                case 3:
                    Condition = WeatherCondition.Sunny;
                    NextWeatherChance = 0.60d;
                    break;
                case 4:
                    Condition = WeatherCondition.ChanceOfThunderstorm;
                    NextWeatherChance = 0.30d;
                    DisasterChance = (float) GameSimulationApp.Instance.Random.NextDouble();
                    break;
                case 5:
                    Condition = WeatherCondition.ChanceOfRain;
                    NextWeatherChance = 0.33d;
                    DisasterChance = (float) GameSimulationApp.Instance.Random.NextDouble();
                    break;
                default:
                    Condition = WeatherCondition.Clear;
                    NextWeatherChance = 0.10d;
                    break;
            }
        }

        /// <summary>
        ///     Returns average temperature for given climate classification and month.
        /// </summary>
        private ClimateData GetTemperatureByMonth(Month whichMonth)
        {
            foreach (var data in AverageTemperatures)
            {
                if (data.ClimateMonth == whichMonth) return data;
            }
            return null;
        }
    }
}