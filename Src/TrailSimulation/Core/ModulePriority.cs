namespace TrailSimulation.Core
{
    /// <summary>
    ///     Defines the various levels of importance that a module can have alongside the value given to determine what type of
    ///     module it will be in the simulation and where it will sit in the priority queue for ticking.
    /// </summary>
    public enum ModulePriority
    {
        /// <summary>
        ///     Default value and indicates the module will be initialized but never ticked by the simulation.
        /// </summary>
        None = 0,

        /// <summary>
        ///     Lowest priority or last, meaning it will be placed near the end of the collection and ticked after modules with
        ///     higher priorities.
        /// </summary>
        Low = 1,

        /// <summary>
        ///     Normal priority for a module, it will attempt to sit near the middle of the collection.
        /// </summary>
        Normal = 2,

        /// <summary>
        ///     Highest priority that a module can have, it will be shifted towards the front of the list sorted by type of module.
        /// </summary>
        High = 3
    }
}