using OregonTrailDotNet.Event;

namespace OregonTrailDotNet.Module.Director
{
    /// <summary>
    ///     Remembers the last random event executed and the turn it landed on, so the graphical travel screen can
    ///     hang the matching event picture in the sky (or swap the burning-wagon frame in) for the day that follows
    ///     — the way the original blits an event picture over the travel screen. Pure recording: nothing here rolls,
    ///     ticks, or renders, so headless hosts carry it inert.
    /// </summary>
    internal static class SceneEvents
    {
        /// <summary>The class name of the last event executed, or null when none has been.</summary>
        internal static string LastEventName { get; private set; }

        /// <summary>The simulation turn the last event executed on.</summary>
        internal static int LastEventTurn { get; private set; }

        /// <summary>Records an executed event; called by the director for every event, forced or rolled.</summary>
        internal static void Record(EventProduct directorEvent)
        {
            LastEventName = directorEvent?.GetType().Name;
            LastEventTurn = GameSimulationApp.Instance?.TotalTurns ?? 0;
        }
    }
}
