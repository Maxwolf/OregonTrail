// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IEntity.cs" company="Ron 'Maxwolf' McDowell">
//   ron.mcdowell@gmail.com
// </copyright>
// <summary>
//   Base interface for all entities in the simulation, this is used as a constraint for generics in event system.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace TrailSimulation.Entity
{
    using System;
    using System.Collections.Generic;
    using Core;

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