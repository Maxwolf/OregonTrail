// Created by Ron 'Maxwolf' McDowell (ron.mcdowell@gmail.com) 
// Timestamp 11/19/2015@6:59 PM

namespace TrailSimulation.Core
{
    using System;

    /// <summary>
    ///     Defines a choice in the dynamic action selection system for a given game Windows. This is intended to be used by a
    ///     wrapper for menu choices that aggregates all of the possible actions a given game Windows can make while it is
    ///     active
    ///     in the simulation.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IMenuChoice<T> where T : struct, IComparable, IFormattable, IConvertible
    {
        /// <summary>
        ///     Gets or sets the command.
        /// </summary>
        T Command { get; set; }

        /// <summary>
        ///     Gets or sets the description.
        /// </summary>
        string Description { get; set; }

        /// <summary>
        ///     Gets or sets the action.
        /// </summary>
        Action Action { get; set; }
    }
}