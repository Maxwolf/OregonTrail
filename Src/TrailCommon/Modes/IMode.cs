namespace TrailCommon
{
    public interface IMode
    {
        ModeType Mode { get; }
        void TickMode();
        IVehicle Vehicle { get; }
    }
}