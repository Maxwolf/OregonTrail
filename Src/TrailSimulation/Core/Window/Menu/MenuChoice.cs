// Created by Ron 'Maxwolf' McDowell (ron.mcdowell@gmail.com) 
// Timestamp 11/19/2015@6:59 PM

namespace TrailSimulation.Core
{
    using System;

    /// <summary>
    ///     Defines a choice in the dynamic action selection system for a given game mode. This is intended to be used by a
    ///     wrapper for menu choices that aggregates all of the possible actions a given game mode can make while it is active
    ///     in the simulation.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class MenuChoice<T> : IMenuChoice<T> where T : struct, IComparable, IFormattable, IConvertible
    {
        /// <summary>Initializes a new instance of the <see cref="MenuChoice{T}" /> class.</summary>
        /// <param name="command">The command.</param>
        /// <param name="action">The action.</param>
        /// <param name="description">The description.</param>
        /// <exception cref="InvalidCastException"></exception>
        public MenuChoice(T command, Action action, string description)
        {
            // Complain the generics implemented is not of an enum type.
            if (!typeof (T).IsEnum)
            {
                throw new InvalidCastException("T must be an enumerated type!");
            }

            Command = command;
            Action = action;
            Description = description;
        }

        /// <summary>
        ///     Gets or sets the command.
        /// </summary>
        public T Command { get; set; }

        /// <summary>
        ///     Gets or sets the description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        ///     Gets or sets the action.
        /// </summary>
        public Action Action { get; set; }
    }
}