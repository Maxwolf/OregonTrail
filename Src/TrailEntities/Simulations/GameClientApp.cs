using System;
using System.IO;
using System.IO.Pipes;
using System.Threading;
using TrailCommon;

namespace TrailEntities
{
    public class GameClientApp : SimulationApp
    {
        private const string ClientPipeName = "FromSrvPipe";
        private readonly Thread _receiver;

        public event ResponseReceived ResponseReceived;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailGame.GameClientApp" /> class.
        /// </summary>
        public GameClientApp()
        {
            _receiver = new Thread(syncClientServer =>
            {
                var waitForResponse = (AutoResetEvent)syncClientServer;

                using (
                    var pipeStream = new NamedPipeClientStream(SimulationName, ClientPipeName, PipeDirection.In,
                        PipeOptions.None)
                    )
                {
                    pipeStream.Connect();

                    using (var sr = new StreamReader(pipeStream))
                        // Do this till Cancel() is called
                        // Again, this is a tight loop, perhaps a Thread.Yield or something?
                        while (!IsClosing)
                            // If there's anything in the stream
                            if (!sr.EndOfStream)
                            {
                                // Read it
                                var response = sr.ReadLine();
                                // Raise the event for processing
                                // Note that this event is being raised from the
                                // receiver thread and you can't access UI here
                                // You will need to Control.BeginInvoke or some such
                                RaiseResponseReceived(_currentId, response);

                                // Proceed with sending subsequent commands
                                waitForResponse.Set();
                            }
                }
            });
        }

        protected override void OnTick()
        {
            base.OnTick();
        }

        protected override void OnFirstTick()
        {
            _receiver.Start(_waitForResponse);
        }

        /// <summary>
        ///     Raises an event when a response is received
        /// </summary>
        private void RaiseResponseReceived(string id, string message)
        {
            ResponseReceived?.Invoke(this, new ResponseReceivedEventArgs(id, message));
        }

        protected override GameMode OnModeChanging(ModeType mode)
        {
            throw new NotImplementedException();
        }
    }
}
