using System;
using System.Collections.Generic;
using TrailSimulation.Game;
using TrailSimulation.Utility;

namespace TrailSimulation.Core
{
    /// <summary>
    ///     Factory pattern for creating game modes on the fly during runtime based on enumeration input parameter.
    /// </summary>
    public sealed class ModeFactory
    {
        /// <summary>
        ///     Creates a new mode factory that will look over the application for all known game types and create reference list
        ///     which we can use to get instances of a given mode by asking for it.
        /// </summary>
        public ModeFactory()
        {
            // Create dictionaries for holding statistics about times run and for reference loading.
            AttachCount = new Dictionary<Mode, int>();
            Modes = new Dictionary<Mode, Type>();

            // Loop through every possible game mode type defined in enumeration.
            foreach (var modeValue in Enum.GetValues(typeof (Mode)))
            {
                // Initialize the mode history dictionary with every game mode type from enumeration.
                AttachCount.Add((Mode) modeValue, 0);

                // GetModule the attribute itself from the mode we are working on, which gives us the game mode enum.
                var modeAttribute = ((Mode) modeValue).GetEnumAttribute<SimulationModeAttribute>();
                var modeType = modeAttribute.ModeType;

                // Add the game mode to reference list for lookup and instancing later during runtime.
                Modes.Add((Mode) modeValue, modeType);
            }
        }

        /// <summary>
        ///     Reference dictionary for all the found game modes that have the game mode attribute on top of them which the
        ///     simulation will want to be able to create instances of when running.
        /// </summary>
        private Dictionary<Mode, Type> Modes { get; }

        /// <summary>
        ///     Statistics for mode runtime. Keeps track of how many times a given mode type was attached to the simulation for
        ///     record keeping purposes.
        /// </summary>
        public Dictionary<Mode, int> AttachCount { get; }

        /// <summary>
        ///     Change to new view mode when told that internal logic wants to display view options to player for a specific set of
        ///     data in the simulation.
        /// </summary>
        /// <param name="mode">Enumeration of the game mode that requested to be attached.</param>
        /// <returns>New game mode instance based on the mode input parameter.</returns>
        public IModeProduct CreateMode(Mode mode)
        {
            // Grab the game mode type reference from inputted mode type enum.
            var modeType = Modes[mode];

            // Check if the class is abstract base class, we don't want to add that.
            if (modeType.IsAbstract)
                return null;

            // Increment the history for loading this type of mode.
            AttachCount[mode]++;

            // Create the game mode, it will have single parameter for user data.
            var gameModeInstance = Activator.CreateInstance(modeType);
            return gameModeInstance as IModeProduct;
        }

        /// <summary>
        ///     Called when the simulation is closing down.
        /// </summary>
        public void Destroy()
        {
            Modes.Clear();
            AttachCount.Clear();
        }
    }
}