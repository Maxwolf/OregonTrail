using System;
using System.Collections.Generic;

namespace TrailEntities
{
    /// <summary>
    ///     Base interface for all entities in the simulation, this is used as a constraint for generics in event system.
    /// </summary>
    public interface IEntity : IComparer<IEntity>, IComparable<IEntity>, IEquatable<IEntity>, IEqualityComparer<IEntity>
    {
        /// <summary>
        ///     Name of the entity as it should be known in the simulation.
        /// </summary>
        string Name { get; }

        /// <summary>
        ///     Defines what type of entity this will take the role of in the simulation. Depending on this value the simulation
        ///     will affect how it is treated, points tabulated, and interactions governed.
        /// </summary>
        SimEntity Category { get; }
    }
}