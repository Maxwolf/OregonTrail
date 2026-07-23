// Created by Maxwolf (bigmaxwolf.com)

using OregonTrailDotNet.Entity.Location.Weather;
using OregonTrailDotNet.Entity.Vehicle;
using OregonTrailDotNet.Event;
using OregonTrailDotNet.Event.Weather;

namespace OregonTrailDotNet.Module.Climate
{
    /// <summary>
    ///     Runs the weather for the whole journey rather than for each place separately: there is one sky over the party, and
    ///     what it has been doing lately is what matters. Every day it may pick a fresh reading from the current country's
    ///     table for the month and decide whether anything is falling out of it, and it keeps two running totals - how wet
    ///     the country has become and how much snow is lying - because those are what swell the rivers ahead and bog the
    ///     wagon down. Rain soaks in and dries out quickly; snow lingers, then melts into the rivers when it warms up.
    /// </summary>
    public sealed class ClimateModule : WolfCurses.Module.Module
    {
        /// <summary>
        ///     Weather is only re-rolled about half the time, which is what makes a spell of weather last a few days rather
        ///     than flickering from one day to the next.
        /// </summary>
        private const double RerollChance = 0.5;

        /// <summary>
        ///     Width in degrees of the band a day's temperature is drawn from, starting at the month's coldest expected value.
        /// </summary>
        private const int TemperatureSpread = 41;

        /// <summary>
        ///     Below this band, precipitation arrives as snow rather than rain.
        /// </summary>
        private const int FreezingBand = 2;

        /// <summary>
        ///     Above this band it is warm enough to melt lying snow into the rivers.
        /// </summary>
        private const int ThawBand = 2;

        /// <summary>
        ///     Initializes the climate for a journey leaving in the given month. Parties setting out early in the year find
        ///     the country still soaked from the spring melt and the rivers running high; by high summer it has dried out.
        ///     Snow is only still lying if they leave before April.
        /// </summary>
        /// <param name="startingMonth">Month the journey begins, one for January.</param>
        public ClimateModule(int startingMonth)
        {
            var game = GameSimulationApp.Instance;

            Wetness = 7 - startingMonth + game.Random.NextDouble();
            if (Wetness < 0)
                Wetness = 0;

            SnowPack = game.Random.NextDouble()*12*(startingMonth < 4 ? 1 : 0);

            // Start with a reading in hand so the very first day has weather rather than a blank.
            Roll(force: true);
        }

        /// <summary>
        ///     Today's temperature in Fahrenheit.
        /// </summary>
        public int Temperature { get; private set; }

        /// <summary>
        ///     Today's temperature banded into the six steps the party actually feels, from very cold to very hot. This is
        ///     what the cold and the heat are judged against; the condition below may be reporting rain instead.
        /// </summary>
        public int TemperatureBand { get; private set; }

        /// <summary>
        ///     What the weather is doing today, which is the temperature band unless there is rain or snow falling.
        /// </summary>
        public WeatherConditionsEnum Condition { get; private set; }

        /// <summary>
        ///     How wet the country has become. Rain adds to it and it dries out by a tenth a day; the rivers ahead are deeper,
        ///     wider and faster for every point of it, which is why a spring crossing is so much worse than a summer one.
        /// </summary>
        public double Wetness { get; private set; }

        /// <summary>
        ///     How much snow is lying. It melts far more slowly than rain dries, and when it does go it goes into the rivers.
        /// </summary>
        public double SnowPack { get; private set; }

        /// <summary>
        ///     Fraction by which lying snow slows the wagon, from none at all up to a complete standstill.
        /// </summary>
        public double SnowDrag
        {
            get
            {
                var drag = SnowPack/40.0;
                if (drag < 0)
                    return 0;
                return drag > 1 ? 1 : drag;
            }
        }

        /// <summary>
        ///     Advances the weather by a day, and lets the day's sky visit its events on a travelling party.
        /// </summary>
        public void Tick()
        {
            Roll(force: false);
            FireWeatherEvents();
        }

