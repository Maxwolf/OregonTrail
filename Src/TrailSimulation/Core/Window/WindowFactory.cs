using System;
using System.Collections.Generic;
using TrailSimulation.Game;
using TrailSimulation.Utility;

namespace TrailSimulation.Core
{
    /// <summary>
    ///     Factory pattern for creating game modes on the fly during runtime based on enumeration input parameter.
    /// </summary>
    public sealed class WindowFactory
    {
        /// <summary>
        ///     Creates a new Windows factory that will look over the application for all known game types and create reference
        ///     list
        ///     which we can use to get instances of a given Windows by asking for it.
        /// </summary>
        public WindowFactory()
        {
            // Create dictionaries for holding statistics about times run and for reference loading.
            AttachCount = new Dictionary<Windows, int>();
            Windows = new Dictionary<Windows, Type>();

            // Loop through every possible game Windows type defined in enumeration.
            foreach (var modeValue in Enum.GetValues(typeof (Windows)))
            {
                // Initialize the Windows history dictionary with every game Windows type from enumeration.
                AttachCount.Add((Windows) modeValue, 0);

                // GetModule the attribute itself from the Windows we are working on, which gives us the game Windows enum.
                var modeAttribute = ((Windows) modeValue).GetEnumAttribute<WindowAttribute>();
                var modeType = modeAttribute.ModeType;

                // Add the game Windows to reference list for lookup and instancing later during runtime.
                Windows.Add((Windows) modeValue, modeType);
            }
        }

        /// <summary>
        ///     Reference dictionary for all the found game modes that have the game Windows attribute on top of them which the
        ///     simulation will want to be able to create instances of when running.
        /// </summary>
        private Dictionary<Windows, Type> Windows { get; }

        /// <summary>
        ///     Statistics for Windows runtime. Keeps track of how many times a given Windows type was attached to the simulation
        ///     for
        ///     record keeping purposes.
        /// </summary>
        public Dictionary<Windows, int> AttachCount { get; }

        /// <summary>
        ///     Change to new view Windows when told that internal logic wants to display view options to player for a specific set
        ///     of
        ///     data in the simulation.
        /// </summary>
        /// <param name="Windows">Enumeration of the game Windows that requested to be attached.</param>
        /// <returns>New game Windows instance based on the Windows input parameter.</returns>
        public IWindow CreateWindow(Windows windows)
        {
            // Grab the game Windows type reference from inputted Windows type enum.
            var modeType = Windows[windows];

            // Check if the class is abstract base class, we don't want to add that.
            if (modeType.IsAbstract)
                return null;

            // Increment the history for loading this type of Windows.
            AttachCount[windows]++;

            // Create the game Windows, it will have single parameter for user data.
            var gameModeInstance = Activator.CreateInstance(modeType);
            return gameModeInstance as IWindow;
        }

        /// <summary>
        ///     Called when the simulation is closing down.
        /// </summary>
        public void Destroy()
        {
            Windows.Clear();
            AttachCount.Clear();
        }
    }
}