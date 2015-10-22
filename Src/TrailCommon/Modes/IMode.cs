using System;
using System.Collections.ObjectModel;

namespace TrailCommon
{
    /// <summary>
    ///     Underlying game mode interface, used by base simulation to keep track of what data should currently have control
    ///     over the simulation details. Only top most game mode will ever be ticked.
    /// </summary>
    public interface IMode<T> where T : struct, IComparable, IFormattable, IConvertible
    {
        /// <summary>
        ///     Defines the type of game mode this is and what it's purpose will be intended for.
        /// </summary>
        SimulationMode Mode { get; }

        /// <summary>
        ///     Reference to all of the possible commands that this game mode supports routing back to the game simulation that
        ///     spawned it.
        /// </summary>
        ReadOnlyCollection<IModeChoice<T>> MenuChoices { get; }

        /// <summary>
        ///     Grabs the text user interface string that will be used for debugging on console application.
        /// </summary>
        /// <returns></returns>
        string GetTUI();

        /// <summary>
        ///     Ticks the internal logic of the game mode so that it may perform linear operations.
        /// </summary>
        void TickMode();

        /// <summary>
        ///     Intended to be used by base simulation to pass along the input buffer after the user has typed several characters
        ///     into the input buffer. This is used when allowing the user to input custom strings like names for their party
        ///     members.
        /// </summary>
        void ProcessCommand(string returnedLine);

        /// <summary>
        ///     Adds a new game mode menu selection that will be available to send as a command for this specific game mode.
        /// </summary>
        /// <param name="action">Method that will be run when the choice is made.</param>
        /// <param name="command">Associated command that will trigger the respective action in the active game mode.</param>
        /// <param name="description">Text that will be shown to user so they know what the choice means.</param>
        void AddCommand(Action action, T command, string description);
    }
}