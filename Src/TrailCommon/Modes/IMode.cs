namespace TrailCommon
{
    public interface IMode
    {
        ModeType Mode { get; }
        string GetTUI();
        void TickMode();
        void SendCommand(string returnedLine);
    }
}