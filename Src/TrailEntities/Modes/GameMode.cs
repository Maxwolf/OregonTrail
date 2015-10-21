using TrailCommon;

namespace TrailEntities
{
    /// <summary>
    ///     Facilitates the ability to control the entire simulation with the passes interface reference. Server simulation
    ///     keeps track of all currently loaded game modes and will only tick the top-most one so they can be stacked and clear
    ///     out until there are none.
    /// </summary>
    public abstract class GameMode : IMode
    {
        public const string GAMEMODE_DEFAULT_TUI = "[DEFAULT GAME MODE TEXT USER INTERFACE]";
        public const string GAMEMODE_EMPTY_TUI = "[NO GAME MODE ATTACHED]";

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailEntities.GameMode" /> class.
        /// </summary>
        protected GameMode(IGameSimulation game)
        {
            Game = game;
        }

        public virtual void OnModeRemoved()
        {
            throw new System.NotImplementedException();
        }

        public virtual string GetTUI()
        {
            return GAMEMODE_DEFAULT_TUI;
        }

        public abstract ModeType Mode { get; }

        public IGameSimulation Game { get; }

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

        public void SendMessage(string returnedLine)
        {
            throw new System.NotImplementedException();
        }
    }
}