using System;
using System.Collections.Generic;

namespace TrailEntities
{
    public abstract class ModeInfo : Comparer<ModeInfo>, IComparable<ModeInfo>, IEquatable<ModeInfo>,
        IEqualityComparer<ModeInfo>
    {
        private readonly string _name;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:System.Object" /> class.
        /// </summary>
        protected ModeInfo()
        {
            _name = "UNKNOWN MODE INFO!";
        }

        protected abstract string Name { get; }

        /// <summary>
        ///     Compares the current object with another object of the same type.
        /// </summary>
        /// <returns>
        ///     A value that indicates the relative order of the objects being compared. The return value has the following
        ///     meanings: Value Meaning Less than zero This object is less than the <paramref name="other" /> parameter.Zero This
        ///     object is equal to <paramref name="other" />. Greater than zero This object is greater than
        ///     <paramref name="other" />.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public int CompareTo(ModeInfo other)
        {
            return Compare(this, other);
        }

        /// <summary>
        ///     Determines whether the specified objects are equal.
        /// </summary>
        /// <returns>
        ///     true if the specified objects are equal; otherwise, false.
        /// </returns>
        /// <param name="x">The first object of type <paramref name="T" /> to compare.</param>
        /// <param name="y">The second object of type <paramref name="T" /> to compare.</param>
        public bool Equals(ModeInfo x, ModeInfo y)
        {
            return x.Equals(y);
        }

        /// <summary>
        ///     Returns a hash code for the specified object.
        /// </summary>
        /// <returns>
        ///     A hash code for the specified object.
        /// </returns>
        /// <param name="obj">The <see cref="T:System.Object" /> for which a hash code is to be returned.</param>
        /// <exception cref="T:System.ArgumentNullException">
        ///     The type of <paramref name="obj" /> is a reference type and
        ///     <paramref name="obj" /> is null.
        /// </exception>
        public int GetHashCode(ModeInfo obj)
        {
            return obj.GetHashCode();
        }

        /// <summary>
        ///     Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        ///     true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public bool Equals(ModeInfo other)
        {
            // Reference equality check
            if (this == other)
            {
                return true;
            }

            if (other == null)
            {
                return false;
            }

            if (other.GetType() != GetType())
            {
                return false;
            }

            if (string.Equals(Name, other.Name, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        ///     Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>
        ///     A hash code for the current <see cref="T:System.Object" />.
        /// </returns>
        public override int GetHashCode()
        {
            var hash = 23;
            hash = (hash*31) + Name.GetHashCode();
            return hash;
        }

        /// <summary>
        ///     When overridden in a derived class, performs a comparison of two objects of the same type and returns a value
        ///     indicating whether one object is less than, equal to, or greater than the other.
        /// </summary>
        /// <returns>
        ///     A signed integer that indicates the relative values of <paramref name="x" /> and <paramref name="y" />, as shown in
        ///     the following table.Value Meaning Less than zero <paramref name="x" /> is less than <paramref name="y" />.Zero
        ///     <paramref name="x" /> equals <paramref name="y" />.Greater than zero <paramref name="x" /> is greater than
        ///     <paramref name="y" />.
        /// </returns>
        /// <param name="x">The first object to compare.</param>
        /// <param name="y">The second object to compare.</param>
        /// <exception cref="T:System.ArgumentException">
        ///     Type <paramref name="T" /> does not implement either the
        ///     <see cref="T:System.IComparable`1" /> generic interface or the <see cref="T:System.IComparable" /> interface.
        /// </exception>
        public override int Compare(ModeInfo x, ModeInfo y)
        {
            var result = string.Compare(x.Name, y.Name, StringComparison.OrdinalIgnoreCase);
            if (result != 0) return result;

            return result;
        }
    }
}