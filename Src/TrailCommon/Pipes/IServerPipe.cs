using System.Collections.Generic;

namespace TrailCommon
{
    public interface IServerPipe : IPipe
    {
        ISet<string> Clients { get; }
    }
}