namespace TrailCommon
{
    public interface IHunt : IGameMode
    {
        void UseBullets(uint amount);
        void AddFood(uint amount);
        void UpdateVehicle();
    }
}
