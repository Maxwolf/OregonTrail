// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ClimateData.cs" company="Ron 'Maxwolf' McDowell">
//   ron.mcdowell@gmail.com
// </copyright>
// <summary>
//   Defines all the data for a given climate simulation for a location.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using TrailSimulation.Game;

namespace TrailSimulation.Entity
{
    /// <summary>
    ///     Defines all the data for a given climate simulation for a location.
    /// </summary>
    public class ClimateData
    {
        /// <summary>Initializes a new instance of the <see cref="ClimateData"/> class.
        ///     Creates a new bit of climate data.</summary>
        /// <param name="month">Month this data is representative of.</param>
        /// <param name="averageTemp">Average set of temperatures for this month.</param>
        /// <param name="tempMax">Maximum temperature this month can have.</param>
        /// <param name="tempMin">Minimum temperature this month can have.</param>
        /// <param name="rainfall">Average rainfall for this month.</param>
        /// <param name="avgHumidity">Daily humidity for this month.</param>
        public ClimateData(
            Month month, 
            float averageTemp, 
            float tempMax, 
            float tempMin, 
            float rainfall, 
            int avgHumidity)
        {
            Month = month;
            Temperature = averageTemp;
            TemperatureMax = tempMax;
            TemperatureMin = tempMin;
            Rainfall = rainfall;
            Humidity = avgHumidity;
        }

        /// <summary>
        ///     Month this data is representative of.
        /// </summary>
        public Month Month { get; }

        /// <summary>
        ///     Average set of temperatures for this month.
        /// </summary>
        public float Temperature { get; }

        /// <summary>
        ///     Maximum temperature this month can have.
        /// </summary>
        public float TemperatureMax { get; }

        /// <summary>
        ///     Minimum temperature this month can have.
        /// </summary>
        public float TemperatureMin { get; }

        /// <summary>
        ///     Average rainfall for this month.
        /// </summary>
        public float Rainfall { get; }

        /// <summary>
        ///     Daily humidity for this month.
        /// </summary>
        public int Humidity { get; }
    }
}