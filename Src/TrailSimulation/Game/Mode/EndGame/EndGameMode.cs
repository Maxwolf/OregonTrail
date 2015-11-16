using TrailSimulation.Core;

namespace TrailSimulation.Game
{
    /// <summary>
    ///     Attached when the party leader dies, or the vehicle reaches the end of the trail.
    /// </summary>
    public sealed class EndGameMode : ModeProduct<EndGameCommands, EndGameInfo>
    {
        /// <summary>
        ///     Defines the current game mode the inheriting class is going to take responsibility for when attached to the
        ///     simulation.
        /// </summary>
        public override GameMode GameMode
        {
            get { return GameMode.EndGame; }
        }
    }
}