using System;
using TrailCommon;

namespace TrailEntities
{
    /// <summary>
    ///     Facilitates the ability to control the entire simulation with the passes interface reference. Server simulation
    ///     keeps track of all currently loaded game modes and will only tick the top-most one so they can be stacked and clear
    ///     out until there are none.
    /// </summary>
    public abstract class GameMode : Invoker, IMode
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailEntities.GameMode" /> class.
        /// </summary>
        protected GameMode(IGameSimulation game)
        {
            Game = game;
            Game.Server.Server.ClientMessage += Server_ClientMessage;
            Game.Server.Server.ClientConnected += Server_ClientConnected;
            Game.Server.Server.ClientDisconnected += Server_ClientDisconnected;
        }

        public virtual void OnModeRemoved()
        {
            Game.Server.Server.ClientMessage -= Server_ClientMessage;
            Game.Server.Server.ClientConnected -= Server_ClientConnected;
            Game.Server.Server.ClientDisconnected -= Server_ClientDisconnected;
        }

        public abstract ModeType Mode { get; }

        public IGameSimulation Game { get; }

        public virtual void TickMode()
        {
        }

        protected void Server_ClientDisconnected(NamedPipeConnection<PipeMessage, PipeMessage> connection)
        {
            throw new NotImplementedException();
        }

        protected void Server_ClientConnected(NamedPipeConnection<PipeMessage, PipeMessage> connection)
        {
            throw new NotImplementedException();
        }

        protected void Server_ClientMessage(NamedPipeConnection<PipeMessage, PipeMessage> connection,
            PipeMessage message)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        ///     Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        ///     A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return Mode.ToString();
        }
    }
}