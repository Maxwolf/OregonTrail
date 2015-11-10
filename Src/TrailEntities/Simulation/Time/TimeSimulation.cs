namespace TrailEntities.Simulation
{
    /// <summary>
    ///     Simulates the linear progression of time from one fixed date to another, requires being ticked to advance the time
    ///     simulation by one day. There are also other options and events for checking state, and changing state.
    /// </summary>
    public sealed class TimeSimulation
    {
        public delegate void DayHandler(int dayCount);

        public delegate void MonthHandler(int monthCount);

        public delegate void YearHandler(int yearCount);

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailEntities.SimulationTime" /> class.
        /// </summary>
        public TimeSimulation(int startingYear, Months startingMonth, int startingDay)
        {
            // Create a new time object for our simulation.
            CurrentYear = startingYear;
            CurrentMonth = startingMonth;
            CurrentDay = startingDay;

            TotalDays = 0;
            TotalMonths = 0;
            TotalYears = 0;
            TotalDaysThisYear = 1;
        }

        public Months CurrentMonth { get; private set; }

        private int CurrentDay { get; set; }

        public int CurrentYear { get; private set; }

        private int TotalYears { get; set; }

        private int TotalMonths { get; set; }

        private int TotalDays { get; set; }

        private int TotalDaysThisYear { get; set; }

        public Date Date
        {
            get { return new Date(CurrentYear, CurrentMonth, CurrentDay); }
        }

        /// <summary>
        ///     Advances the tick of the game forward firing off events for new eras, years, months and days.
        /// </summary>
        public void TickTime()
        {
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

            // Fire day end event.
            DayEndEvent?.Invoke(TotalDays);

            if (shouldCheckMonthEnd)
            {
                // If number of days reaches NumberOfDaysInMonth advance the month, which in tandem will increment years if needed all based on days.
                if (CurrentMonth != Months.December)
                {
                    // End of month
                    CurrentMonth++;
                    TotalMonths++;

                    // Fire month end event
                    MonthEndEvent?.Invoke(TotalMonths);
                }
                else
                {
                    // End of year
                    CurrentYear++;
                    TotalDaysThisYear = 1;
                    CurrentMonth = Months.January;
                    TotalMonths++;
                    TotalYears++;

                    // Fire month end event
                    MonthEndEvent?.Invoke(TotalMonths);

                    // Fire year end event
                    YearEndEvent?.Invoke(TotalYears);
                }
            }
        }

        /// <summary>
        ///     Changes the time simulations current month, this also will reset the day back to the first of that month.
        /// </summary>
        public void SetMonth(Months month)
        {
            CurrentMonth = month;
            CurrentDay = 1;
        }

        public event YearHandler YearEndEvent;
        public event MonthHandler MonthEndEvent;
        public event DayHandler DayEndEvent;
    }
}