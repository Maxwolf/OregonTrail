namespace TrailCommon
{
    public interface ISimulationTime
    {
        Date Date { get; }
        uint GrassAvaliable { get; }
        Climate Weather { get; }
        Months CurrentMonth { get; }
        TravelPace CurrentSpeed { get; }
        uint CurrentDay { get; }
        uint CurrentYear { get; }
        uint TotalYears { get; }
        uint TotalMonths { get; }
        uint TotalDays { get; }
        int TotalDaysThisYear { get; }
        Climate CalculateWeather();
        void UpdateClimate();
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