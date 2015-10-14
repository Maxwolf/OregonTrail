namespace TrailCommon
{
    public class Date
    {
        private uint _day;
        private uint _month;
        private uint _year;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailCommon.Date" /> class.
        /// </summary>
        public Date(uint day, uint month, uint year)
        {
            _day = day;
            _month = month;
            _year = year;
        }

        private uint Month
        {
            get { return _month; }
        }

        private uint Day
        {
            get { return _day; }
        }

        private uint Year
        {
            get { return _year; }
        }
    }
}