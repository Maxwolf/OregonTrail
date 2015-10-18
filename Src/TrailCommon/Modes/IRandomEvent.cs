namespace TrailCommon
{
    /// <summary>
    ///     Base travel event that will be used in the creation of other variations of the event such as a medical event,
    ///     physical event, derelict, tombstone, thief, etc.
    /// </summary>
    public interface IRandomEvent : IMode
    {
        string Name { get; }
        void MakeEvent();
        void CheckForRandomEvent();
    }
}