using System;
using System.IO;
using System.IO.Pipes;
using TrailCommon;

namespace TrailEntities
{
    public sealed class ReceiverPipe : Pipe, IReceiverPipe
    {
        private bool _connected;
        private NamedPipeClientStream pipeStream;


        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailGame.GameClientApp" /> class.
        /// </summary>
        public ReceiverPipe()
        {
            pipeStream = new NamedPipeClientStream(
                SimulationName,
                ClientPipeName,
                PipeDirection.In,
                PipeOptions.None);
        }

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

            using (var sr = new StreamReader(pipeStream))
            {
                Console.WriteLine("Tick Receiver Pipe");

                // If there's anything in the stream
                if (sr.EndOfStream)
                    return;

                // Read it
                var response = sr.ReadLine();

                // Raise the event for processing
                // Note that this event is being raised from the
                // receiver thread and you can't access UI here
                // You will need to Control.BeginInvoke or some such
                RaiseResponseReceived(CurrentID, response);
            }
        }

        public override void Start()
        {
            pipeStream.Connect();
            _connected = true;
        }

        public event ResponseReceived ResponseReceived;

        /// <summary>
        ///     Raises an event when a response is received
        /// </summary>
        private void RaiseResponseReceived(string id, string message)
        {
            ResponseReceived?.Invoke(this, new ResponseReceivedEventArgs(id, message));
        }
    }
}