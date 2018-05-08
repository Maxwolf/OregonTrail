// Created by Ron 'Maxwolf' McDowell (ron.mcdowell@gmail.com) 
// Timestamp 01/03/2016@1:50 AM

using System;
using System.Collections.Generic;
using OregonTrailDotNet.Entity.Vehicle;
using OregonTrailDotNet.Event;
using OregonTrailDotNet.Event.Weather;
using OregonTrailDotNet.Module.Time;

namespace OregonTrailDotNet.Entity.Location.Weather
{
    /// <summary>
    ///     Controls the weather, temperature, environment for getting food, illness probability, and various other factors
    ///     related to the players current location in the game world.
    /// </summary>
    public sealed class LocationWeather
    {
        /// <summary>
        ///     Defines the type of climate this weather manager is currently simulating.
        /// </summary>
        private readonly Climate _climateType;

        /// <summary>
        ///     Contains of all the average temperatures that we loaded from the static climate registry.
        /// </summary>
        private readonly List<ClimateData> _averageTemperatures;

        /// <summary>
        ///     Chance that a weather event will occur such as storm, tornado, blizzard, etc.
        /// </summary>
        private double _disasterChance;

        /// <summary>
        ///     Next change that the weather might change.
        /// </summary>
        private double _nextWeatherChance;

        /// <summary>
        ///     Initializes a new instance of the <see cref="LocationWeather" /> class.
        ///     Initializes a new instance of the <see cref="T:TrailSimulation.Core.ModuleProduct" /> class.
        /// </summary>
        /// <param name="climateType">The climate Type.</param>
        public LocationWeather(Climate climateType)
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
        public Weather Condition { get; private set; }

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
            if ((_disasterChance > 0) && (game.Random.NextDouble() >= _disasterChance))
            {
                // Only trigger weather events if the vehicle is moving and we are on the trail.
                if ((game.Vehicle.Status == VehicleStatus.Moving) &&
                    (game.Trail.CurrentLocation.Status == LocationStatus.Departed))
                    game.EventDirector.TriggerEventByType(game.Vehicle, EventCategory.Weather);

                // Resets the disaster chance after firing event for it.
                _disasterChance = 0;
                return;
            }

            var possibleClimate = GetTemperatureByMonth(game.Time.CurrentMonth);
            var possibleTemperature = game.Random.Next((int) possibleClimate.TemperatureMin,
                (int) possibleClimate.TemperatureMax);

            // Make it so climate doesn't change every single day (ex. 4 days of clear skies, 2 of rain).
            if ((_nextWeatherChance > 0) && (game.Random.NextDouble() >= _nextWeatherChance))
                return;

            // If generated temp is greater than average for this month we consider this a good day!
            OutsideTemperature = possibleTemperature;
            OutsideHumidity = possibleClimate.Humidity;
            if (possibleTemperature > possibleClimate.Temperature)
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
                if (possibleClimate.Rainfall > game.Random.NextDouble())
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
                if (_climateType == Climate.Polar)
                    InsideHumidity -= 0.2f;
                else

                    InsideHumidity -= 0.1f;
            else if (InsideHumidity < OutsideHumidity)
                InsideHumidity += 0.1f;
        }

        /// <summary>
        ///     Adjust temperature levels.
        /// </summary>
        private void AdjustTemperature()
        {
            if (InsideTemperature > OutsideTemperature)
                if (_climateType == Climate.Polar)

                    InsideTemperature -= GameSimulationApp.Instance.Random.Next(1, 3);
                else
                    InsideTemperature--;
            else if (InsideTemperature < OutsideTemperature)
                InsideTemperature++;
        }

