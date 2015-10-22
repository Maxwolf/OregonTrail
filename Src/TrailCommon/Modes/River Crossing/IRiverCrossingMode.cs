namespace TrailCommon
{
    public interface IRiverCrossingMode : IMode
    {
        void CaulkVehicle();
        void Ford();
        void UseFerry();
        void UpdateVehicle();
    }
}