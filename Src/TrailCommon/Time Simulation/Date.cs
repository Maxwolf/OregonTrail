namespace TrailCommon
{
    public class Date
    {
        public const uint NumberOfDaysInMonth = 30;

        public Date(uint dueYear, Months dueMonth, uint dueDay)
        {
            Day = dueDay;
            Month = dueMonth;
            Year = dueYear;
        }

        public Months Month { get; set; }
        public uint Year { get; set; }
        public uint Day { get; set; }

        public override string ToString()
        {
            return $"{Month}, {Year}";
        }
    }
}