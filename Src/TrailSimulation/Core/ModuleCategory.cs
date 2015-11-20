namespace TrailSimulation.Core
{
    /// <summary>
    ///     Defines what type of module this will be in the simulation. It's important to keep track of this since we build up
    ///     the entire representation of what needs to be ticked via attributes and reflection for a dynamic factory pattern.
    /// </summary>
    public enum ModuleCategory
    {
        /// <summary>
        ///     Core modules should not have anything to do with the application or game being created with this framework. They
        ///     should consist of functionality that makes up the base simulation which all game modules might potentially need
        ///     access to depending on how their hierarchy is laid out and prioritized by module factory.
        /// </summary>
        Core = 1,

        /// <summary>
        ///     Application module that sits on top of the core modules to provide the features, menus, modes, and states that make
        ///     the simulation unique to every application created with it.
        /// </summary>
        Application = 2
    }
}