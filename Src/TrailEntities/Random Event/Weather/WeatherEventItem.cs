using System;

namespace TrailEntities
{
    public sealed class WeatherEventItem : EventItem<Vehicle, WeatherEvent>
    {
        /// <summary>
        ///     Create a new event item that can be passed to the simulation director.
        /// </summary>
        /// <param name="eventTarget">Simulation compatible entity which will be affected.</param>
        /// <param name="eventEnum">Enumeration that will be passed to result along with target for event execution.</param>
        public WeatherEventItem(Vehicle eventTarget, WeatherEvent eventEnum) : base(eventTarget, eventEnum)
        {
        }

        /// <summary>
        ///     Fired when the event handler associated with this enum type triggers action on target entity. Implementation is
        ///     left completely up to handler.
        /// </summary>
        /// <param name="eventTarget">Entity which will be affected by this method.</param>
        /// <param name="eventEnum">Enumeration that helps this method determine what should be done.</param>
        protected override void OnEventExecute(Vehicle eventTarget, WeatherEvent eventEnum)
        {
            switch (eventEnum)
            {
                case WeatherEvent.RoughTrail:
                    break;
                case WeatherEvent.Wildfire:
                    break;
                case WeatherEvent.SevereWeather:
                    break;
                case WeatherEvent.QuicksandAhead:
                    break;
                case WeatherEvent.ObstructedPath:
                    break;
                case WeatherEvent.WagonDust:
                    break;
                case WeatherEvent.RiverCrossing:
                    break;
                case WeatherEvent.MountainClimbing:
                    break;
                case WeatherEvent.Desert:
                    break;
                case WeatherEvent.WrongTrail:
                    break;
                case WeatherEvent.HeavyFog:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(eventEnum), eventEnum, null);
            }
        }
    }
}