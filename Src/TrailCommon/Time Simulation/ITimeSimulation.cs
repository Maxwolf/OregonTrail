namespace TrailCommon
{
    public interface ITimeSimulation
    {
        Date Date { get; }
        Months CurrentMonth { get; }
        TravelPace CurrentSpeed { get; }
        uint CurrentDay { get; }
        uint CurrentYear { get; }
        uint TotalYears { get; }
        uint TotalMonths { get; }
        uint TotalDays { get; }
        int TotalDaysThisYear { get; }
        void TickTime();
        void SetSpeed(TravelPace castedSpeed);
        void ResumeTick();
        event YearHandler YearEndEvent;
        event MonthHandler MonthEndEvent;
        event DayHandler DayEndEvent;
        event SpeedHandler SpeedChangeEvent;
    }

    public delegate void DayHandler(uint dayCount);

    public delegate void EraHandler(uint eraCount);

    public delegate void MonthHandler(uint monthCount);

    public delegate void SpeedHandler();

    public delegate void YearHandler(uint yearCount);
}