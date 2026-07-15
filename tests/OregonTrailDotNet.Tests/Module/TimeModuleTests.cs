using OregonTrailDotNet.Module.Time;
using Xunit;

namespace OregonTrailDotNet.Tests.Module
{
    /// <summary>
    ///     Covers the linear day/month/year progression. Ticking time also ticks the trail, vehicle,
    ///     and weather underneath it, so these run against a fully booted simulation.
    /// </summary>
    public class TimeModuleTests : SimulationTestBase
    {
        [Fact]
        public void Simulation_StartsOnMarchFirst1848()
        {
            Assert.Equal(1848, Game.Time.CurrentYear);
            Assert.Equal(MonthEnum.March, Game.Time.CurrentMonth);
            Assert.Equal(1, Game.Time.Date.Day);
        }

        [Fact]
        public void TickTime_AdvancesOneDay()
        {
            Game.Time.TickTime(false);

            Assert.Equal(2, Game.Time.Date.Day);
        }

        [Fact]
        public void TickTime_SkipDay_DoesNotAdvanceDate()
        {
            Game.Time.TickTime(true);

            Assert.Equal(1, Game.Time.Date.Day);
            Assert.Equal(MonthEnum.March, Game.Time.CurrentMonth);
        }

        [Fact]
        public void TickTime_RollsOverMonthAfterThirtyDays()
        {
            for (var day = 0; day < Date.NumberOfDaysInMonth; day++)
                Game.Time.TickTime(false);

            Assert.Equal(MonthEnum.April, Game.Time.CurrentMonth);
            Assert.Equal(1, Game.Time.Date.Day);
            Assert.Equal(1848, Game.Time.CurrentYear);
        }

        [Fact]
        public void TickTime_CountsTheMonthRolloverDay_TotalDaysEqualsNumberOfTicks()
        {
            // Crossing a month boundary is still a full day elapsing. Ticking one whole month must advance TotalDays
            // by exactly that many days; the rollover tick used to skip TotalDays++, undercounting by one per month.
            var before = Game.Time.TotalDays;

            for (var day = 0; day < Date.NumberOfDaysInMonth; day++)
                Game.Time.TickTime(false);

            Assert.Equal(MonthEnum.April, Game.Time.CurrentMonth);
            Assert.Equal(before + Date.NumberOfDaysInMonth, Game.Time.TotalDays);
        }

        [Fact]
        public void TickTime_RollsOverYearAfterDecember()
        {
            Game.Time.SetMonth(MonthEnum.December);
            for (var day = 0; day < Date.NumberOfDaysInMonth; day++)
                Game.Time.TickTime(false);

            Assert.Equal(MonthEnum.January, Game.Time.CurrentMonth);
            Assert.Equal(1849, Game.Time.CurrentYear);
        }

        [Fact]
        public void SetMonth_ChangesMonthAndResetsDayToFirst()
        {
            Game.Time.TickTime(false);
            Game.Time.TickTime(false);
            Game.Time.SetMonth(MonthEnum.May);

            Assert.Equal(MonthEnum.May, Game.Time.CurrentMonth);
            Assert.Equal(1, Game.Time.Date.Day);
        }

        [Fact]
        public void Date_ToString_FormatsMonthDayYear()
        {
            Assert.Equal("March 1, 1848", Game.Time.Date.ToString());
        }
    }
}
