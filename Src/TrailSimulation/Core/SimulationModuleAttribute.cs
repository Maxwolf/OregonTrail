using System;

namespace TrailSimulation.Core
{
    /// <summary>
    ///     Allows the simulation to query all known modules that need ticking. Will prioritize and sort list of modules to be
    ///     ticked over so they can do work, attach modes, accept input, etc.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public sealed class SimulationModuleAttribute : Attribute
    {
    }
}