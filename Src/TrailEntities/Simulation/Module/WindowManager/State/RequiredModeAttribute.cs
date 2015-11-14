using System;
using TrailEntities.Game;

namespace TrailEntities.Simulation
{
    /// <summary>
    ///     Used to map game mode states to their respective parent modes by mode type enumeration value. All of this is done
    ///     by the state factory which is called by the base simulation whom also keeps track of all the game modes in similar
    ///     manner. After startup we will add to that data by telling what possible states may exist for a particular game mode
    ///     and what user data IModeInfo object will be created.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public sealed class RequiredModeAttribute : Attribute
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:System.Attribute" /> class.
        /// </summary>
        public RequiredModeAttribute(GameMode modeType)
        {
            ModeType = modeType;
        }

        /// <summary>
        ///     Defines what the parent game mode of this particular state should be.
        /// </summary>
        public GameMode ModeType { get; private set; }
    }
}