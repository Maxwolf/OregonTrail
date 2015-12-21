// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WindowFactory.cs" company="Ron 'Maxwolf' McDowell">
//   ron.mcdowell@gmail.com
// </copyright>
// <summary>
//   Factory pattern for creating game modes on the fly during runtime based on enumeration input parameter.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using TrailSimulation.Game;

namespace TrailSimulation.Core
{
    /// <summary>
    ///     Factory pattern for creating game modes on the fly during runtime based on enumeration input parameter.
    /// </summary>
    public sealed class WindowFactory
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="WindowFactory" /> class.
        ///     Creates a new Windows factory that will look over the application for all known game types and create reference
        ///     list
        ///     which we can use to get instances of a given Windows by asking for it.
        /// </summary>
        public WindowFactory()
        {
            // Create dictionaries for holding statistics about times run and for reference loading.
            AttachCount = new Dictionary<GameWindow, int>();
            Windows = new Dictionary<GameWindow, Type>();

            // Loop through every possible game Windows type defined in enumeration.
            foreach (var modeValue in Enum.GetValues(typeof (GameWindow)))
            {
                // Initialize the Windows history dictionary with every game Windows type from enumeration.
                AttachCount.Add((GameWindow) modeValue, 0);

                // GetModule the attribute itself from the Windows we are working on, which gives us the game Windows enum.
                var modeAttribute = ((GameWindow) modeValue).GetEnumAttribute<WindowAttribute>();
                var modeType = modeAttribute.ModeType;

                // Add the game Windows to reference list for lookup and instancing later during runtime.
                Windows.Add((GameWindow) modeValue, modeType);
            }
        }

        /// <summary>
        ///     Reference dictionary for all the found game modes that have the game Windows attribute on top of them which the
        ///     simulation will want to be able to create instances of when running.
        /// </summary>
        private Dictionary<GameWindow, Type> Windows { get; }

        /// <summary>
        ///     Statistics for Windows runtime. Keeps track of how many times a given Windows type was attached to the simulation
        ///     for
        ///     record keeping purposes.
        /// </summary>
        public Dictionary<GameWindow, int> AttachCount { get; }

        /// <summary>Change to new view Windows when told that internal logic wants to display view options to player for a specific set
        ///     of
        ///     data in the simulation.</summary>
        /// <param name="windows">The windows.</param>
        /// <returns>New game Windows instance based on the Windows input parameter.</returns>
        public IWindow CreateWindow(GameWindow windows)
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