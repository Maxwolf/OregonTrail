namespace TrailCommon
{
    public interface IGameInitializer
    {
        void ChooseProfession();
        void BuyInitialItems();
        void StartGame();
        void Tick();
        event Tick TickEvent;
    }

    public delegate void Tick(ulong tickCount);
}
