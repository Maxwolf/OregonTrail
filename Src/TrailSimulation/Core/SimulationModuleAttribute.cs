using System;

namespace TrailSimulation.Core
{
    /// <summary>
    ///     Allows the core simulation to query all known modules that need ticking.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public sealed class SimulationModuleAttribute : Attribute
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailSimulation.Core.SimulationModuleAttribute" /> class.
        /// </summary>
        public SimulationModuleAttribute(Type moduleType)
        {
            // Complain if the type sent is not an implementation of simulation module base abstract class.
            if (!moduleType.IsImplementationOf(typeof (SimulationModule)))
                throw new ArgumentException(
                    "Simulation module attribute was used on a class that does not inherit from SimulationModule!");

            // Apply the type information to our property so mode factory can see it.
            ModuleType = moduleType;
        }

        /// <summary>
        ///     Holds reference to the type of class that will be treated as a simulation module.
        /// </summary>
        public Type ModuleType { get; private set; }
    }
}