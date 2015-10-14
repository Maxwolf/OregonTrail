namespace TrailCommon
{
    public interface IRiverCrossing
    {
        IVehicle Vehicle { get; }
        void CaulkVehicle();
        void Ford();
        void UseFerry();
        void UpdateVehicle();
    }
}
