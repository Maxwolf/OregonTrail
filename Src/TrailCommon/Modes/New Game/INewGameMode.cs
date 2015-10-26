namespace TrailCommon
{
    public interface INewGameMode : IMode
    {
        NewGameInfo NewGameInfo { get; set; }
    }
}