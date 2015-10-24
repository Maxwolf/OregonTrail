using System;
using TrailCommon;

namespace TrailEntities
{
    /// <summary>
    ///     Simulates the linear progression of time from one fixed date to another, requires being ticked to advance the time
    ///     simulation by one day. There are also other options and events for checking state, and changing state.
    /// </summary>
    public sealed class TimeSim : ITimeSimulation
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailEntities.SimulationTime" /> class.
        /// </summary>
        public TimeSim(uint startingYear, Months startingMonth, uint startingDay, TravelPace startingSpeed)
        {
            // Create a new time object for our simulation.
            CurrentYear = startingYear;
            CurrentMonth = startingMonth;
            CurrentDay = startingDay;
            CurrentSpeed = startingSpeed;

            TotalDays = 0;
            TotalMonths = 0;
            TotalYears = 0;
            TotalDaysThisYear = 1;
        }

        public Months CurrentMonth { get; private set; }

        public TravelPace CurrentSpeed { get; private set; }

        public uint CurrentDay { get; private set; }

        public uint CurrentYear { get; private set; }

        public uint TotalYears { get; private set; }

        public uint TotalMonths { get; private set; }

        public uint TotalDays { get; private set; }

        public int TotalDaysThisYear { get; private set; }

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
        ///     Sets the current speed of the game simulation.
        /// </summary>
        public void SetSpeed(TravelPace castedSpeed)
        {
            // Check to make sure we are not already at this speed.
            if (castedSpeed == CurrentSpeed) return;

            // Change game simulation speed.
            CurrentSpeed = castedSpeed;
            SpeedChangeEvent?.Invoke();
        }

        /// <summary>
        ///     Changes the time simulations current month, this also will reset the day back to the first of that month.
        /// </summary>
        public void SetMonth(Months month)
        {
            // Complain if developer sets month to same thing twice.
            if (month == CurrentMonth)
                throw new InvalidOperationException("Attempted to set current month to exact same month!");

            CurrentMonth = month;
            CurrentDay = 1;
        }

        /// <summary>
        ///     Resumes game simulation after being paused.
        /// </summary>
        public void ResumeTick()
        {
            // Fire speed change event to kick start timer.
            SpeedChangeEvent?.Invoke();
        }

        public event YearHandler YearEndEvent;
        public event MonthHandler MonthEndEvent;
        public event DayHandler DayEndEvent;
        public event SpeedHandler SpeedChangeEvent;

        public Date Date
        {
            get { return new Date(CurrentYear, CurrentMonth, CurrentDay); }
        }
    }
}