namespace TrailCommon
{
    public interface IRiverCrossingMode : IMode<RiverCrossingCommands>
    {
        void CaulkVehicle();
        void Ford();
        void UseFerry();
        void UpdateVehicle();
    }
}