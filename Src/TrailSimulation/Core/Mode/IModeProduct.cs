using System;
using System.Collections.Generic;
using TrailSimulation.Game;

namespace TrailSimulation.Core
{
    /// <summary>
    ///     Underlying game mode interface, used by base simulation to keep track of what data should currently have control
    ///     over the simulation details. Only top most game mode will ever be ticked.
    /// </summary>
    public interface IModeProduct :
        IComparer<IModeProduct>,
        IComparable<IModeProduct>
    {
        /// <summary>
        ///     Determines if the game mode should not be ticked if it is active but instead removed. The mode when set to being
        ///     removed will not actually be removed until the simulation attempts to tick it and realizes that this is set to true
        ///     and then it will be removed.
        /// </summary>
        bool ShouldRemoveMode { get; }

        /// <summary>
        ///     Defines the type of game mode this is and what it's purpose will be intended for.
        /// </summary>
        Mode Mode { get; }

        /// <summary>
        ///     Determines if user input is currently allowed to be typed and filled into the input buffer.
        /// </summary>
        /// <remarks>Default is FALSE. Setting to TRUE allows characters and input buffer to be read when submitted.</remarks>
        bool AcceptsInput { get; }

        /// <summary>
        ///     Holds the current state which this mode is in, a mode will cycle through available states until it is finished and
        ///     then detach.
        /// </summary>
        IStateProduct CurrentState { get; }

        /// <summary>
        ///     Intended to be overridden in abstract class by generics to provide method to return object that contains all the
        ///     data for parent game mode.
        /// </summary>
        ModeInfo UserData { get; }

        /// <summary>
        ///     Creates and adds the specified type of state to currently active game mode.
        /// </summary>
        void SetState(Type stateType);

        /// <summary>
        ///     Removes the current state from the active game mode.
        /// </summary>
        void ClearState();

        /// <summary>
        ///     Sets the flag for this game mode to be removed the next time it is ticked by the simulation.
        /// </summary>
        void RemoveModeNextTick();

        /// <summary>
        ///     Grabs the text user interface string that will be used for debugging on console application.
        /// </summary>
        string OnRenderMode();

        /// <summary>
        ///     Ticks the internal logic of the game mode so that it may perform linear operations.
        /// </summary>
        void TickMode();

        /// <summary>
        ///     Intended to be used by base simulation to pass along the input buffer after the user has typed several characters
        ///     into the input buffer. This is used when allowing the user to input custom strings like names for their party
        ///     members.
        /// </summary>
        void SendCommand(string command);

        /// <summary>
        ///     Called after the mode has been added to list of modes and made active.
        /// </summary>
        void OnModePostCreate();

        /// <summary>
        ///     Called when the mode manager in simulation makes this mode the currently active game mode. Depending on order of
        ///     modes this might not get called until the mode is actually ticked by the simulation.
        /// </summary>
        void OnModeActivate();
    }
}