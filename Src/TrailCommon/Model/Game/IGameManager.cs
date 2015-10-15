namespace TrailCommon
{
    public interface IGameManager
    {
        void ChooseProfession();
        void BuyInitialItems();
        void ChooseNames();
        void StartGame();
        void SetMode(IGameMode gameMode);
        void Tick();
        event Tick TickEvent;
        event NewGame NewgameEvent;
        event EndGame EndgameEvent;
    }

    public delegate void EndGame();
    public delegate void NewGame();
    public delegate void Tick(ulong tickCount);
}
