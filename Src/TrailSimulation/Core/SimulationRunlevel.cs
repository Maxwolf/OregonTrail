namespace TrailSimulation.Core
{
    /// <summary>
    ///     Used by core simulation to figure out which modules need to be ticked in system, game, and their order. Effectively
    ///     emulating a UNIX style SystemV init using attributes and reflection to figure out how the simulation will run.
    /// </summary>
    public enum SimulationRunlevel
    {
        /// <summary>
        ///     Shuts the simulation down and destroys all data.
        /// </summary>
        Halt = 0,

        /// <summary>
        ///     Mode for administrative tasks, or when no default mode, or no modes found.
        /// </summary>
        Maintenance = 1,

        /// <summary>
        ///     Collects all simulation modules via reflection and sorts them by priority.
        /// </summary>
        Initialize = 2,

        /// <summary>
        ///     Starts the system normally.
        /// </summary>
        Startup = 3,

        /// <summary>
        ///     For special purposes.
        /// </summary>
        Custom = 4,

        /// <summary>
        ///     Simulation is active and running.
        /// </summary>
        Running = 5,

        /// <summary>
        ///     Restarts the simulation as if it just started.
        /// </summary>
        Restart = 6
    }
}