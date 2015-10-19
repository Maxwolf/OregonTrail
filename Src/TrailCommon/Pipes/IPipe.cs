namespace TrailCommon
{
    public interface IPipe
    {
        bool IsClosing { get; }
        void Start();
        void Stop();
        void TickPipe();
        ISimulation Game { get; }
    }
}