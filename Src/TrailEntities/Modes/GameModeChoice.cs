using System;
using TrailCommon;

namespace TrailEntities
{
    /// <summary>
    ///     Defines a choice in the dynamic action selection system for a given game mode. This is intended to be used by a
    ///     wrapper for menu choices that aggregates all of the possible actions a given game mode can make while it is active
    ///     in the simulation.
    /// </summary>
    public sealed class GameModeChoice : IModeChoice
    {
        public GameModeChoice(Action action, string description, string commandName)
            : this(action, description)
        {
            CommandName = commandName;
        }

        public GameModeChoice(Action action, string description)
        {
            Description = description;
            Action = action;
        }

        public string CommandName { get; set; }
        public string Description { get; set; }
        public Action Action { get; set; }
    }
}