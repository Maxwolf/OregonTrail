namespace TrailCommon
{
    public interface IPipe
    {
        bool Connected { get; }
        string CurrentID { get; set; }
        bool ShouldStop { get; }
        void TickPipe();
        void Start();
        void Stop();
    }
}