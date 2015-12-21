// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IForm.cs" company="Ron 'Maxwolf' McDowell">
//   ron.mcdowell@gmail.com
// </copyright>
// <summary>
//   Defines interface for game mode state which can show data, accept input, add new game modes, set new state, and
//   have user data custom per implementation.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace TrailSimulation.Core
{
    /// <summary>
    ///     Defines interface for game mode state which can show data, accept input, add new game modes, set new state, and
    ///     have user data custom per implementation.
    /// </summary>
    public interface IForm : IComparer<IForm>, IComparable<IForm>, ITick
    {
        /// <summary>
        ///     Intended to be overridden in abstract class by generics to provide method to return object that contains all the
        ///     data for parent game Windows.
        /// </summary>
        WindowData UserData { get; }

        /// <summary>
        ///     Determines if user input is currently allowed to be typed and filled into the input buffer.
        /// </summary>
        /// <remarks>Default is FALSE. Setting to TRUE allows characters and input buffer to be read when submitted.</remarks>
        bool InputFillsBuffer { get; }

        /// <summary>
        ///     Determines if this dialog state is allowed to receive any input at all, even empty line returns. This is useful for
        ///     preventing the player from leaving a particular dialog until you are ready or finished processing some data.
        /// </summary>
        bool AllowInput { get; }

        /// <summary>
        ///     Returns a text only representation of the current game Windows state. Could be a statement, information, question
        ///     waiting input, etc.
        /// </summary>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        string OnRenderForm();

        /// <summary>Fired when the game Windows current state is not null and input buffer does not match any known command.</summary>
        /// <param name="input">Contents of the input buffer which didn't match any known command in parent game Windows.</param>
        void OnInputBufferReturned(string input);

        /// <summary>Creates and adds the specified type of state to currently active game Windows.</summary>
        /// <param name="stateType">The state Type.</param>
        void SetForm(Type stateType);

        /// <summary>
        ///     Removes the current state from the active game Windows.
        /// </summary>
        void ClearForm();

        /// <summary>
        ///     Fired after the state has been completely attached to the simulation letting the state know it can browse the user
        ///     data and other properties below it.
        /// </summary>
        void OnFormPostCreate();
    }
}