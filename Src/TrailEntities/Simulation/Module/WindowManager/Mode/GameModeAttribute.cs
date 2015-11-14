using System;

namespace TrailEntities.Simulation
{
    /// <summary>
    ///     Allows the simulation to reflect over the mode type enumeration and map classes to a given enum value.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class GameModeAttribute : Attribute
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailEntities.Simulation.GameModeAttribute" /> class.
        /// </summary>
        public GameModeAttribute(Type modeType)
        {
            // Complain if type does not subclass to interface we want.
            if (!modeType.IsAssignableFrom(typeof (IModeProduct)))
                throw new ArgumentException("Unable to cast game mode enumeration attribute type to IModeProduct!");

            // Apply the type information to our property so mode factory can see it.
            ModeType = modeType;
        }

        /// <summary>
        ///     Holds reference to the type of class that will be treated as a game mode.
        /// </summary>
        public Type ModeType { get; private set; }
    }
}