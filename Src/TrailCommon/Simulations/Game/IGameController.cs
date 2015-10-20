namespace TrailCommon
{
    public interface IGameController : ITick
    {
        IClientPipe Client { get; }
    }
}