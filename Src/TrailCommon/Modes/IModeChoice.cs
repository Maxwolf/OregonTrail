using System;

namespace TrailCommon
{
    /// <summary>
    ///     Defines a choice in the dynamic action selection system for a given game mode. This is intended to be used by a
    ///     wrapper for menu choices that aggregates all of the possible actions a given game mode can make while it is active
    ///     in the simulation.
    /// </summary>
    public interface IModeChoice
    {
        string CommandName { get; set; }
        string Description { get; set; }
        Action Action { get; set; }
    }
}