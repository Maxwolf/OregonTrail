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
            Assert.Equal(Month.March, Game.Time.CurrentMonth);
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
            Assert.Equal(Month.March, Game.Time.CurrentMonth);
        }

        [Fact]
        public void TickTime_RollsOverMonthAfterThirtyDays()
        {
            for (var day = 0; day < Date.NumberOfDaysInMonth; day++)
                Game.Time.TickTime(false);

            Assert.Equal(Month.April, Game.Time.CurrentMonth);
            Assert.Equal(1, Game.Time.Date.Day);
            Assert.Equal(1848, Game.Time.CurrentYear);
        }

        [Fact]
        public void TickTime_RollsOverYearAfterDecember()
        {
            Game.Time.SetMonth(Month.December);
            for (var day = 0; day < Date.NumberOfDaysInMonth; day++)
                Game.Time.TickTime(false);

            Assert.Equal(Month.January, Game.Time.CurrentMonth);
            Assert.Equal(1849, Game.Time.CurrentYear);
        }

        [Fact]
        public void SetMonth_ChangesMonthAndResetsDayToFirst()
        {
            Game.Time.TickTime(false);
            Game.Time.TickTime(false);
            Game.Time.SetMonth(Month.May);

            Assert.Equal(Month.May, Game.Time.CurrentMonth);
            Assert.Equal(1, Game.Time.Date.Day);
        }

        [Fact]
        public void Date_ToString_FormatsMonthDayYear()
        {
            Assert.Equal("March 1, 1848", Game.Time.Date.ToString());
        }
    }
}
