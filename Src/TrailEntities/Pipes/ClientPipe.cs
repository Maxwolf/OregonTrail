using System;
using TrailCommon;

namespace TrailEntities
{
    public sealed class ClientPipe : Pipe, IClientPipe
    {
        private bool _isClosing;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailEntities.ClientPipe" /> class.
        /// </summary>
        public ClientPipe(ISimulation clientGame) : base(clientGame)
        {
            Client = new NamedPipeClient<PipeMessage>(DefaultPipeName);
            Client.ServerMessage += OnServerMessage;
            Client.Disconnected += OnDisconnected;
            Client.Error += OnError;
        }

        public override bool IsClosing
        {
            get { return _isClosing; }
        }

        public override void Start()
        {
            Client.Start();
        }

        public override void Stop()
        {
            _isClosing = true;
            Client.ServerMessage -= OnServerMessage;
            Client.Disconnected -= OnDisconnected;
            Client.Stop();
        }

        public override void TickPipe()
        {
            throw new NotImplementedException();
        }

        public NamedPipeClient<PipeMessage> Client { get; }

        private void OnDisconnected(NamedPipeConnection<PipeMessage, PipeMessage> connection)
        {
            Console.WriteLine("Client disconnected.");
        }

        private void OnServerMessage(NamedPipeConnection<PipeMessage, PipeMessage> connection, PipeMessage message)
        {
            Console.WriteLine("Server says: {0}", message);
        }

        private void OnError(Exception exception)
        {
            Console.Error.WriteLine("ERROR: {0}", exception);
        }
    }
}