// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WindowAttribute.cs" company="Ron 'Maxwolf' McDowell">
//   ron.mcdowell@gmail.com
// </copyright>
// <summary>
//   Allows the simulation to reflect over the Windows type enumeration and map classes to a given enum value.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace TrailSimulation.Core
{
    using System;

    /// <summary>
    ///     Allows the simulation to reflect over the Windows type enumeration and map classes to a given enum value.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public sealed class WindowAttribute : Attribute
    {
        /// <summary>Initializes a new instance of the <see cref="T:TrailSimulation.Core.WindowAttribute"/> class.</summary>
        /// <param name="modeType">The mode Type.</param>
        public WindowAttribute(Type modeType)
        {
            // Complain if the type sent is not an implementation of game Windows interface.
            if (!modeType.IsImplementationOf(typeof (IWindow)))
                throw new ArgumentException(
                    "Game Windows attribute was used on a class that does not inherit from IWindow!");

            // Apply the type information to our property so Windows factory can see it.
            ModeType = modeType;
        }

        /// <summary>
        ///     Holds reference to the type of class that will be treated as a game Windows.
        /// </summary>
        public Type ModeType { get; private set; }
    }
}