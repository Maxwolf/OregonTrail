// Created by Maxwolf (bigmaxwolf.com) 
// Timestamp 01/03/2016@1:50 AM

using System.Collections.Generic;
using OregonTrailDotNet.Module.Time;

namespace OregonTrailDotNet.Entity.Location.Weather
{
    /// <summary>
    ///     Holds a bunch of static data about various climate types that is used in the location weather simulation.
    /// </summary>
    public static class ClimateRegistry
    {
        /// <summary>
        ///     Gets the polar.
        /// </summary>
        public static IEnumerable<ClimateData> Polar
        {
            get
            {
                var polarTemperatures = new List<ClimateData>
                {
                    new ClimateData(MonthEnum.January, -2.9f, -0.2f, -5.5f, 15, 1),
                    new ClimateData(MonthEnum.February, -9.5f, -6.3f, -11.6f, 21.2f, 2),
                    new ClimateData(MonthEnum.March, -18.2f, -14, -21.1f, 24.1f, 4),
                    new ClimateData(MonthEnum.April, -20.7f, -17.4f, -24.9f, 18.4f, 2),
                    new ClimateData(MonthEnum.May, -21.7f, -19, -27.1f, 18.4f, 1),
                    new ClimateData(MonthEnum.June, -23, -19.1f, -27.3f, 24.9f, 4),
                    new ClimateData(MonthEnum.July, -25.7f, -21.7f, -30.1f, 15.6f, 5),
                    new ClimateData(MonthEnum.August, -26.1f, -22.8f, -31.8f, 11.3f, 3),
                    new ClimateData(MonthEnum.September, -24.6f, -20.8f, -29.4f, 11.8f, 2),
                    new ClimateData(MonthEnum.October, -18.9f, -15.5f, -23.4f, 9.7f, 3),
                    new ClimateData(MonthEnum.November, -9.7f, -6.7f, -12.7f, 9.5f, 4),
                    new ClimateData(MonthEnum.December, -3.4f, -0.8f, -6, 15.7f, 2)
                };

                return polarTemperatures;
            }
        }

        /// <summary>
        ///     Gets the continental.
        /// </summary>
        public static IEnumerable<ClimateData> Continental
        {
            get
            {
                var continentalTemperatures = new List<ClimateData>
                {
                    new ClimateData(MonthEnum.January, 20.5f, 28, 13, 54, 53),
                    new ClimateData(MonthEnum.February, 20, 27, 13, 55, 54),
                    new ClimateData(MonthEnum.March, 17, 24, 10, 63, 58),
                    new ClimateData(MonthEnum.April, 14, 20, 7, 55, 60),
                    new ClimateData(MonthEnum.May, 9, 15, 3, 52, 71),
                    new ClimateData(MonthEnum.June, 7, 12, 1, 49, 79),
                    new ClimateData(MonthEnum.July, 6, 11, 0, 41, 78),
                    new ClimateData(MonthEnum.August, 7, 13, 1, 50, 70),
                    new ClimateData(MonthEnum.September, 10, 16, 3, 41, 62),
                    new ClimateData(MonthEnum.October, 13, 19, 6, 70, 55),
                    new ClimateData(MonthEnum.November, 15, 22, 8, 54, 53),
                    new ClimateData(MonthEnum.December, 18.5f, 26, 11, 47, 53)
                };

                return continentalTemperatures;
            }
        }

        /// <summary>
        ///     Gets the moderate.
        /// </summary>
        public static IEnumerable<ClimateData> Moderate
        {
            get
            {
                var moderateTemperatures = new List<ClimateData>
                {
                    new ClimateData(MonthEnum.January, -6.1f, 8.6f, -8.8f, 40, 85),
                    new ClimateData(MonthEnum.February, -6f, 10.2f, -8.8f, 31, 82),
                    new ClimateData(MonthEnum.March, -1.4f, 14.9f, -4.2f, 35, 77),
                    new ClimateData(MonthEnum.April, 4.4f, 25.3f, 1.0f, 33, 71),
                    new ClimateData(MonthEnum.May, 10.9f, 30.9f, 6.6f, 38, 64),
                    new ClimateData(MonthEnum.June, 15.8f, 34.6f, 11.8f, 64, 66),
                    new ClimateData(MonthEnum.July, 18.1f, 35.3f, 14.4f, 78, 69),
                    new ClimateData(MonthEnum.August, 16.4f, 37.1f, 13, 77, 74),
                    new ClimateData(MonthEnum.September, 11.0f, 30.4f, 8.1f, 67, 79),
                    new ClimateData(MonthEnum.October, 5.6f, 21.0f, 3.4f, 65, 82),
                    new ClimateData(MonthEnum.November, -0.1f, 12.3f, -2.1f, 56, 85),
                    new ClimateData(MonthEnum.December, -3.9f, 10.9f, -6.4f, 49, 86)
                };

                return moderateTemperatures;
            }
        }

        /// <summary>
        ///     Gets the dry.
        /// </summary>
        public static IEnumerable<ClimateData> Dry
        {
            get
            {
                var dryTemperatures = new List<ClimateData>
                {
                    new ClimateData(MonthEnum.January, 27, 33, 21, 102, 70),
                    new ClimateData(MonthEnum.February, 26.5f, 33, 20, 88, 70),
                    new ClimateData(MonthEnum.March, 26, 33, 19, 46, 68),
                    new ClimateData(MonthEnum.April, 24, 32, 16, 19, 65),
                    new ClimateData(MonthEnum.May, 20.5f, 29, 12, 10, 61),
                    new ClimateData(MonthEnum.June, 18, 27, 9, 1, 54),
                    new ClimateData(MonthEnum.July, 17.5f, 27, 9, 1, 56),
                    new ClimateData(MonthEnum.August, 21, 30, 12, 4, 60),
                    new ClimateData(MonthEnum.September, 25, 34, 16, 5, 66),
                    new ClimateData(MonthEnum.October, 28, 36, 20, 13, 69),
                    new ClimateData(MonthEnum.November, 28, 35, 21, 46, 71),
                    new ClimateData(MonthEnum.December, 27.5f, 34, 21, 67, 69)
                };

                return dryTemperatures;
            }
        }

        /// <summary>
        ///     Gets the tropical.
        /// </summary>
        public static IEnumerable<ClimateData> Tropical
        {
            get
            {
                var tropicalTemperatures = new List<ClimateData>
                {
                    new ClimateData(MonthEnum.January, 12, 18, 7, 40, 73),
                    new ClimateData(MonthEnum.February, 13, 18, 6, 50, 74),
                    new ClimateData(MonthEnum.March, 13, 18, 7, 80, 77),
                    new ClimateData(MonthEnum.April, 13, 19, 8, 110, 78),
                    new ClimateData(MonthEnum.May, 13, 18, 8, 100, 79),
                    new ClimateData(MonthEnum.June, 13, 18, 8, 60, 72),
                    new ClimateData(MonthEnum.July, 13, 17, 8, 40, 70),
                    new ClimateData(MonthEnum.August, 12, 17, 8, 40, 63),
                    new ClimateData(MonthEnum.September, 12, 17, 7, 50, 62),
                    new ClimateData(MonthEnum.October, 13, 18, 8, 140, 55),
                    new ClimateData(MonthEnum.November, 13, 18, 8, 110, 55),
                    new ClimateData(MonthEnum.December, 12, 18, 7, 60, 53)
                };

                return tropicalTemperatures;
            }
        }
    }
}