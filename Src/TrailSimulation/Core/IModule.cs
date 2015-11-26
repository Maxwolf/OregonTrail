namespace TrailSimulation.Core
{
    public interface IModule : ITick
    {
        /// <summary>
        ///     Fired when the simulation is closing and needs to clear out any data structures that it created so the program can
        ///     exit cleanly.
        /// </summary>
        void Destroy();
    }
}