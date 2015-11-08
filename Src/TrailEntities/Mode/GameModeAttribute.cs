using System;

namespace TrailEntities.Mode
{
    /// <summary>
    ///     Intended to be used as decorations on the top of game mode classes so the game simulation can create a dictionary
    ///     of all available game modes on startup based on what type they are so they can be quickly activated.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class GameModeAttribute : Attribute
    {
        /// <summary>
        ///     Will hold an enumeration of all the available commands in this game mode.
        /// </summary>
        private Type _commands;

        /// <summary>
        ///     Defines the type of object that will be created to act as intermediate object for all game mode states. It will be
        ///     created when the game mode is attached to the simulation.
        /// </summary>
        private Type _modeInfo;

        /// <summary>
        ///     Defines what type of game mode the attribute is decorated on will represent in the game simulation.
        /// </summary>
        private ModeCategory _modeType;

        /// <summary>
        ///     Defines what type of game mode the attribute is decorated on will represent in the game simulation.
        /// </summary>
        public GameModeAttribute(ModeCategory modeCategory, Type commands, Type userData)
        {
            // Complain the generics implemented is not of an enum type.
            if (!commands.IsEnum)
                throw new InvalidCastException(
                    "Commands parameter on game mode attribute must be an enumerated type!");

            // Complain if user data is not of class type.
            if (!userData.IsClass || !userData.IsAssignableFrom(typeof (object)))
                throw new InvalidCastException(
                    "Attempted to pass in game mode data object that is not inherited directly from object!");

            // Throw the data onto our local variables.
            _modeInfo = userData;
            _modeType = modeCategory;
            _commands = commands;
        }
    }
}