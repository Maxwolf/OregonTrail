namespace TrailCommon
{
    public interface IHunt : ITrailMode
    {
        void UseBullets(uint amount);
        void AddFood(uint amount);
        void UpdateVehicle();
    }
}
