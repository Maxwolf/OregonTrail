namespace TrailCommon
{
    public interface IEventResponse
    {
        /// <summary>
        ///     Each event result has the ability to execute method.
        /// </summary>
        void Execute();
    }
}