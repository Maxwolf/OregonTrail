using System.Diagnostics.CodeAnalysis;
using TrailSimulation.Game;

namespace TrailSimulation.Event
{
    [DirectorEvent(EventCategory.Vehicle)]
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public sealed class WrongTrail : EventLoseTime
    {
    }
}