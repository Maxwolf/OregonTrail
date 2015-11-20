namespace TrailSimulation.Core
{
    /// <summary>
    ///     Defines a module in the simulation. Typically this only has commands for when it is created, destroyed, and ticked
    ///     by the core simulation. The tick itself is not something a module defines but rather the entire simulation
    ///     application since we cannot be sure what type it will be until it is running.
    /// </summary>
    public abstract class SimulationModule
    {
        /// <summary>
        ///     Determines how important this module is to the simulation in regards to when it should be ticked after sorting all
        ///     loaded modules by this priority level.
        /// </summary>
        public abstract ModulePriority Priority { get; }

        /// <summary>
        ///     Holds reference to the type of class that will be treated as a simulation module.
        /// </summary>
        public abstract ModuleCategory Category { get; }

        /// <summary>
        ///     Fired when the simulation is closing and needs to clear out any data structures that it created so the program can
        ///     exit cleanly.
        /// </summary>
        public abstract void OnModuleDestroy();

        /// <summary>
        ///     Fired when the simulation loads and creates the module and allows it to create any data structures it cares about
        ///     without calling constructor.
        /// </summary>
        public abstract void OnModuleCreate();

        /// <summary>
        ///     Fired when the simulation ticks the module that it created inside of itself.
        /// </summary>
        public virtual void Tick()
        {
            // Not every module requires the ability to tick.
        }
    }
}