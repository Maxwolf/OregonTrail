namespace TrailCommon
{
    public interface INewGameMode : IMode<NewGameCommands>
    {
        void ChooseProfession();
        void BuyInitialItems();
        void ChooseNames();
    }
}