using System;

namespace TrailEntities
{
    public sealed class AnimalEventItem : EventItem<Vehicle, AnimalEvent>
    {
        /// <summary>
        ///     Create a new event item that can be passed to the simulation director.
        /// </summary>
        /// <param name="eventTarget">Simulation compatible entity which will be affected.</param>
        /// <param name="eventEnum">Enumeration that will be passed to result along with target for event execution.</param>
        public AnimalEventItem(Vehicle eventTarget, AnimalEvent eventEnum) : base(eventTarget, eventEnum)
        {
        }

        /// <summary>
        ///     Fired when the event handler associated with this enum type triggers action on target entity. Implementation is
        ///     left completely up to handler.
        /// </summary>
        /// <param name="eventTarget">Entity which will be affected by this method.</param>
        /// <param name="eventEnum">Enumeration that helps this method determine what should be done.</param>
        protected override void OnEventExecute(Vehicle eventTarget, AnimalEvent eventEnum)
        {
            switch (eventEnum)
            {
                case AnimalEvent.Snakebite:
                    break;
                case AnimalEvent.Locusts:
                    break;
                case AnimalEvent.Mosquitos:
                    break;
                case AnimalEvent.BuffaloStampede:
                    break;
                case AnimalEvent.OxWanderedOff:
                    break;
                case AnimalEvent.OxIsSick:
                    break;
                case AnimalEvent.OxDied:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(eventEnum), eventEnum, null);
            }
        }
    }
}