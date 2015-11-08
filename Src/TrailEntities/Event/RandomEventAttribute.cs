using System;

namespace TrailEntities.Event
{
    /// <summary>
    ///     Used to tag the base event item class so we can grab all inheriting types that use it.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class RandomEventAttribute : Attribute
    {
    }
}