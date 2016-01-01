// Created by Ron 'Maxwolf' McDowell (ron.mcdowell@gmail.com) 
// Timestamp 01/01/2016@3:27 AM

namespace TrailSimulation.Module.Time
{
    /// <summary>
    ///     The date.
    /// </summary>
    public class Date
    {
        /// <summary>
        ///     The number of days in month.
        /// </summary>
        public const int NumberOfDaysInMonth = 30;

        /// <summary>Initializes a new instance of the <see cref="Date" /> class.</summary>
        /// <param name="dueYear">The due year.</param>
        /// <param name="dueMonth">The due month.</param>
        /// <param name="dueDay">The due day.</param>
        public Date(int dueYear, Month dueMonth, int dueDay)
        {
            Day = dueDay;
            Month = dueMonth;
            Year = dueYear;
        }

        /// <summary>
        ///     Gets or sets the month.
        /// </summary>
        public Month Month { get; set; }

        /// <summary>
        ///     Gets or sets the year.
        /// </summary>
        public int Year { get; set; }

        /// <summary>
        ///     Gets or sets the day.
        /// </summary>
        public int Day { get; set; }

        /// <summary>
        ///     The to string.
        /// </summary>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        public override string ToString()
        {
            return $"{Month} {Day}, {Year}";
        }
    }
}