using System;

namespace TrailEntities.Simulation.Mode
{
    /// <summary>
    ///     Defines a choice in the dynamic action selection system for a given game mode. This is intended to be used by a
    ///     wrapper for menu choices that aggregates all of the possible actions a given game mode can make while it is active
    ///     in the simulation.
    /// </summary>
    public sealed class ModeChoiceItem<T> : IModeChoiceItem<T> where T : struct, IComparable, IFormattable, IConvertible
    {
        public ModeChoiceItem(T command, Action action, string description)
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

        public T Command { get; set; }
        public string Description { get; set; }
        public Action Action { get; set; }
    }
}