using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using TrailEntities.Game;
using TrailEntities.Widget;

namespace TrailEntities.Simulation
{
    /// <summary>
    ///     Keeps track of all the possible states a given game mode can have by using attributes and reflection to keep track
    ///     of which user data object gets mapped to which particular state.
    /// </summary>
    public sealed class StateFactory
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailEntities.Simulation.StateFactory" /> class.
        /// </summary>
        public StateFactory()
        {
            // Create dictionaries for reference tracking for what states belong to what game modes.
            LoadedStates = new Dictionary<Tuple<Type, GameMode>, IModeInfo>();

            // Collect all of the states with the custom attribute decorated on them.
            var gameModes = AttributeHelper.GetTypesWith<RequiredModeAttribute>(false);
            foreach (var modeType in gameModes)
            {
                // Get the attribute itself from the state we are working on, which gives us the game mode enum.
                var modeAttribute = modeType.GetAttributes<RequiredModeAttribute>(false).First();
                var modeCategory = modeAttribute.ModeType;

                // Add the state reference list for lookup and instancing later during runtime.
                // TODO: Add reference information object from mode factory via window manager? Or maybe attribute?
                LoadedStates.Add(new Tuple<Type, GameMode>(modeType, modeCategory), null);
            }
        }

        /// <summary>
        ///     Reference dictionary for all the reflected state types.
        /// </summary>
        private Dictionary<Tuple<Type, GameMode>, IModeInfo> LoadedStates { get; set; }

        /// <summary>
        ///     Creates and adds the specified type of state to currently active game mode.
        /// </summary>
        /// <param name="stateType">Type object that is the actual type of state that needs created.</param>
        /// <param name="gameMode">Enumeration value that defines parent game mode type.</param>
        /// <returns>Created state instance from reference types build on startup.</returns>
        public IStateProduct CreateStateFromType(Type stateType, GameMode gameMode)
        {
            // Create tuple we will use as dictionary key.
            var stateKey = new Tuple<Type, GameMode>(stateType, gameMode);

            // Check if the state tuple key exists in our reference list.
            if (!LoadedStates.ContainsKey(stateKey))
                throw new ArgumentException(
                    "State factory cannot create state from type that does not exist in reference states! " +
                    "Perhaps developer forgot [RequiredMode] attribute on state?!");

            // States are based on abstract class, but never should be one.
            if (stateType.IsAbstract)
                return null;

            // Grab the user data object from active mode

            // Create the state, it will have parameterless constructor.
            var stateInstance = Activator.CreateInstance(
                stateType,
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance,
                null,
                new object[] {}, // Parameterless constructor.
                CultureInfo.InvariantCulture);

            // Pass the created state back to caller.
            return stateInstance as IStateProduct;
        }

        /// <summary>
        ///     Called when primary simulation is closing down.
        /// </summary>
        public void Destroy()
        {
            LoadedStates.Clear();
            LoadedStates = null;
        }
    }
}