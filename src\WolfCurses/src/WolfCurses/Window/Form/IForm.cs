// Created by Ron 'Maxwolf' McDowell (ron.mcdowell@gmail.com) 
// Timestamp 12/31/2015@4:49 AM

namespace SimUnit.Form
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    ///     Defines interface for game mode state which can show data, accept input, add new game modes, set new state, and
    ///     have user data custom per implementation.
    /// </summary>
    public interface IForm : IComparer<IForm>, IComparable<IForm>, ITick
    {
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
        ///     Intended to be overridden in abstract class by generics to provide method to return object that contains all the
        ///     data for parent game Windows.
        /// </summary>
        [SuppressMessage("ReSharper", "UnusedMember.Global")]
        WindowData UserData { get; }

        /// <summary>
        ///     Returns a text only representation of the current game Windows state. Could be a statement, information, question
        ///     waiting input, etc.
        /// </summary>
        /// <returns>
        ///     The text user interface.<see cref="string" />.
        /// </returns>
        string OnRenderForm();

        /// <summary>Fired when the game Windows current state is not null and input buffer does not match any known command.</summary>
        /// <param name="input">Contents of the input buffer which didn't match any known command in parent game Windows.</param>
        void OnInputBufferReturned(string input);

        /// <summary>
        ///     Fired after the state has been completely attached to the simulation letting the state know it can browse the user
        ///     data and other properties below it.
        /// </summary>
        void OnFormPostCreate();

        /// <summary>
        ///     Fired when the window is activated and or refocused after another window was removed from being on-top of it.
        ///     Useful for re-initializing form data after something like a random event runs which might kill people or alter the
        ///     vehicle inventory.
        /// </summary>
        void OnFormActivate();
    }
}