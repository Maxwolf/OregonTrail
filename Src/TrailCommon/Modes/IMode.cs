namespace TrailCommon
{
    public interface IMode
    {
        ModeType Mode { get; }
        IVehicle Vehicle { get; }
        void TickMode();
    }
}