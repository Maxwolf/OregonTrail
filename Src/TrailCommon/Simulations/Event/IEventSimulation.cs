using System.Collections.ObjectModel;

namespace TrailCommon
{
    /// <summary>
    ///     Base interface for the event manager, it is ticked as a sub-system of the primary game simulation and can affect
    ///     game modes, people, and vehicles.
    /// </summary>
    public interface IEventSimulation
    {
        ReadOnlyCollection<IEventItem> Events { get; }
        event EventAdd EventAdded;
        void AddEvent(IEventItem eventItem);
    }

    public delegate void EventAdd(IEventItem eventItem);
}