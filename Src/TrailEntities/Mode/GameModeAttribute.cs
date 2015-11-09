using System;

namespace TrailEntities.Mode
{
    /// <summary>
    ///     Intended to be used as decorations on the top of game gameMode classes so the game simulation can create a dictionary
    ///     of all available game GameMode on startup based on what type they are so they can be quickly activated.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class GameModeAttribute : Attribute
    {
        /// <summary>
        ///     Defines what type of game gameMode the attribute is decorated on will represent in the game simulation.
        /// </summary>
        public GameModeAttribute(Type modeCategory, Type commands, Type userData)
        {
            // Complain the generics implemented is not of an enum type.
            if (!commands.IsEnum)
                throw new InvalidCastException(
                    "Commands parameter on game gameMode attribute must be an enumerated type!");

            // Complain if user data is not of class type.
            if (!userData.IsClass || !userData.IsAssignableFrom(typeof (object)))
                throw new InvalidCastException(
                    "Attempted to pass in game gameMode data object that is not inherited directly from object!");

            // Throw the data onto our local variables.
            UserData = userData;
            Mode = modeCategory;
            Commands = commands;
        }

        /// <summary>
        ///     Will hold an enumeration of all the available commands in this game gameMode.
        /// </summary>
        public Type Commands { get; }

        /// <summary>
        ///     Defines the type of object that will be created to act as intermediate object for all game gameMode states. It will be
        ///     created when the game gameMode is attached to the simulation.
        /// </summary>
        public Type UserData { get; }

        /// <summary>
        ///     Defines the actual game gameMode type which will be loaded into the simulation on startup using attributes and
        ///     reflection.
        /// </summary>
        public Type Mode { get; set; }
    }
}