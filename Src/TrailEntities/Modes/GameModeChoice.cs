using System;
using TrailCommon;

namespace TrailEntities
{
    /// <summary>
    ///     Defines a choice in the dynamic action selection system for a given game mode. This is intended to be used by a
    ///     wrapper for menu choices that aggregates all of the possible actions a given game mode can make while it is active
    ///     in the simulation.
    /// </summary>
    public abstract class GameModeChoice<T> where T : struct, IComparable, IFormattable, IConvertible, IModeChoice<T>
    {
        protected GameModeChoice(Action action, string description, T command)
        {
            // Complain the generics implemented is not of an enum type.
            if (!typeof (T).IsEnum)
            {
                throw new InvalidCastException("T must be an enumerated type!");
            }

            Command = command;
            Description = description;
            Action = action;
        }

        public T Command { get; set; }
        public string Description { get; set; }
        public Action Action { get; set; }
    }
}