namespace TrailCommon
{
    public interface IPipe
    {
        bool Connected { get; }
        bool IsStopping { get; }
        void TickPipe();
        void Start();
        void Stop();
    }
}