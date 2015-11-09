using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using TrailEntities.Widget;

namespace TrailEntities.Mode
{
    /// <summary>
    ///     Creates new game modes for the simulation using reflection and custom attributes to collect all of the known types
    ///     during runtime.
    /// </summary>
    public sealed class ModeFactory
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailEntities.Mode.ModeFactory" /> class.
        /// </summary>
        public ModeFactory()
        {
            // Dictionary of types sorted by what type they claim responsibility for.
            Modes = new SortedDictionary<GameMode, Type>();

            // Grab all the game GameMode we can use via attributes.
            PopulateModes();
        }

        /// <summary>
        ///     Provides a constant reference list of all currently available game GameMode at runtime, created during simulation
        ///     startup.
        /// </summary>
        private SortedDictionary<GameMode, Type> Modes { get; }

        /// <summary>
        ///     Figures out what game GameMode the simulation has available to it in the form of system types. Once it generates a
        ///     list they will be checked and then added to internal list of game GameMode we can reference at runtime for on the
        ///     fly creation.
        /// </summary>
        private void PopulateModes()
        {
            // Loop through all the enum values for game GameMode.
            foreach (var value in Enum.GetValues(typeof (GameMode)))
            {
                // Grab game gameMode attribute from casted game gameMode enum value.
                var enumAttribute = ((GameMode) value).GetAttribute<GameModeAttribute>();
                if (enumAttribute == null)
                    throw new InvalidCastException("Failed to get attribute from gameMode enumeration value!");

                // Add the verified game gameMode to the list of reference modes.
                Modes.Add(((GameMode) value), enumAttribute.Mode);
            }
        }

        /// <summary>
        ///     Checks to reference modes for matching gameMode type, if found will create and activate class for the gameMode,
        ///     user info object, and commands for the incoming game gameMode.
        /// </summary>
        /// <returns>Fully created and activated game gameMode ready to be added to simulation active modes list.</returns>
        internal ModeProduct CreateInstance(GameMode mode)
        {
            // Check if the key for the desired game gameMode exists in reference list.
            if (!Modes.ContainsKey(mode))
                return null;

            // Grab game gameMode attribute from casted game gameMode enum value.
            var enumAttribute = mode.GetAttribute<GameModeAttribute>();
            if (enumAttribute == null)
                throw new InvalidCastException("Failed to get attribute from gameMode enumeration value!");

            // Fetch all of the commands by name specified in the enumeration.
            var modeCommands = Enum.GetValues(enumAttribute.Commands);

            // Create user data for the game gameMode that will be passed around it's internal states.
            var userData = CreateUserDataByType(enumAttribute.UserData);
            if (userData == null)
                throw new InvalidCastException("Failed to create gameMode user data object from type!");

            // Create the user data object from the specified type.
            var gameMode = CreateModeByType(enumAttribute.Mode, modeCommands, userData);
            if (gameMode == null)
                throw new InvalidCastException("Failed to create game gameMode from type!");

            // Returns the game gameMode so it may be attached to the running simulation.
            return gameMode;
        }

        /// <summary>
        ///     Creates a new mode product from type information, allows for user data and commands to be sent via reflection from
        ///     attribute.
        /// </summary>
        /// <param name="modeType">The type of game mode that should be created and activated.</param>
        /// <param name="modeCommands">Array of all the enumeration values that make up the commands for this gameMode.</param>
        /// <param name="modeInfo">User data object that is attached to game gameMode </param>
        private static ModeProduct CreateModeByType(Type modeType, Array modeCommands, object modeInfo)
        {
            // Check if the class is abstract base class, we don't want to add that.
            if (modeType.IsAbstract)
                return null;

            // Get the constructor and create an instance object type.
            var instantiatedType = Activator.CreateInstance(
                modeType,
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance,
                null,
                new[]
                {
                    false, // TODO: Replace with debugger attached flag?!
                    modeCommands,
                    modeInfo
                },
                CultureInfo.InvariantCulture);

            return instantiatedType as ModeProduct;
        }

        /// <summary>
        ///     Creates the custom user data object for the game gameMode and it's states.
        /// </summary>
        private static object CreateUserDataByType(Type objectType)
        {
            // Check if the class is abstract base class, we don't want to add that.
            if (objectType.IsAbstract)
                return null;

            // Get the constructor and create an instance object type.
            var instantiatedType = Activator.CreateInstance(
                objectType,
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance,
                null,
                new object[] {objectType.Name}, // Constructor with 1 parameter...
                CultureInfo.InvariantCulture);

            return instantiatedType;
        }
    }
}