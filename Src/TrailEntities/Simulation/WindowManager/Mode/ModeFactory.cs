using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using TrailEntities.Widget;

namespace TrailEntities.Simulation
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
            RunCount = new Dictionary<ModeType, int>();
            LoadedModes = new Dictionary<ModeType, Type>();

            // Initialize the mode history dictionary with every game mode type from enumeration.
            foreach (var modeType in Enum.GetValues(typeof (ModeType)))
            {
                RunCount.Add((ModeType) modeType, 0);
            }

            // Collect all of the game modes with the attribute decorated on them.
            var gameModes = AttributeHelper.GetTypesWith<GameModeAttribute>(false);
            foreach (var modeType in gameModes)
            {
                // Get the attribute itself from the mode we are working on, which gives us the game mode enum.
                var modeAttribute = modeType.GetAttributes<GameModeAttribute>(false).First();
                var modeCategory = modeAttribute.ModeType;

                // Add the game mode to reference list for lookup and instancing later during runtime.
                LoadedModes.Add(modeCategory, modeType);
            }
        }

        /// <summary>
        ///     Reference dictionary for all the found game modes that have the game mode attribute on top of them which the
        ///     simulation will want to be able to create instances of when running.
        /// </summary>
        private Dictionary<ModeType, Type> LoadedModes { get; }

        /// <summary>
        ///     Statistics for mode runtime. Keeps track of how many times a given mode type was attached to the simulation for
        ///     record keeping purposes.
        /// </summary>
        public Dictionary<ModeType, int> RunCount { get; }

        /// <summary>
        ///     Change to new view mode when told that internal logic wants to display view options to player for a specific set of
        ///     data in the simulation.
        /// </summary>
        /// <param name="modeType">Enumeration of the game mode that requested to be attached.</param>
        /// <returns>New game mode instance based on the mode input parameter.</returns>
        public IModeProduct CreateMode(ModeType modeType)
        {
            // Grab the game mode type reference from inputted mode type enum.
            var modeToSpawn = LoadedModes[modeType];

            // Check if the class is abstract base class, we don't want to add that.
            if (modeToSpawn.IsAbstract)
                return null;

            // Increment the history for loading this type of mode.
            RunCount[modeType]++;

            // Create the game mode, it will have parameterless constructor.
            var gameModeInstance = Activator.CreateInstance(
                modeToSpawn,
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance,
                null,
                new object[] {}, // Parameterless constructor.
                CultureInfo.InvariantCulture);

            return gameModeInstance as IModeProduct;
        }

        public void Destroy()
        {
            LoadedModes.Clear();
            RunCount.Clear();
        }
    }
}