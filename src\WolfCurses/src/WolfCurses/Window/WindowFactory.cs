// Created by Ron 'Maxwolf' McDowell (ron.mcdowell@gmail.com) 
// Timestamp 12/31/2015@4:49 AM

namespace SimUnit
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    ///     Factory pattern for creating game modes on the fly during runtime based on enumeration input parameter.
    /// </summary>
    public sealed class WindowFactory
    {
        /// <summary>
        ///     Reference to running game simulation we will need to pass along to created window instances.
        /// </summary>
        private readonly SimulationApp _simUnit;

        /// <summary>
        ///     Initializes a new instance of the <see cref="WindowFactory" /> class.
        ///     Creates a new Windows factory that will look over the application for all known game types and create reference
        ///     list which we can use to get instances of a given Windows by asking for it.
        /// </summary>
        /// <param name="simUnit">Core simulation which is controlling the window factory.</param>
        public WindowFactory(SimulationApp simUnit)
        {
            // Copy over reference for simulation core.
            _simUnit = simUnit;

            // Create dictionaries for holding statistics about times run and for reference loading.
            Windows = new Dictionary<string, Type>();

            // Loop through every possible game Windows type defined in enumeration.
            foreach (var window in simUnit.AllowedWindows)
            {
                // Add the game Windows to reference list for lookup and instancing later during runtime.
                Windows.Add(window.Name, window);
            }
        }

        /// <summary>
        ///     Reference dictionary for all the found game modes that have the game Windows attribute on top of them which the
        ///     simulation will want to be able to create instances of when running.
        /// </summary>
        private Dictionary<string, Type> Windows { get; }

        /// <summary>
        ///     Change to new view Windows when told that internal logic wants to display view options to player for a specific set
        ///     of data in the simulation.
        /// </summary>
        /// <param name="window">The windows.</param>
        /// <returns>New game Windows instance based on the Windows input parameter.</returns>
        public IWindow CreateWindow(Type window)
        {
            // Grab the game Windows type reference from inputted Windows type enum.
            var modeType = Windows[window.Name];

            // Check if the class is abstract base class, we don't want to add that.
            if (modeType.IsAbstract)
                return null;

            // Create the game Windows, it will have single parameter for user data.
            var gameModeInstance = Activator.CreateInstance(modeType, _simUnit);
            return gameModeInstance as IWindow;
        }

        /// <summary>
        ///     Called when the simulation is closing down.
        /// </summary>
        public void Destroy()
        {
            Windows.Clear();
        }
    }
}