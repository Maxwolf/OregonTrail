using TrailCommon;

namespace TrailEntities
{
    /// <summary>
    ///     Information that should be told to the player, does not mean the simulation should stop but more that alarms should
    ///     be going off to inform the player of critical situations before they get worse.
    /// </summary>
    public sealed class WarningEvent : EventItem<Vehicle, Warning, WarningResponse>
    {
        /// <summary>
        ///     Create a new event item that can be passed to the simulation director.
        /// </summary>
        /// <param name="targetThing">Simulation compatible entity which will be affected.</param>
        /// <param name="eventEnum">Enumeration that will be passed to result along with target for event execution.</param>
        /// <param name="responseNoun">Class that can affect the target game entity based on event verb selection.</param>
        public WarningEvent(Vehicle targetThing, Warning eventEnum, WarningResponse responseNoun)
            : base(targetThing, eventEnum, responseNoun)
        {
        }
    }
}