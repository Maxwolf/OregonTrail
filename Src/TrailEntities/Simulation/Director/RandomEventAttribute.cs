using System;

namespace TrailEntities.Simulation
{
    /// <summary>
    ///     Used to tag the base event item class so we can grab all inheriting types that use it.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class RandomEventAttribute : Attribute
    {
    }
}