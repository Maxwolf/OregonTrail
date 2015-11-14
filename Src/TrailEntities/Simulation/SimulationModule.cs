namespace TrailEntities.Simulation
{
    /// <summary>
    ///     Defines a module in the simulation. Typically this only has commands for when it is created, destroyed, and ticked
    ///     by the core simulation. The tick itself is not something a module defines but rather the entire simulation
    ///     application since we cannot be sure what type it will be until it is running.
    /// </summary>
    public abstract class SimulationModule
    {
        /// <summary>
        ///     Fired when the simulation is closing and needs to clear out any data structures that it created so the program can
        ///     exit cleanly.
        /// </summary>
        public abstract void Destroy();

        /// <summary>
        ///     Fired when the simulation ticks the module that it created inside of itself.
        /// </summary>
        public abstract void Tick();
    }
}