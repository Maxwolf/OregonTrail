// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TimeModule.cs" company="Ron 'Maxwolf' McDowell">
//   ron.mcdowell@gmail.com
// </copyright>
// <summary>
//   Simulates the linear progression of time from one fixed date to another, requires being ticked to advance the time
//   simulation by one day. There are also other options and events for checking state, and changing state.
// </summary>
// --------------------------------------------------------------------------------------------------------------------



using TrailSimulation.Core;

namespace TrailSimulation.Game
{
    /// <summary>
    ///     Simulates the linear progression of time from one fixed date to another, requires being ticked to advance the time
    ///     simulation by one day. There are also other options and events for checking state, and changing state.
    /// </summary>
    public sealed class TimeModule : Module
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TimeModule"/> class. 
        ///     Initializes a new instance of the <see cref="T:TrailSimulation.Core.ModuleProduct"/> class.
        /// </summary>
        public TimeModule()
        {
            // Create a new time object for our simulation.
            CurrentYear = 1848;
            CurrentMonth = Month.March;
            CurrentDay = 1;

            TotalDays = 0;
            TotalMonths = 0;
            TotalYears = 0;
            TotalDaysThisYear = 1;
        }

        /// <summary>
        ///     Current month the simulation is ticking over, it has typically thirty (30) days in it.
        /// </summary>
        public Month CurrentMonth { get; private set; }

        /// <summary>
        ///     Day of the month the simulation is currently ticking over.
        /// </summary>
        private int CurrentDay { get; set; }

        /// <summary>
        ///     Year of the simulation, this takes twelve (12) months going by before it will be incremented.
        /// </summary>
        public int CurrentYear { get; private set; }

        /// <summary>
        ///     Total number of simulated years that have gone by since the simulation began.
        /// </summary>
        private int TotalYears { get; set; }

        /// <summary>
        ///     Total number of simulated months that have gone by since the start of the simulation.
        /// </summary>
        private int TotalMonths { get; set; }

        /// <summary>
        ///     Total number of days that have gone by since the start of the simulation.
        /// </summary>
        private int TotalDays { get; set; }

        /// <summary>
        ///     Total number of days that have passed in the current year, typically this is three-hundred and sixty-five days
        ///     (365). Leap year is not calculated or simulated.
        /// </summary>
        private int TotalDaysThisYear { get; set; }

        /// <summary>
        ///     Assembles the current date of the simulation by creating date object from current year, month, and day.
        /// </summary>
        public Date Date
        {
            get { return new Date(CurrentYear, CurrentMonth, CurrentDay); }
        }

        /// <summary>
        ///     Fired when the simulation is closing and needs to clear out any data structures that it created so the program can
        ///     exit cleanly.
        /// </summary>
        public override void Destroy()
        {
            // Create a new time object for our simulation.
            CurrentYear = 0;
            CurrentMonth = 0;
            CurrentDay = 0;

            TotalDays = 0;
            TotalMonths = 0;
            TotalYears = 0;
            TotalDaysThisYear = 1;
        }

        /// <summary>
        /// Advances the tick of the game forward firing off events for new eras, years, months and days.
        /// </summary>
        /// <param name="skipDay">
        /// Determines if the time module should fire off event for a day but not actually advance time
        ///     itself.
        /// </param>
        public void TickTime(bool skipDay)
        {
            // Check if we should skip date calculations and just fire off day.
            if (skipDay)
            {
                OnTickDay(true);
                return;
            }

            var shouldCheckMonthEnd = false;
            if (CurrentDay < Date.NumberOfDaysInMonth)
            {
                // Advance the count of days without triggers for end of month/year.
                TotalDaysThisYear++;
                CurrentDay++;
                TotalDays++;
            }
            else
            {
                // Allow processing and checking of end of month/year calculations.
                CurrentDay = 1;
                shouldCheckMonthEnd = true;
            }

            // One day has passed.
            OnTickDay(false);

            if (shouldCheckMonthEnd)
            {
                // If number of days reaches NumberOfDaysInMonth advance the month, which in tandem will increment years if needed all based on days.
                if (CurrentMonth != Month.December)
                {
                    // End of month
                    CurrentMonth++;
                    TotalMonths++;

                    // Fire month end event
                    OnMonthEnd();
                }
                else
                {
                    // End of year
                    CurrentYear++;
                    TotalDaysThisYear = 1;
                    CurrentMonth = Month.January;
                    TotalMonths++;
                    TotalYears++;

                    // Fire month end event
                    OnMonthEnd();

                    // Fire year end event
                    OnYearEnd();
                }
            }
        }

        /// <summary>
        ///     Fired after each year in the simulation.
        /// </summary>
        private void OnYearEnd()
        {
            // TODO: Use time module year end or remove it!
        }

        /// <summary>
        ///     Fired after each month in the simulation.
        /// </summary>
        private void OnMonthEnd()
        {
            // TODO: Use time module month end or remove it!
        }

        /// <summary>
        /// Fired after each day in the simulation.
        /// </summary>
        /// <param name="skipDay">
        /// Determines if the time simulation did not advance the day but still ticked the game.
        /// </param>
        private void OnTickDay(bool skipDay)
        {
            // Move towards the next location on the trail. Ticks vehicle, location, people, weather, etc.
            GameSimulationApp.Instance.Trail.OnTick(false, skipDay);
        }

        /// <summary>
        /// Changes the time simulations current month, this also will reset the day back to the first of that month.
        /// </summary>
        /// <param name="month">
        /// The month.
        /// </param>
        public void SetMonth(Month month)
        {
            CurrentMonth = month;
            CurrentDay = 1;
        }
    }
}