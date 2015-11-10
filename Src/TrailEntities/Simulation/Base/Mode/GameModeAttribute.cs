using System;

namespace TrailEntities.Simulation.Mode
{
    /// <summary>
    ///     Intended to be used as decorations on the top of game mode classes so the game simulation can create a dictionary
    ///     of all available game modes on startup based on what type they are so they can be quickly activated.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class GameModeAttribute : Attribute
    {
        /// <summary>
        ///     Defines what type of game mode the attribute is decorated on will represent in the game simulation.
        /// </summary>
        public GameModeAttribute(ModeType modeType)
        {
            ModeType = modeType;
        }

        /// <summary>
        ///     Defines what type of game mode the attribute is decorated on will represent in the game simulation.
        /// </summary>
        public ModeType ModeType { get; private set; }
    }
}