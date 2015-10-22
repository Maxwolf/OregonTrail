namespace TrailCommon
{
    public interface IHuntingMode : IMode
    {
        void UseBullets(uint amount);
        void AddFood(uint amount);
        void UpdateVehicle();
    }
}