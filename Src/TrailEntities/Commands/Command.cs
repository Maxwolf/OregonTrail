using TrailCommon;

namespace TrailEntities
{
    /// <summary>
    ///     Declares an interface for executing an operation.
    /// </summary>
    public abstract class Command : ICommand
    {
        private readonly IGameSimulation _game;
        private readonly Receiver _receiver;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailEntities.Command" /> class.
        /// </summary>
        protected Command(IGameSimulation game)
        {
            _game = game;
            _receiver = game.Server as Receiver;
        }

        public IGameSimulation Game
        {
            get { return _game; }
        }

        public IReceiver Receiver
        {
            get { return _receiver; }
        }

        public virtual void Execute()
        {
            _receiver.Action();
        }
    }
}