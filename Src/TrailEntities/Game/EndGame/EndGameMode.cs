using TrailEntities.Mode;

namespace TrailEntities.Game.EndGame
{
    /// <summary>
    ///     Attached when the party leader dies, or the vehicle reaches the end of the trail.
    /// </summary>
    public sealed class EndGameMode : ModeProduct
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailEntities.ModeProduct" /> class.
        /// </summary>
        public EndGameMode() : base(new EndGameInfo(), false)
        {
        }

        /// <summary>
        ///     Defines the current game gameMode the inheriting class is going to take responsibility for when attached to the
        ///     simulation.
        /// </summary>
        public override GameMode ModeType
        {
            get { return GameMode.EndGame; }
        }
    }
}