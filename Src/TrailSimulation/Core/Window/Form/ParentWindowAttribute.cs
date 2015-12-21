// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ParentWindowAttribute.cs" company="Ron 'Maxwolf' McDowell">
//   ron.mcdowell@gmail.com
// </copyright>
// <summary>
//   Used to map game Windows states to their respective parent modes by Windows type enumeration value. All of this is
//   done
//   by the state factory which is called by the base simulation whom also keeps track of all the game modes in similar
//   manner. After startup we will add to that data by telling what possible states may exist for a particular game
//   Windows
//   and what user data IModeInfo object will be created.
// </summary>
// --------------------------------------------------------------------------------------------------------------------



using System;
using TrailSimulation.Game;

namespace TrailSimulation.Core
{
    /// <summary>
    ///     Used to map game Windows states to their respective parent modes by Windows type enumeration value. All of this is
    ///     done
    ///     by the state factory which is called by the base simulation whom also keeps track of all the game modes in similar
    ///     manner. After startup we will add to that data by telling what possible states may exist for a particular game
    ///     Windows
    ///     and what user data IModeInfo object will be created.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public sealed class ParentWindowAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:TrailSimulation.Core.ParentWindowAttribute"/> class.
        /// </summary>
        /// <param name="parentWindow">
        /// The parent Window.
        /// </param>
        public ParentWindowAttribute(GameWindow parentWindow)
        {
            ParentWindow = parentWindow;
        }

        /// <summary>
        ///     Defines what the parent game mode of this particular state should be.
        /// </summary>
        public GameWindow ParentWindow { get; private set; }
    }
}