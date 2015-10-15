namespace TrailCommon
{
    public interface IGameManager
    {
        void ChooseProfession();
        void BuyInitialItems();
        void StartGame();
        void Tick();
        event Tick TickEvent;
        event KeyPress KeypressEvent;
        event NewGame NewgameEvent;
        event EndGame EndgameEvent;
    }

    public delegate void EndGame();
    public delegate void NewGame();
    public delegate void Tick(ulong tickCount);
    public delegate void KeyPress(string keyCode);
}
