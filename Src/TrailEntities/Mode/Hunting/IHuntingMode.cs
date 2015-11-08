namespace TrailEntities.Mode
{
    public interface IHuntingMode : IMode
    {
        void UseBullets(int amount);
        void AddFood(int amount);
        void UpdateVehicle();
    }
}