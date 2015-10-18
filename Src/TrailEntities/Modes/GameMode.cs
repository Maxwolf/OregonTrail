using TrailCommon;

namespace TrailEntities
{
    public abstract class GameMode : IMode
    {
        private readonly IGameServer _game;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailEntities.GameMode" /> class.
        /// </summary>
        protected GameMode(IGameServer game)
        {
            _game = game;
        }

        public abstract ModeType Mode { get; }

        public IGameServer Game
        {
            get { return _game; }
        }

        public virtual void TickMode()
        {
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