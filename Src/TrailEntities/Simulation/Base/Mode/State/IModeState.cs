using System;
using System.Collections.Generic;

namespace TrailEntities.Simulation.Mode
{
    /// <summary>
    ///     Defines interface for game mode state which can show data, accept input, add new game modes, set new state, and
    ///     have user data custom per implementation.
    /// </summary>
    public interface IModeState : IComparer<IModeState>, IComparable<IModeState>
    {
        /// <summary>
        ///     Intended to be overridden in abstract class by generics to provide method to return object that contains all the
        ///     data for parent game mode.
        /// </summary>
        IModeInfo UserData { get; }

        /// <summary>
        ///     Determines if user input is currently allowed to be typed and filled into the input buffer.
        /// </summary>
        /// <remarks>Default is FALSE. Setting to TRUE allows characters and input buffer to be read when submitted.</remarks>
        bool AcceptsInput { get; }

        /// <summary>
        ///     Forces the current game mode state to update itself, this typically results in moving to the next state.
        /// </summary>
        void TickState();

        /// <summary>
        ///     Returns a text only representation of the current game mode state. Could be a statement, information, question
        ///     waiting input, etc.
        /// </summary>
        string OnRenderState();

        /// <summary>
        ///     Fired when the game mode current state is not null and input buffer does not match any known command.
        /// </summary>
        /// <param name="input">Contents of the input buffer which didn't match any known command in parent game mode.</param>
        void OnInputBufferReturned(string input);

        /// <summary>
        ///     Creates and adds the specified type of state to currently active game mode.
        /// </summary>
        void SetState(Type stateType);

        /// <summary>
        ///     Removes the current state from the active game mode.
        /// </summary>
        void ClearState();
    }
}