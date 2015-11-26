using System;
using TrailSimulation.Core;
using TrailSimulation.Entity;

namespace TrailSimulation.Game
{
    /// <summary>
    ///     Simulates the linear progression of time from one fixed date to another, requires being ticked to advance the time
    ///     simulation by one day. There are also other options and events for checking state, and changing state.
    /// </summary>
    public sealed class TimeModule : IModule
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailSimulation.Core.ModuleProduct" /> class.
        /// </summary>
        public TimeModule()
        {
            // Create a new time object for our simulation.
            CurrentYear = 1848;
            CurrentMonth = Months.March;
            CurrentDay = 1;

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
        ///     Fired when the simulation is closing and needs to clear out any data structures that it created so the program can
        ///     exit cleanly.
        /// </summary>
        public void Destroy()
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
        ///     Fired when the simulation ticks the module that it created inside of itself.
        /// </summary>
        public void Tick()
        {
            throw new NotImplementedException();
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

            // One day has passed.
            OnTickDay(TotalDays);

            if (shouldCheckMonthEnd)
            {
                // If number of days reaches NumberOfDaysInMonth advance the month, which in tandem will increment years if needed all based on days.
                if (CurrentMonth != Months.December)
                {
                    // End of month
                    CurrentMonth++;
                    TotalMonths++;

                    // Fire month end event
                    OnMonthEnd(TotalMonths);
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
                    OnMonthEnd(TotalMonths);

                    // Fire year end event
                    OnYearEnd(TotalYears);
                }
            }
        }

        /// <summary>
        ///     Fired after each year in the simulation.
        /// </summary>
        /// <param name="totalYears">Total number of years the simulation has ticked.</param>
        private void OnYearEnd(int totalYears)
        {
            // TODO: Use time module year end or remove it...
        }

        /// <summary>
        ///     Fired after each month in the simulation.
        /// </summary>
        /// <param name="totalMonths">Total number of months the simulation has ticked.</param>
        private void OnMonthEnd(int totalMonths)
        {
            // TODO: Use time module month end or remove it...
        }

        /// <summary>
        ///     Fired after each day in the simulation.
        /// </summary>
        /// <param name="totalDays">Total number of days the simulation has ticked.</param>
        private void OnTickDay(int totalDays)
        {
            // Each day we tick the weather, vehicle, and the people in it.
            GameSimulationApp.Instance.Climate.Tick();

            // Update total distance traveled on vehicle if we have not reached the point.
            GameSimulationApp.Instance.Vehicle.Tick();

            // Grab the total amount of monies the player has spent on the items in their inventory.
            var cost_ammo = GameSimulationApp.Instance.Vehicle.Inventory[SimEntity.Ammo].TotalValue;
            var cost_clothes = GameSimulationApp.Instance.Vehicle.Inventory[SimEntity.Clothes].TotalValue;
            var start_cash = GameSimulationApp.Instance.Vehicle.Inventory[SimEntity.Cash].TotalValue;

            // Move towards the next location on the trail.
            GameSimulationApp.Instance.Trail.Tick();
        }

        /// <summary>
        ///     Changes the time simulations current month, this also will reset the day back to the first of that month.
        /// </summary>
        public void SetMonth(Months month)
        {
            CurrentMonth = month;
            CurrentDay = 1;
        }
    }
}