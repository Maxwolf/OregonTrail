namespace TrailCommon
{
    public interface IHunt : IMode
    {
        void UseBullets(uint amount);
        void AddFood(uint amount);
        void UpdateVehicle();
    }
}
