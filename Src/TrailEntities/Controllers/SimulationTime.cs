using TrailCommon;

namespace TrailEntities
{
    /// <summary>
    ///     Manages the weather for a given location, used to make randomizing and keeping track of the weather based on time
    ///     of year much easier.
    /// </summary>
    public class SimulationTime : ISimulationTime
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailEntities.SimulationTime" /> class.
        /// </summary>
        public SimulationTime(uint startingYear, Months startingMonth, uint startingDay, TravelPace startingSpeed)
        {
            // Create a new time object for our simulation.
            CurrentYear = startingYear;
            CurrentMonth = startingMonth;
            CurrentDay = startingDay;
            CurrentSpeed = startingSpeed;

            TotalTicks = 0;
            TotalDays = 0;
            TotalMonths = 0;
            TotalYears = 0;
            TotalDaysThisYear = 1;

            Weather = CalculateWeather();
            GrassAvaliable = 10;
        }

        public Climate Weather { get; private set; }

        public Months CurrentMonth { get; private set; }

        public TravelPace CurrentSpeed { get; private set; }

        public uint CurrentDay { get; private set; }

        public uint CurrentYear { get; private set; }

        public uint TotalTicks { get; private set; }

        public uint TotalYears { get; private set; }

        public uint TotalMonths { get; private set; }

        public uint TotalDays { get; private set; }

        public int TotalDaysThisYear { get; private set; }

        public Climate CalculateWeather()
        {
            return Climate.Sunny;
        }

        public void UpdateClimate()
        {
            Weather = CalculateWeather();
            GrassAvaliable = 42;
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

            // Fire tick event.
            // NOTE: Do this last as it triggers logic pertaining to advancement of the game.
            TotalTicks++;
            TickTimeEvent?.Invoke(TotalTicks);
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
        ///     Resumes game simulation after being paused.
        /// </summary>
        public void ResumeTick()
        {
            // Fire speed change event to kick start timer.
            SpeedChangeEvent?.Invoke();
        }

        public event TickTimeHandler TickTimeEvent;
        public event YearHandler YearEndEvent;
        public event MonthHandler MonthEndEvent;
        public event DayHandler DayEndEvent;
        public event SpeedHandler SpeedChangeEvent;

        public Date Date
        {
            get { return new Date(CurrentYear, CurrentMonth, CurrentDay); }
        }

        public uint GrassAvaliable { get; private set; }
    }
}