        /// <summary>
        ///     If temp is above 10 and there is snow convert it to rain.
        /// </summary>
        private void ConvertSnowIntoRain()
        {
            if ((OutsideTemperature <= 10) ||
                ((Condition != Weather.Hail) && (Condition != Weather.LightSnow) &&
                 (Condition != Weather.Flurries) && (Condition != Weather.SnowShowers) &&
                 (Condition != Weather.Icy) && (Condition != Weather.Snow) &&
                 (Condition != Weather.Sleet) && (Condition != Weather.FreezingDrizzle)))
                return;

            // Randomly select another type to replace it with because of temp being to high!
            switch (GameSimulationApp.Instance.Random.Next(5))
            {
                case 0:
                    Condition = Weather.Clear;
                    _nextWeatherChance = 0.30d;
                    break;
                case 1:
                    Condition = Weather.MostlySunny;
                    _nextWeatherChance = 0.25d;
                    break;
                case 2:
                    Condition = Weather.PartlySunny;
                    _nextWeatherChance = 0.42d;
                    break;
                case 3:
                    Condition = Weather.Sunny;
                    _nextWeatherChance = 0.33d;
                    break;
                case 4:
                    Condition = Weather.ChanceOfThunderstorm;
                    _nextWeatherChance = 0.45d;
                    _disasterChance = GameSimulationApp.Instance.Random.NextDouble();
                    break;
                case 5:
                    Condition = Weather.ChanceOfRain;
                    _nextWeatherChance = 0.55d;
                    _disasterChance = GameSimulationApp.Instance.Random.NextDouble();
                    break;
                default:
                    Condition = Weather.Clear;
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
                    Condition = Weather.Flurries;
                    _nextWeatherChance = 0.80d;
                    break;
                case 1:
                    Condition = Weather.SnowShowers;
                    _nextWeatherChance = 0.85d;
                    break;
                case 2:
                    Condition = Weather.Snow;
                    _nextWeatherChance = 0.75d;
                    break;
                case 3:
                    Condition = Weather.Sleet;
                    _nextWeatherChance = 0.90d;
                    break;
                case 4:
                    Condition = Weather.Hail;
                    _nextWeatherChance = 0.95d;
                    if (GameSimulationApp.Instance.Random.NextBool())
                        GameSimulationApp.Instance.EventDirector.TriggerEvent(GameSimulationApp.Instance.Vehicle,
                            typeof(HailStorm));

                    break;
                case 5:
                    Condition = Weather.Storm;
                    _nextWeatherChance = 0.85d;
                    _disasterChance = (float) GameSimulationApp.Instance.Random.NextDouble();

                    if (GameSimulationApp.Instance.Random.NextBool())
                        GameSimulationApp.Instance.EventDirector.TriggerEvent(GameSimulationApp.Instance.Vehicle,
                            typeof(SevereWeather));

                    break;
                default:
                    Condition = Weather.Snow;
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
                    Condition = Weather.ScatteredThunderstorms;
                    _nextWeatherChance = 0.90d;
                    _disasterChance = (float) GameSimulationApp.Instance.Random.NextDouble();

                    if (GameSimulationApp.Instance.Random.NextBool())
                        GameSimulationApp.Instance.EventDirector.TriggerEvent(GameSimulationApp.Instance.Vehicle,
                            typeof(SevereWeather));

                    break;
                case 1:
                    Condition = Weather.ScatteredShowers;
                    _nextWeatherChance = 0.85d;
                    _disasterChance = (float) GameSimulationApp.Instance.Random.NextDouble();
                    break;
                case 2:
                    Condition = Weather.MostlySunny;
                    _nextWeatherChance = 0.50d;
                    break;
                case 3:
                    Condition = Weather.Thunderstorm;
                    _nextWeatherChance = 0.90d;
                    _disasterChance = (float) GameSimulationApp.Instance.Random.NextDouble();

                    if (GameSimulationApp.Instance.Random.NextBool())
                        GameSimulationApp.Instance.EventDirector.TriggerEvent(GameSimulationApp.Instance.Vehicle,
                            typeof(SevereWeather));

                    break;
                case 4:
                    Condition = Weather.Haze;
                    _nextWeatherChance = 0.70d;
                    break;
                case 5:
                    Condition = Weather.Fog;
                    _nextWeatherChance = 0.78d;

                    if (GameSimulationApp.Instance.Random.NextBool())
                        GameSimulationApp.Instance.EventDirector.TriggerEvent(GameSimulationApp.Instance.Vehicle,
                            typeof(HeavyFog));

                    break;
                case 6:
                    Condition = Weather.Rain;
                    _nextWeatherChance = 0.56d;
                    break;
                case 7:
                    Condition = Weather.Overcast;
                    _nextWeatherChance = 0.42d;
                    break;
                case 8:
                    Condition = Weather.Cloudy;
                    _nextWeatherChance = 0.30d;
                    break;
                default:
                    Condition = Weather.MostlySunny;
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
                    Condition = Weather.Clear;
                    _nextWeatherChance = 0.28d;
                    break;
                case 1:
                    Condition = Weather.MostlySunny;
                    _nextWeatherChance = 0.35d;
                    break;
                case 2:
                    Condition = Weather.PartlySunny;
                    _nextWeatherChance = 0.28d;
                    break;
                case 3:
                    Condition = Weather.Sunny;
                    _nextWeatherChance = 0.24d;
                    break;
                case 4:
                    Condition = Weather.ChanceOfThunderstorm;
                    _nextWeatherChance = 0.60d;
                    _disasterChance = (float) GameSimulationApp.Instance.Random.NextDouble();
                    break;
                case 5:
                    Condition = Weather.ChanceOfRain;
                    _nextWeatherChance = 0.56d;
                    _disasterChance = (float) GameSimulationApp.Instance.Random.NextDouble();
                    break;
                default:
                    Condition = Weather.Clear;
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
                    Condition = Weather.Clear;
                    _nextWeatherChance = 0.10d;
                    break;
                case 1:
                    Condition = Weather.MostlySunny;
                    _nextWeatherChance = 0.25d;
                    break;
                case 2:
                    Condition = Weather.PartlySunny;
                    _nextWeatherChance = 0.35d;
                    break;
                case 3:
                    Condition = Weather.Sunny;
                    _nextWeatherChance = 0.60d;
                    break;
                case 4:
                    Condition = Weather.ChanceOfThunderstorm;
                    _nextWeatherChance = 0.30d;
                    _disasterChance = (float) GameSimulationApp.Instance.Random.NextDouble();
                    break;
                case 5:
                    Condition = Weather.ChanceOfRain;
                    _nextWeatherChance = 0.33d;
                    _disasterChance = (float) GameSimulationApp.Instance.Random.NextDouble();
                    break;
                default:
                    Condition = Weather.Clear;
                    _nextWeatherChance = 0.10d;
                    break;
            }
        }

        /// <summary>Returns average temperature for given climate classification and month.</summary>
        /// <param name="whichMonth">The which Month.</param>
        /// <returns>The <see cref="ClimateData" />.</returns>
        private ClimateData GetTemperatureByMonth(Month whichMonth)
        {
            foreach (var data in _averageTemperatures)
                if (data.Month == whichMonth) return data;

            return null;
        }
    }
}