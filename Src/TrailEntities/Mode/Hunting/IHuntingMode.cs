namespace TrailEntities.Mode.Hunting
{
    public interface IHuntingMode : IMode
    {
        void UseBullets(int amount);
        void AddFood(int amount);
        void UpdateVehicle();
    }
}