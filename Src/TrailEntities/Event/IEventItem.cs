namespace TrailEntities
{
    /// <summary>
    ///     Defines a basic interface for working with an abstract event. The idea is what events need to keep track of what
    ///     they are affecting, what is happening, and the end result of that actually happening. Finally we have what caused
    ///     this to happen, this is optional because sometimes things happen for no good reason.
    /// </summary>
    public interface IEventItem
    {
        /// <summary>
        ///     Target object which the event is probably going to affect in one way or another.
        /// </summary>
        object TargetThing { get; set; }

        /// <summary>
        ///     Defines a descriptive action that can be taken on the target thing.
        /// </summary>
        object ActionVerb { get; set; }

        /// <summary>
        ///     Time stamp from the simulation on when this event occurred in the time line of events that make up the players
        ///     progress on the trail and game in total.
        /// </summary>
        Date Timestamp { get; set; }

        /// <summary>
        ///     Name of the event as it will be known in the directory system, can also be used to call it manually by name.
        ///     Another thing is this also is used as a key in dictionary of all events, hopefully preventing developers from
        ///     adding the same event more than once to the list.
        /// </summary>
        string Name { get; }

        /// <summary>
        ///     Each event result has the ability to execute method.
        /// </summary>
        void Execute();
    }
}