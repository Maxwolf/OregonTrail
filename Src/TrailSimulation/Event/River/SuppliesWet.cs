using TrailSimulation.Game;

namespace TrailSimulation.Event
{
    /// <summary>
    ///     Does not destroy items or drown people but will make you lose time gathering your things and drying them out.
    /// </summary>
    [DirectorEvent(EventCategory.RiverCross)]
    public sealed class SuppliesWet : EventLoseTime
    {
    }
}