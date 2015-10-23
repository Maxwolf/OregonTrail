namespace TrailCommon
{
    public interface INewGameMode : IMode
    {
        NewGameInfo NewGameInfo { get; set; }
        void ChooseProfession();
        void BuyInitialItems();
        void ChooseNames();
        void StartGame();
    }
}