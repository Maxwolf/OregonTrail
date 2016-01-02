// Created by Ron 'Maxwolf' McDowell (ron.mcdowell@gmail.com) 
// Timestamp 12/31/2015@4:49 AM

namespace SimUnit
{
    using System;
    using System.Collections.Generic;
    using Form;

    /// <summary>
    ///     Underlying game Windows interface, used by base simulation to keep track of what data should currently have control
    ///     over the simulation details. Only top most game Windows will ever be ticked.
    /// </summary>
    public interface IWindow :
        IComparer<IWindow>,
        IComparable<IWindow>,
        ITick
    {
        /// <summary>
        ///     Determines if the game Windows should not be ticked if it is active but instead removed. The Windows when set to
        ///     being
        ///     removed will not actually be removed until the simulation attempts to tick it and realizes that this is set to true
        ///     and then it will be removed.
        /// </summary>
        bool ShouldRemoveMode { get; }

        /// <summary>
        ///     Determines if user input is currently allowed to be typed and filled into the input buffer.
        /// </summary>
        /// <remarks>Default is FALSE. Setting to TRUE allows characters and input buffer to be read when submitted.</remarks>
        bool AcceptsInput { get; }

        /// <summary>
        ///     Holds the current state which this Windows is in, a Windows will cycle through available states until it is
        ///     finished and
        ///     then detach.
        /// </summary>
        IForm CurrentForm { get; }

        /// <summary>
        ///     Intended to be overridden in abstract class by generics to provide method to return object that contains all the
        ///     data for parent game Windows.
        /// </summary>
        WindowData UserData { get; }

        /// <summary>Creates and adds the specified type of state to currently active game Windows.</summary>
        /// <param name="stateType">The state Type.</param>
        void SetForm(Type stateType);

        /// <summary>
        ///     Removes the current state from the active game Windows.
        /// </summary>
        void ClearForm();

        /// <summary>
        ///     Sets the flag for this game Windows to be removed the next time it is ticked by the simulation.
        /// </summary>
        void RemoveWindowNextTick();

        /// <summary>
        ///     Grabs the text user interface string that will be used for debugging on console application.
        /// </summary>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        string OnRenderWindow();

        /// <summary>
        ///     Intended to be used by base simulation to pass along the input buffer after the user has typed several characters
        ///     into the input buffer. This is used when allowing the user to input custom strings like names for their party
        ///     members.
        /// </summary>
        /// <param name="input">The input.</param>
        void SendCommand(string input);

        /// <summary>
        ///     Called after the Windows has been added to list of modes and made active.
        /// </summary>
        void OnWindowPostCreate();

        /// <summary>
        ///     Called when the Windows manager in simulation makes this Windows the currently active game Windows. Depending on
        ///     order of
        ///     modes this might not get called until the Windows is actually ticked by the simulation.
        /// </summary>
        void OnWindowActivate();

        /// <summary>
        ///     Fired when the simulation adds a game Windows that is not this Windows. Used to execute code in other modes that
        ///     are not
        ///     the active Windows anymore one last time.
        /// </summary>
        void OnWindowAdded();
    }
}