namespace TrailCommon
{
    public interface INewGameMode : IMode
    {
        void ChooseProfession();
        void BuyInitialItems();
        void ChooseNames();
    }
}