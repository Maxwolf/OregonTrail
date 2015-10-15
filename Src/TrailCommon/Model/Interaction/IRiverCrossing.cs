namespace TrailCommon
{
    public interface IRiverCrossing
    {
        IVehicle Vehicle { get; set; }
        void CaulkVehicle();
        void Ford();
        void UseFerry();
        void UpdateVehicle();
    }
}
