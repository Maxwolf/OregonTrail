using System;
using System.Collections.Generic;
using TrailCommon;

namespace TrailEntities
{
    public sealed class ServerPipe : Pipe, IServerPipe
    {
        private bool _isClosing;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailEntities.ServerPipe" /> class.
        /// </summary>
        public ServerPipe(ISimulation serverGame)
        {
            GameHost = serverGame;
            Server = new NamedPipeServer<PipeMessage>(DefaultPipeName);
            Server.ClientConnected += OnClientConnected;
            Server.ClientDisconnected += OnClientDisconnected;
            Server.ClientMessage += OnClientMessage;
            Server.Error += OnError;
        }

        public override bool IsClosing
        {
            get { return _isClosing; }
        }

        public ISimulation GameHost { get; }

        public NamedPipeServer<PipeMessage> Server { get; }

        public ISet<string> Clients { get; } = new HashSet<string>();

        public override void Start()
        {
            Server.Start();
        }

        public override void Stop()
        {
            _isClosing = true;
            Server.ClientConnected -= OnClientConnected;
            Server.ClientDisconnected -= OnClientDisconnected;
            Server.ClientMessage -= OnClientMessage;
            Server.Stop();
        }

        public void TickPipe()
        {
            // Check if there are any clients to pump messages to.
            if (Clients.Count <= 0)
                return;
        }

        public override void Action()
        {
            Console.WriteLine("Called ServerPipe.Action()");
        }

        private void OnClientConnected(NamedPipeConnection<PipeMessage, PipeMessage> connection)
        {
            Clients.Add(connection.Name);
            Console.WriteLine("Client {0} is now connected!", connection.Id);
            connection.PushMessage(new PipeMessage
            {
                ID = (int) GameHost.ActiveMode.Mode,
                Text = "Welcome!"
            });
        }

        private void OnClientDisconnected(NamedPipeConnection<PipeMessage, PipeMessage> connection)
        {
            Clients.Remove(connection.Name);
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