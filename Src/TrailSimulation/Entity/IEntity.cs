// Created by Ron 'Maxwolf' McDowell (ron.mcdowell@gmail.com) 
// Timestamp 11/14/2015@3:12 AM

namespace TrailSimulation
{
    using System;
    using System.Collections.Generic;
    using SimUnit;

    /// <summary>
    ///     Base interface for all entities in the simulation, this is used as a constraint for generics in event system.
    /// </summary>
    public interface IEntity : IComparer<IEntity>, IComparable<IEntity>, IEquatable<IEntity>, IEqualityComparer<IEntity>,
        ITick
    {
        /// <summary>
        ///     Name of the entity as it should be known in the simulation.
        /// </summary>
        string Name { get; }
    }
}