namespace TrailCommon
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
        ///     Defines a result that will occur when the event runs, this typically is a result on party, vehicle such as altering
        ///     health, removing items, inflicting diseases, etc.
        /// </summary>
        object ResultNoun { get; set; }

        /// <summary>
        ///     Time stamp from the simulation on when this event occurred in the time line of events that make up the players
        ///     progress on the trail and game in total.
        /// </summary>
        Date Timestamp { get; set; }
    }
}