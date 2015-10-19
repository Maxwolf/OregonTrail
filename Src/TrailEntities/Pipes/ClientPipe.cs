using System;
using TrailCommon;

namespace TrailEntities
{
    public sealed class ClientPipe : Pipe, IClientPipe
    {
        private readonly NamedPipeClient<PipeMessage> _client;
        private bool _isClosing;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailEntities.ClientPipe" /> class.
        /// </summary>
        public ClientPipe()
        {
            _client = new NamedPipeClient<PipeMessage>(DefaultPipeName);
            _client.ServerMessage += OnServerMessage;
            _client.Disconnected += OnDisconnected;
            _client.Error += OnError;
        }

        public override bool IsClosing
        {
            get { return _isClosing; }
        }

        public override void Start()
        {
            _client.Start();
        }

        public override void Stop()
        {
            _isClosing = true;
            _client.Stop();
        }

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