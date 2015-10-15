namespace TrailCommon
{
    /// <summary>
    ///     Manages the weather for a given location, used to make randomizing and keeping track of the weather based on time
    ///     of year much easier.
    /// </summary>
    public class Climate : IClimate
    {
        private Date _date;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailCommon.Climate" /> class.
        /// </summary>
        public Climate()
        {
            _date = new Date(5, 5, 1985);
            WeatherCondition = CalculateWeather();
            GrassAvaliable = 10;
        }

        public WeatherCondition WeatherCondition { get; private set; }

        public WeatherCondition CalculateWeather()
        {
            return WeatherCondition.Warm;
        }

        public void UpdateClimate()
        {
            WeatherCondition = CalculateWeather();
            GrassAvaliable = 42;
        }

        public Date Date
        {
            get { return _date; }
        }

        public uint GrassAvaliable { get; private set; }
    }
}