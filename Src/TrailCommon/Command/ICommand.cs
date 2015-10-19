namespace TrailCommon
{
    /// <summary>
    ///     Declares an interface for executing an operation.
    /// </summary>
    public interface ICommand
    {
        IGameSimulation Game { get; }
        IReceiver Receiver { get; }
        void Execute();
    }
}