using System.Collections.Generic;

namespace TrailCommon
{
    public interface IServerPipe : IPipe
    {
        NamedPipeServer<PipeMessage> Server { get; }
        ISet<string> Clients { get; }
    }
}