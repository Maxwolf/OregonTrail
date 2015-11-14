using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using TrailEntities.Game;
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
            RunCount = new Dictionary<GameMode, int>();
            LoadedModes = new Dictionary<GameMode, Type>();

            // Loop through every possible game mode type defined in enumeration.
            foreach (var modeValue in Enum.GetValues(typeof (GameMode)))
            {
                // Initialize the mode history dictionary with every game mode type from enumeration.
                RunCount.Add((GameMode) modeValue, 0);

                // Get the attribute itself from the mode we are working on, which gives us the game mode enum.
                var modeAttribute = ((GameMode) modeValue).GetEnumAttribute<GameModeAttribute>();
                var modeType = modeAttribute.ModeType;

                // Add the game mode to reference list for lookup and instancing later during runtime.
                LoadedModes.Add((GameMode) modeValue, modeType);
            }
        }

        /// <summary>
        ///     Reference dictionary for all the found game modes that have the game mode attribute on top of them which the
        ///     simulation will want to be able to create instances of when running.
        /// </summary>
        private Dictionary<GameMode, Type> LoadedModes { get; }

        /// <summary>
        ///     Statistics for mode runtime. Keeps track of how many times a given mode type was attached to the simulation for
        ///     record keeping purposes.
        /// </summary>
        public Dictionary<GameMode, int> RunCount { get; }

        /// <summary>
        ///     Change to new view mode when told that internal logic wants to display view options to player for a specific set of
        ///     data in the simulation.
        /// </summary>
        /// <param name="gameMode">Enumeration of the game mode that requested to be attached.</param>
        /// <returns>New game mode instance based on the mode input parameter.</returns>
        public IModeProduct CreateMode(GameMode gameMode)
        {
            // Grab the game mode type reference from inputted mode type enum.
            var modeToSpawn = LoadedModes[gameMode];

            // Check if the class is abstract base class, we don't want to add that.
            if (modeToSpawn.IsAbstract)
                return null;

            // Increment the history for loading this type of mode.
            RunCount[gameMode]++;

            // Create the game mode, it will have parameterless constructor.
            var gameModeInstance = Activator.CreateInstance(
                modeToSpawn,
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance,
                null,
                new object[]
                {
                    
                },
                CultureInfo.InvariantCulture);

            return gameModeInstance as IModeProduct;
        }

        /// <summary>
        ///     Called when the simulation is closing down.
        /// </summary>
        public void Destroy()
        {
            LoadedModes.Clear();
            RunCount.Clear();
        }
    }
}