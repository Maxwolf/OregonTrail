using System;
using TrailSimulation.Utility;

namespace TrailSimulation.Core
{
    /// <summary>
    ///     Allows the simulation to reflect over the mode type enumeration and map classes to a given enum value.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public sealed class SimulationModeAttribute : Attribute
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailSimulation.Core.SimulationModeAttribute" /> class.
        /// </summary>
        public SimulationModeAttribute(Type modeType)
        {
            // Complain if the type sent is not an implementation of game mode interface.
            if (!modeType.IsImplementationOf(typeof (IModeProduct)))
                throw new ArgumentException(
                    "Game mode attribute was used on a class that does not inherit from IModeProduct!");

            // Apply the type information to our property so mode factory can see it.
            ModeType = modeType;
        }

        /// <summary>
        ///     Holds reference to the type of class that will be treated as a game mode.
        /// </summary>
        public Type ModeType { get; private set; }
    }
}