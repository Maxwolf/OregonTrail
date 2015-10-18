using System;
using System.Collections.Generic;
using System.IO.Pipes;

namespace TrailCommon
{
    public interface ISenderPipe : IPipe
    {
        Queue<Tuple<string, string>> QueuedCommands { get; }
        object CommandQueueLock { get; }
        NamedPipeClientStream PipeStream { get; }
        string EnqueueCommand(string command);
    }
}