        /// <summary>
        ///     The sky's own events, fired from the sky rather than a blind category roll so a blizzard only ever
        ///     strikes out of blizzard weather. Only a travelling party is exposed — a party resting, trading or
        ///     waiting out a storm is already stopped, which is also what stops a lost-time event chaining into
        ///     another storm before the first has cleared.
        /// </summary>
        private void FireWeatherEvents()
        {
            var game = GameSimulationApp.Instance;
            var director = game?.EventDirector;
            var vehicle = game?.Vehicle;
            if (director == null || vehicle == null || vehicle.Status != VehicleStatusEnum.Moving)
                return;

            switch (Condition)
            {
                case WeatherConditionsEnum.VerySnowy:
                    // The worst travelling weather in the game sometimes pins the party down for days.
                    if (game.Random.NextDouble() < 0.15)
                        director.TriggerEvent(vehicle, typeof(Blizzard));
                    break;
                case WeatherConditionsEnum.VeryRainy:
                    // A downpour sometimes turns violent: usually a destructive storm, occasionally hail.
                    if (game.Random.NextDouble() < 0.15)
                        director.TriggerEvent(vehicle,
                            game.Random.NextDouble() < 0.33 ? typeof(HailStorm) : typeof(SevereWeather));
                    break;
                default:
                    // The mundane sky (heavy fog and whatever joins it) rolls at the same ~1% a day every other
                    // event category gets.
                    director.TriggerEventByType(vehicle, EventCategoryEnum.Weather);
                    break;
            }
        }

        /// <summary>
        ///     Picks the day's weather and folds whatever fell into the running totals.
        /// </summary>
        /// <param name="force">TRUE to take a fresh reading regardless, used when starting out.</param>
        private void Roll(bool force)
        {
            var game = GameSimulationApp.Instance;
            var raining = false;

            // Most spells of weather run on for a few days before the sky changes its mind.
            if (force || (game.Random.NextDouble() < RerollChance))
            {
                var climate = game.Trail?.CurrentLocation?.Climate ?? ClimateEnum.MissouriValley;
                var month = (int) game.Time.CurrentMonth;

                var low = ClimateRegistry.LowTemperature(climate, month);
                Temperature = low + game.Random.Next(TemperatureSpread);
                TemperatureBand = Band(Temperature);
                Condition = (WeatherConditionsEnum) TemperatureBand;

                raining = game.Random.NextDouble() < ClimateRegistry.PrecipitationChance(climate, month);
            }

            // How much water fell today, and how much of it settled as snow.
            var rainfall = 0.0;
            var snowfall = 0.0;

            if (raining)
            {
                // Most rain is a passing shower; occasionally it comes down in earnest and does four times as much to
                // the rivers ahead.
                var heavy = game.Random.NextDouble() < 0.3;
                Condition = heavy ? WeatherConditionsEnum.VeryRainy : WeatherConditionsEnum.Rainy;
                rainfall = heavy ? 0.8 : 0.2;

                // On a cold enough day the same weather arrives as snow, which piles up instead of running off.
                if (TemperatureBand < FreezingBand)
                {
                    Condition = heavy ? WeatherConditionsEnum.VerySnowy : WeatherConditionsEnum.Snowy;
                    snowfall = 8*rainfall;
                    rainfall = 0;
                }
            }

            // Rain dries out of the country quickly; snow sits.
            Wetness = 0.9*Wetness + rainfall;
            SnowPack = 0.97*SnowPack + snowfall;

            // A thaw, or a hard rain falling on the snow, carries it off into the rivers.
            if (SnowPack > 0 && ((TemperatureBand > ThawBand) || (Condition == WeatherConditionsEnum.VeryRainy)))
            {
                Wetness += 0.5;
                SnowPack -= 5;
                if (SnowPack < 0)
                    SnowPack = 0;
            }
        }

        /// <summary>
        ///     Bands a temperature reading into the six steps from very cold to very hot.
        /// </summary>
        /// <param name="temperature">Reading in Fahrenheit.</param>
        /// <returns>Band from zero (very cold) to five (very hot).</returns>
        private static int Band(int temperature)
        {
            var band = (temperature + 10)/20;
            if (band < 0)
                return 0;
            return band > 5 ? 5 : band;
        }

        /// <summary>
        ///     Fired when the simulation is closing and needs to clear out any data structures that it created so the program
        ///     can exit cleanly.
        /// </summary>
        public override void Destroy()
        {
            Wetness = 0;
            SnowPack = 0;
        }
    }
}
