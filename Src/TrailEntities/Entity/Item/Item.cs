using System;
using System.Diagnostics;

namespace TrailEntities
{
    /// <summary>
    ///     Defines a base item which can represent almost any commodity the player can purchase for the party or vehicle.
    /// </summary>
    public sealed class Item : IEntity
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailEntities.Item" /> class. The quantity property will be set to
        ///     whatever the minimum amount of the item is.
        /// </summary>
        public Item(
            SimEntity category,
            string name,
            string pluralForm,
            string delineatingUnit,
            int maxQuantity,
            float cost,
            int weight = 1,
            int minimumQuantity = 1)
        {
            // Complain if minimum amount is zero, you cannot have zero of something.
            if (minimumQuantity <= 0)
                throw new ArgumentException(
                    "Cannot set minimum amount of an item to be zero, you cannot have nothing of something!");

            // Setup quantity based on minimum amount.
            MinimumQuantity = minimumQuantity;
            MaxQuantity = maxQuantity;
            Quantity = minimumQuantity;

            // Identification of item should be unique, we should also be able to refer to multiples and per.
            Category = category;
            Name = name;
            PluralForm = pluralForm;
            DelineatingUnit = delineatingUnit;

            // Determines how much the item costs in monies.
            Cost = cost;

            // Weight of the item, traditionally this was done in pounds.
            Weight = weight;
        }

        /// <summary>
        ///     Creates a new item from previous instance and with updated quantity.
        /// </summary>
        /// <param name="oldItem">Old item that is going to be replaced.</param>
        /// <param name="newQuantity">Updated quantity the new item will have.</param>
        public Item(Item oldItem, int newQuantity)
        {
            // Complain if new quantity is above maximum.
            if (newQuantity > MaxQuantity)
                throw new ArgumentException("New quantity for item cannot be larger than predefined maximum!");

            // Set updated quantity.
            Quantity = newQuantity;

            // Set other various item properties.
            Cost = oldItem.Cost;
            DelineatingUnit = oldItem.DelineatingUnit;
            PluralForm = oldItem.PluralForm;
            Weight = oldItem.Weight;
            MinimumQuantity = oldItem.MinimumQuantity;
            MaxQuantity = oldItem.MaxQuantity;
            Category = oldItem.Category;
            Name = oldItem.Name;
        }

        /// <summary>
        ///     Determines what type of item this is, used by the simulation to help sort the items and quickly iterate over them
        ///     when looking for a particular piece of data in the vehicles inventory list.
        /// </summary>
        public SimEntity Category { get; }

        /// <summary>
        ///     Total number of the items the player is going to be taking.
        /// </summary>
        public int Quantity { get; }

        /// <summary>
        ///     Cost of the item in monies.
        /// </summary>
        public float Cost { get; }

        /// <summary>
        ///     Single unit of the items name, for example is there is an Oxen item each one of those items is referred to as an
        ///     'ox'.
        /// </summary>
        public string DelineatingUnit { get; }

        /// <summary>
        ///     When multiple of this item exist in a stack or need to be referenced, such as "10 pounds of food" the 'pounds' is
        ///     very important to get correct in context. Another example of this property being used is for Oxen item, a single Ox
        ///     is the delineating and the plural form would be "Oxen".
        /// </summary>
        public string PluralForm { get; }

        /// <summary>
        ///     Weight of a single item of this type, the original game used pounds so that is roughly what this should represent.
        /// </summary>
        private int Weight { get; }

        /// <summary>
        ///     Total number of items this item represents.
        /// </summary>
        private int MinimumQuantity { get; }

        /// <summary>
        ///     Total weight of all food items this represents multiplied by base minimum weight.
        /// </summary>
        public int TotalWeight
        {
            get { return Weight*MinimumQuantity; }
        }

        /// <summary>
        ///     Limit on the number of items that are possible to have of this particular type.
        /// </summary>
        public int MaxQuantity { get; }

        /// <summary>
        ///     Display name of the item as it should be known to players.
        /// </summary>
        public string Name { get; }

        /// <summary>
        ///     Compares two objects and returns a value indicating whether one is less than, equal to, or greater than the other.
        /// </summary>
        /// <returns>
        ///     A signed integer that indicates the relative values of <paramref name="x" /> and <paramref name="y" />, as shown in
        ///     the following table.Value Meaning Less than zero<paramref name="x" /> is less than <paramref name="y" />.Zero
        ///     <paramref name="x" /> equals <paramref name="y" />.Greater than zero<paramref name="x" /> is greater than
        ///     <paramref name="y" />.
        /// </returns>
        /// <param name="x">The first object to compare.</param>
        /// <param name="y">The second object to compare.</param>
        public int Compare(IEntity x, IEntity y)
        {
            Debug.Assert(x != null, "x != null");
            Debug.Assert(y != null, "y != null");

            var result = string.Compare(x.Name, y.Name, StringComparison.Ordinal);
            if (result != 0) return result;

            return result;
        }

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
        public int CompareTo(IEntity other)
        {
            Debug.Assert(other != null, "other != null");

            var result = string.Compare(other.Name, Name, StringComparison.Ordinal);
            if (result != 0) return result;

            return result;
        }

        /// <summary>
        ///     Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        ///     true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public bool Equals(IEntity other)
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

            if (Name.Equals(other.Name))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        ///     Determines whether the specified objects are equal.
        /// </summary>
        /// <returns>
        ///     true if the specified objects are equal; otherwise, false.
        /// </returns>
        public bool Equals(IEntity x, IEntity y)
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
        public int GetHashCode(IEntity obj)
        {
            var hash = 23;
            hash = (hash*31) + Name.GetHashCode();
            return hash;
        }

        /// <summary>
        ///     Shows off a representation of the item as cost per delineating unit of the particular item.
        /// </summary>
        /// <returns></returns>
        public string ToString(bool storeMode)
        {
            return storeMode
                ? $"{Cost.ToString("F2")} per {DelineatingUnit}"
                : (Quantity*Cost).ToString("C2");
        }
    }
}