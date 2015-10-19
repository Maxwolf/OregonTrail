namespace TrailCommon
{
    public interface IClientPipe : IPipe
    {
        NamedPipeClient<PipeMessage> Client { get; }
    }
}