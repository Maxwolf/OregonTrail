namespace TrailEntities.Mode
{
    /// <summary>
    ///     Attached when the party leader dies, or the vehicle reaches the end of the trail.
    /// </summary>
    [GameMode(ModeCategory.EndGame, typeof(EndGameCommands), typeof(EndGameInfo))]
    public sealed class EndGameMode : GameMode
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailEntities.GameMode" /> class.
        /// </summary>
        public EndGameMode() : base(false)
        {
        }

        /// <summary>
        ///     Defines the current game mode the inheriting class is going to take responsibility for when attached to the
        ///     simulation.
        /// </summary>
        public override ModeCategory ModeCategory
        {
            get { return ModeCategory.EndGame; }
        }
    }
}