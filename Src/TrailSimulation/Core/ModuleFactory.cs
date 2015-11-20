using System.Collections.Generic;
using System.Linq;

namespace TrailSimulation.Core
{
    /// <summary>
    ///     Creates and activates modules for the simulation on initialization
    /// </summary>
    public sealed class ModuleFactory
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailSimulation.Core.ModuleFactory" /> class.
        /// </summary>
        public ModuleFactory()
        {
            var foundModules = AttributeHelper.GetTypesWith<SimulationModuleAttribute>(false);
            foreach (var stateType in foundModules)
            {
                var moduleAttribute = stateType.GetAttributes<SimulationModuleAttribute>(false).First();
                //moduleAttribute.Priority
                //moduleAttribute.Category
            }
        }

        /// <summary>
        ///     References all of the located and safely casted module objects.
        /// </summary>
        private Dictionary<SimulationModule, ModuleCategory> Modules { get; }
    }
}