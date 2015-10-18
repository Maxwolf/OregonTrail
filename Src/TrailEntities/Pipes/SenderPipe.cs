using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using TrailCommon;

namespace TrailEntities
{
    /// <summary>
    ///     Handles core interaction of the game, all other game types are inherited from this game mode. Deals with weather,
    ///     parties, random events, keeping track of beginning and end of the game.
    /// </summary>
    public sealed class SenderPipe : Pipe, ISenderPipe
    {
        private bool _connected;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailGame.GameServerApp" /> class.
        /// </summary>
        public SenderPipe()
        {
            PipeStream = new NamedPipeClientStream(
                SimulationName,
                ServerPipeName,
                PipeDirection.Out,
                PipeOptions.None);
        }

        public Queue<Tuple<string, string>> QueuedCommands { get; } = new Queue<Tuple<string, string>>();

        public object CommandQueueLock { get; } = new object();

        /// <summary>
        ///     Add a command to queue of outgoing commands.
        /// </summary>
        /// <returns>ID of the enqueued command so the user can relate it with the corresponding response.</returns>
        public string EnqueueCommand(string command)
        {
            var resultId = Guid.NewGuid().ToString();
            lock (CommandQueueLock)
            {
                QueuedCommands.Enqueue(Tuple.Create(resultId, command));
            }
            return resultId;
        }

        public NamedPipeClientStream PipeStream { get; }

        public override bool Connected
        {
            get { return _connected; }
        }

        public override void TickPipe()
        {
            if (!_connected)
                return;

            if (ShouldStop)
                return;

            using (var sw = new StreamWriter(PipeStream) {AutoFlush = true})
            {
                // No commands? Keep waiting
                if (QueuedCommands.Count <= 0)
                    return;

                // We're going to modify the command queue, lock it
                Tuple<string, string> _currentCommand = null;
                lock (CommandQueueLock)
                {
                    // Check to see if someone else stole our command before we got here
                    if (QueuedCommands.Count > 0)
                        _currentCommand = QueuedCommands.Dequeue();
                }

                // Was a command dequeued above?
                if (_currentCommand == null)
                    return;

                CurrentID = _currentCommand.Item1;
                sw.WriteLine(_currentCommand.Item2);
            }
        }

        public override void Start()
        {
            PipeStream.Connect();
            _connected = true;
        }
    }
}