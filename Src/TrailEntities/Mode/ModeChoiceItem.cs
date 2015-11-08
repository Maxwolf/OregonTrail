using System;

namespace TrailEntities.Mode
{
    /// <summary>
    ///     Defines a choice in the dynamic action selection system for a given game mode. This is intended to be used by a
    ///     wrapper for menu choices that aggregates all of the possible actions a given game mode can make while it is active
    ///     in the simulation.
    /// </summary>
    public sealed class ModeChoiceItem
    {
        public ModeChoiceItem(object command, Action action, string description)
        {
            // Complain the generics implemented is not of an enum type.
            if (!command.GetType().IsEnum)
            {
                throw new InvalidCastException("GameMode choice command must be an enumerated type!");
            }

            // Pass along the information about this choice.
            Command = command;
            Action = action;
            Description = description;
        }

        public object Command { get; }
        public string Description { get;}
        public Action Action { get; }
    }
}