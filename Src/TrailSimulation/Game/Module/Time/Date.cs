namespace TrailSimulation.Game
{
    public class Date
    {
        public const int NumberOfDaysInMonth = 30;

        public Date(int dueYear, Months dueMonth, int dueDay)
        {
            Day = dueDay;
            Month = dueMonth;
            Year = dueYear;
        }

        public Months Month { get; set; }
        public int Year { get; set; }
        public int Day { get; set; }

        public override string ToString()
        {
            return $"{Month} {Day}, {Year}";
        }
    }
}