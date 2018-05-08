// Created by Ron 'Maxwolf' McDowell (ron.mcdowell@gmail.com) 
// Timestamp 01/03/2016@1:50 AM

using System;
using System.Collections.Generic;
using OregonTrailDotNet.Event;

namespace OregonTrailDotNet.Module.Director
{
    /// <summary>
    ///     Acts like a unique identifier for each event that is to be registered in the system and defines several special
    ///     things about that we want in a key such as category, name, and if it should be random or not.
    /// </summary>
    public sealed class EventKey : IComparer<EventKey>, IComparable<EventKey>, IEquatable<EventKey>,
        IEqualityComparer<EventKey>
    {
        /// <summary>Initializes a new instance of the <see cref="T:OregonTrailDotNet.Module.Director.EventKey" /> class.</summary>
        /// <param name="category">The category.</param>
        /// <param name="name">The name.</param>
        /// <param name="executionType">The execution Type.</param>
        public EventKey(EventCategory category, string name, EventExecution executionType)
        {
            ExecutionType = executionType;
            Category = category;
            Name = name;
        }

        /// <summary>
        ///     Category the event should fall under so when we select by type we can include this event in that selection.
        /// </summary>
        public EventCategory Category { get; }

        /// <summary>
        ///     Name of the event, typically this is the type name but it really could be anything.
        /// </summary>
        public string Name { get; }

        /// <summary>
        ///     Determines if this event will be selected for being chosen at random when events are fired by category and not
        ///     directly by their type.
        /// </summary>
        public EventExecution ExecutionType { get; }

        /// <summary>Compares the current object with another object of the same type.</summary>
        /// <returns>
        ///     A value that indicates the relative order of the objects being compared. The return value has the following
        ///     meanings: Value Meaning Less than zero This object is less than the <paramref name="other" /> parameter.Zero This
        ///     object is equal to <paramref name="other" />. Greater than zero This object is greater than
        ///     <paramref name="other" />.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public int CompareTo(EventKey other)
        {
            var result = string.Compare(other.Name, Name, StringComparison.Ordinal);
            if (result != 0) return result;

            return result;
        }

        /// <summary>
        ///     Compares two objects and returns a value indicating whether one is less than, equal to, or greater than the
        ///     other.
        /// </summary>
        /// <returns>
        ///     A signed integer that indicates the relative values of <paramref name="x" /> and <paramref name="y" />, as shown in
        ///     the following table.Value Meaning Less than zero<paramref name="x" /> is less than <paramref name="y" />.Zero
        ///     <paramref name="x" /> equals <paramref name="y" />.Greater than zero<paramref name="x" /> is greater than
        ///     <paramref name="y" />.
        /// </returns>
        /// <param name="x">The first object to compare.</param>
        /// <param name="y">The second object to compare.</param>
        public int Compare(EventKey x, EventKey y)
        {
            var result = string.Compare(x?.Name, y?.Name, StringComparison.Ordinal);
            if (result != 0) return result;

            return result;
        }

        /// <summary>Determines whether the specified objects are equal.</summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <returns>true if the specified objects are equal; otherwise, false.</returns>
        public bool Equals(EventKey x, EventKey y)
        {
            return x.Equals(y);
        }

        /// <summary>Returns a hash code for the specified object.</summary>
        /// <returns>A hash code for the specified object.</returns>
        /// <param name="obj">The <see cref="T:System.Object" /> for which a hash code is to be returned.</param>
        /// <exception cref="T:System.ArgumentNullException">
        ///     The type of <paramref name="obj" /> is a reference type and
        ///     <paramref name="obj" /> is null.
        /// </exception>
        public int GetHashCode(EventKey obj)
        {
            var hash = 23;
            hash = hash*31 + Name.GetHashCode();
            return hash;
        }

        /// <summary>Indicates whether the current object is equal to another object of the same type.</summary>
        /// <returns>true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.</returns>
        /// <param name="other">An object to compare with this object.</param>
        public bool Equals(EventKey other)
        {
            // Reference equality check
            if (this == other)
                return true;

            if (other == null)
                return false;

            if (other.GetType() != GetType())
                return false;

            if (Name.Equals(other.Name))
                return true;

            return false;
        }

        /// <summary>
        ///     Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        ///     A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return Name;
        }
    }
}