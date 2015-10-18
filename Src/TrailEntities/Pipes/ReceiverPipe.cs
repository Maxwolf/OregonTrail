using System;
using System.IO;
using System.IO.Pipes;
using TrailCommon;

namespace TrailEntities
{
    public sealed class ReceiverPipe : Pipe, IReceiverPipe
    {
        private bool _connected;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailGame.GameClientApp" /> class.
        /// </summary>
        public ReceiverPipe()
        {

        }

        public override bool Connected
        {
            get { return _connected; }
        }

        public override void TickPipe()
        {
            if (!_connected)
                return;

            if (IsStopping)
                return;

            Console.WriteLine("Tick Receiver Pipe");
        }

        public override void Start()
        {

            _connected = true;
        }
    }
}