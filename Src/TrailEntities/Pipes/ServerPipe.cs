using System;
using System.Collections.Generic;
using TrailCommon;

namespace TrailEntities
{
    public sealed class ServerPipe : Pipe, IServerPipe
    {
        private bool _isClosing;
        private readonly NamedPipeServer<PipeMessage> _server;
        private readonly ISet<string> _clients = new HashSet<string>();

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailEntities.ServerPipe" /> class.
        /// </summary>
        public ServerPipe()
        {
            _server = new NamedPipeServer<PipeMessage>(DefaultPipeName);
            _server.ClientConnected += OnClientConnected;
            _server.ClientDisconnected += OnClientDisconnected;
            _server.ClientMessage += OnClientMessage;
            _server.Error += OnError;
        }

        public override bool IsClosing
        {
            get { return _isClosing; }
        }

        public ISet<string> Clients
        {
            get { return _clients; }
        }

        public override void Start()
        {
            _server.Start();
        }

        public override void Stop()
        {
            _isClosing = true;
            _server.Stop();
        }

        private void OnClientConnected(NamedPipeConnection<PipeMessage, PipeMessage> connection)
        {
            _clients.Add(connection.Name);
            Console.WriteLine("Client {0} is now connected!", connection.Id);
            connection.PushMessage(new PipeMessage
            {
                Id = new Random().Next(),
                Text = "Welcome!"
            });
        }

        private void OnClientDisconnected(NamedPipeConnection<PipeMessage, PipeMessage> connection)
        {
            _clients.Remove(connection.Name);
            Console.WriteLine("Client {0} disconnected", connection.Id);
        }

        private void OnClientMessage(NamedPipeConnection<PipeMessage, PipeMessage> connection, PipeMessage message)
        {
            Console.WriteLine("Client {0} says: {1}", connection.Id, message);
        }

        private void OnError(Exception exception)
        {
            Console.Error.WriteLine("ERROR: {0}", exception);
        }
    }
}