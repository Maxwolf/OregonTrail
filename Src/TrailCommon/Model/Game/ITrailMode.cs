namespace TrailCommon
{
    public interface ITrailMode
    {
        TrailModeType Mode { get; }

        ITrailVehicle TrailVehicle { get; }
    }
}