namespace TrailCommon
{
    public interface IHuntingMode : IMode<HuntingCommands>
    {
        void UseBullets(uint amount);
        void AddFood(uint amount);
        void UpdateVehicle();
    }
}