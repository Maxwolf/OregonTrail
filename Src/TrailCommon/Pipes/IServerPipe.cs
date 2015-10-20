using System.Collections.Generic;

namespace TrailCommon
{
    public interface IServerPipe : IPipe
    {
        ISimulation GameHost { get; }
        NamedPipeServer<PipeMessage> Server { get; }
        ISet<string> Clients { get; }
        void TickPipe();
    }
}