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

            Console.WriteLine("Tick Sender Pipe");
        }

        public override void Start()
        {

            _connected = true;
        }
    }
}