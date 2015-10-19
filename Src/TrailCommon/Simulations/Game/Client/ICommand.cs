namespace TrailCommon
{
    /// <summary>
    ///     Declares an interface for executing an operation.
    /// </summary>
    public interface ICommand
    {
        IReceiver Receiver { get; }
        void Execute();
    }
}