using TrailCommon;

namespace TrailEntities
{
    public abstract class Pipe : Receiver, IPipe
    {
        protected const string DefaultPipeName = "TrailGame";
        private readonly ISimulation _game;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailEntities.Pipe" /> class.
        /// </summary>
        protected Pipe(ISimulation game)
        {
            _game = game;
        }

        public abstract bool IsClosing { get; }
        public abstract void Start();
        public abstract void Stop();
        public abstract void TickPipe();

        public ISimulation Game
        {
            get { return _game; }
        }
    }
}