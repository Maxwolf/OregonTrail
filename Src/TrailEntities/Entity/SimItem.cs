using System;
using System.Diagnostics;

namespace TrailEntities
{
    /// <summary>
    ///     Defines a base SimItem which can represent almost any commodity the player can purchase for the party or vehicle.
    /// </summary>
    public sealed class SimItem : IEntity
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailEntities.SimItem" /> class. The quantity property will be set
        ///     to
        ///     whatever the minimum amount of the SimItem is.
        /// </summary>
        public SimItem(
            SimEntity category,
            string name,
            string pluralForm,
            string delineatingUnit,
            int maxQuantity,
            float cost,
            int weight = 1,
            int minimumQuantity = 1,
            int startingQuantity = 0)
        {
            // Complain if minimum amount is zero, you cannot have zero of something.
            if (minimumQuantity <= 0)
                throw new ArgumentException(
                    "Cannot set minimum quantity of an SimItem to be zero, you cannot have nothing of something!");

            // Setup quantity based on minimum amount.
            StartingQuantity = startingQuantity;
            MinQuantity = minimumQuantity;
            MaxQuantity = maxQuantity;
            Quantity = startingQuantity;

            // Identification of SimItem should be unique, we should also be able to refer to multiples and per.
            Category = category;
            Name = name;
            PluralForm = pluralForm;
            DelineatingUnit = delineatingUnit;

            // Determines how much the SimItem costs in monies.
            Cost = cost;

            // Weight of the SimItem, traditionally this was done in pounds.
            Weight = weight;
        }

        /// <summary>
        ///     Creates a new SimItem from previous instance and with updated quantity.
        /// </summary>
        /// <param name="oldSimItem">Old SimItem that is going to be replaced.</param>
        /// <param name="newQuantity">Updated quantity the new SimItem will have.</param>
        public SimItem(SimItem oldSimItem, int newQuantity)
        {
            // Set updated quantity values, plus ceiling and floor.
            Quantity = newQuantity;
            MinQuantity = oldSimItem.MinQuantity;
            MaxQuantity = oldSimItem.MaxQuantity;
            StartingQuantity = oldSimItem.StartingQuantity;

            // Display name and SimItem entity type.
            Name = oldSimItem.Name;
            Category = oldSimItem.Category;
            Cost = oldSimItem.Cost;
            DelineatingUnit = oldSimItem.DelineatingUnit;
            PluralForm = oldSimItem.PluralForm;
            Weight = oldSimItem.Weight;
        }

        /// <summary>
        ///     Minimum number of this item the player must purchase for it to be considered actually in his inventory.
        /// </summary>
        private int MinQuantity { get; }

        /// <summary>
        ///     Total number of the items the player is going to be taking.
        /// </summary>
        public int Quantity { get; private set; }

        /// <summary>
        ///     Cost of the SimItem in monies.
        /// </summary>
        public float Cost { get; }

        /// <summary>
        ///     Single unit of the items name, for example is there is an Oxen SimItem each one of those items is referred to as an
        ///     'ox'.
        /// </summary>
        public string DelineatingUnit { get; }

        /// <summary>
        ///     When multiple of this SimItem exist in a stack or need to be referenced, such as "10 pounds of food" the 'pounds'
        ///     is
        ///     very important to get correct in context. Another example of this property being used is for Oxen SimItem, a single
        ///     Ox
        ///     is the delineating and the plural form would be "Oxen".
        /// </summary>
        public string PluralForm { get; }

        /// <summary>
        ///     Weight of a single SimItem of this type, the original game used pounds so that is roughly what this should
        ///     represent.
        /// </summary>
        private int Weight { get; }

        /// <summary>
        ///     Total number of items this SimItem represents.
        /// </summary>
        public int StartingQuantity { get; }

        /// <summary>
        ///     Total weight of all food items this represents multiplied by base minimum weight.
        /// </summary>
        public int TotalWeight
        {
            get { return Weight*Quantity; }
        }

        /// <summary>
        ///     Returns the total value of the item which is it's quantity multiplied by it's base cost value.
        /// </summary>
        public float TotalValue
        {
            get { return Cost*Quantity; }
        }

        /// <summary>
        ///     Limit on the number of items that are possible to have of this particular type.
        /// </summary>
        public int MaxQuantity { get; }

        /// <summary>
        ///     Determines what type of SimItem this is, used by the simulation to help sort the items and quickly iterate over
        ///     them
        ///     when looking for a particular piece of data in the vehicles inventory list.
        /// </summary>
        public SimEntity Category { get; }

        /// <summary>
        ///     Display name of the SimItem as it should be known to players.
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
        ///     Forcefully resets the quantity to whatever the starting quantity was configured to be when the item was created.
        /// </summary>
        public void Reset()
        {
            Quantity = StartingQuantity;

            // TODO: Adjust cost of item, create multiplier that can be used to make items more expensive with curve.
        }

        /// <summary>
        ///     Shows off a representation of the SimItem as cost per delineating unit of the particular SimItem.
        /// </summary>
        /// <returns></returns>
        public string ToString(bool storeMode)
        {
            return !storeMode
                ? $"{Cost.ToString("F2")} per {DelineatingUnit}"
                : (Quantity*Cost).ToString("C2");
        }

        /// <summary>
        ///     Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        ///     A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return ToString(false);
        }
    }
}