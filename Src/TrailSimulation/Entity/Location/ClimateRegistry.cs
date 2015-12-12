using System.Collections.Generic;
using TrailSimulation.Game;

namespace TrailSimulation.Entity
{
    /// <summary>
    ///     Holds a bunch of static data about various climate types that is used in the location weather simulation.
    /// </summary>
    public static class ClimateRegistry
    {
        public static IEnumerable<ClimateData> Polar
        {
            get
            {
                var polarTemperatures = new List<ClimateData>
                {
                    new ClimateData(Month.January, -2.9f, -0.2f, -5.5f, 15, 1),
                    new ClimateData(Month.February, -9.5f, -6.3f, -11.6f, 21.2f, 2),
                    new ClimateData(Month.March, -18.2f, -14, -21.1f, 24.1f, 4),
                    new ClimateData(Month.April, -20.7f, -17.4f, -24.9f, 18.4f, 2),
                    new ClimateData(Month.May, -21.7f, -19, -27.1f, 18.4f, 1),
                    new ClimateData(Month.June, -23, -19.1f, -27.3f, 24.9f, 4),
                    new ClimateData(Month.July, -25.7f, -21.7f, -30.1f, 15.6f, 5),
                    new ClimateData(Month.August, -26.1f, -22.8f, -31.8f, 11.3f, 3),
                    new ClimateData(Month.September, -24.6f, -20.8f, -29.4f, 11.8f, 2),
                    new ClimateData(Month.October, -18.9f, -15.5f, -23.4f, 9.7f, 3),
                    new ClimateData(Month.November, -9.7f, -6.7f, -12.7f, 9.5f, 4),
                    new ClimateData(Month.December, -3.4f, -0.8f, -6, 15.7f, 2)
                };

                return polarTemperatures;
            }
        }

        public static IEnumerable<ClimateData> Continental
        {
            get
            {
                var continentalTemperatures = new List<ClimateData>
                {
                    new ClimateData(Month.January, 20.5f, 28, 13, 54, 53),
                    new ClimateData(Month.February, 20, 27, 13, 55, 54),
                    new ClimateData(Month.March, 17, 24, 10, 63, 58),
                    new ClimateData(Month.April, 14, 20, 7, 55, 60),
                    new ClimateData(Month.May, 9, 15, 3, 52, 71),
                    new ClimateData(Month.June, 7, 12, 1, 49, 79),
                    new ClimateData(Month.July, 6, 11, 0, 41, 78),
                    new ClimateData(Month.August, 7, 13, 1, 50, 70),
                    new ClimateData(Month.September, 10, 16, 3, 41, 62),
                    new ClimateData(Month.October, 13, 19, 6, 70, 55),
                    new ClimateData(Month.November, 15, 22, 8, 54, 53),
                    new ClimateData(Month.December, 18.5f, 26, 11, 47, 53)
                };

                return continentalTemperatures;
            }
        }

        public static IEnumerable<ClimateData> Moderate
        {
            get
            {
                var moderateTemperatures = new List<ClimateData>
                {
                    new ClimateData(Month.January, -6.1f, 8.6f, -8.8f, 40, 85),
                    new ClimateData(Month.February, -6f, 10.2f, -8.8f, 31, 82),
                    new ClimateData(Month.March, -1.4f, 14.9f, -4.2f, 35, 77),
                    new ClimateData(Month.April, 4.4f, 25.3f, 1.0f, 33, 71),
                    new ClimateData(Month.May, 10.9f, 30.9f, 6.6f, 38, 64),
                    new ClimateData(Month.June, 15.8f, 34.6f, 11.8f, 64, 66),
                    new ClimateData(Month.July, 18.1f, 35.3f, 14.4f, 78, 69),
                    new ClimateData(Month.August, 16.4f, 37.1f, 13, 77, 74),
                    new ClimateData(Month.September, 11.0f, 30.4f, 8.1f, 67, 79),
                    new ClimateData(Month.October, 5.6f, 21.0f, 3.4f, 65, 82),
                    new ClimateData(Month.November, -0.1f, 12.3f, -2.1f, 56, 85),
                    new ClimateData(Month.December, -3.9f, 10.9f, -6.4f, 49, 86)
                };

                return moderateTemperatures;
            }
        }

        public static IEnumerable<ClimateData> Dry
        {
            get
            {
                var dryTemperatures = new List<ClimateData>
                {
                    new ClimateData(Month.January, 27, 33, 21, 102, 70),
                    new ClimateData(Month.February, 26.5f, 33, 20, 88, 70),
                    new ClimateData(Month.March, 26, 33, 19, 46, 68),
                    new ClimateData(Month.April, 24, 32, 16, 19, 65),
                    new ClimateData(Month.May, 20.5f, 29, 12, 10, 61),
                    new ClimateData(Month.June, 18, 27, 9, 1, 54),
                    new ClimateData(Month.July, 17.5f, 27, 9, 1, 56),
                    new ClimateData(Month.August, 21, 30, 12, 4, 60),
                    new ClimateData(Month.September, 25, 34, 16, 5, 66),
                    new ClimateData(Month.October, 28, 36, 20, 13, 69),
                    new ClimateData(Month.November, 28, 35, 21, 46, 71),
                    new ClimateData(Month.December, 27.5f, 34, 21, 67, 69)
                };

                return dryTemperatures;
            }
        }

        public static IEnumerable<ClimateData> Tropical
        {
            get
            {
                var tropicalTemperatures = new List<ClimateData>
                {
                    new ClimateData(Month.January, 12, 18, 7, 40, 73),
                    new ClimateData(Month.February, 13, 18, 6, 50, 74),
                    new ClimateData(Month.March, 13, 18, 7, 80, 77),
                    new ClimateData(Month.April, 13, 19, 8, 110, 78),
                    new ClimateData(Month.May, 13, 18, 8, 100, 79),
                    new ClimateData(Month.June, 13, 18, 8, 60, 72),
                    new ClimateData(Month.July, 13, 17, 8, 40, 70),
                    new ClimateData(Month.August, 12, 17, 8, 40, 63),
                    new ClimateData(Month.September, 12, 17, 7, 50, 62),
                    new ClimateData(Month.October, 13, 18, 8, 140, 55),
                    new ClimateData(Month.November, 13, 18, 8, 110, 55),
                    new ClimateData(Month.December, 12, 18, 7, 60, 53)
                };

                return tropicalTemperatures;
            }
        }
    }
}