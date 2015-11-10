using TrailEntities.Mode;

namespace TrailEntities.Game
{
    public interface IHuntingMode : IMode
    {
        void UseBullets(int amount);
        void AddFood(int amount);
        void UpdateVehicle();
    }
}