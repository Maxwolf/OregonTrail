// Created by Ron 'Maxwolf' McDowell (ron.mcdowell@gmail.com) 
// Timestamp 01/03/2016@1:50 AM

using System;

namespace OregonTrailDotNet.Entity.Item
{
    /// <summary>
    ///     Defines a base SimItem which can represent almost any commodity the player can purchase for the party or
    ///     vehicle.
    /// </summary>
    public sealed class SimItem : IEntity
    {
        /// <summary>Initializes a new instance of the <see cref="T:TrailEntities.Entities.SimItem" /> class.</summary>
        /// <param name="category">The category.</param>
        /// <param name="name">The name.</param>
        /// <param name="pluralForm">The plural Form.</param>
        /// <param name="delineatingUnit">The delineating Unit.</param>
        /// <param name="maxQuantity">The max Quantity.</param>
        /// <param name="cost">The cost.</param>
        /// <param name="weight">The weight.</param>
        /// <param name="minimumQuantity">The minimum Quantity.</param>
        /// <param name="startingQuantity">The starting Quantity.</param>
        /// <param name="pointsAwarded">The points Awarded.</param>
        /// <param name="pointsPerAmount">The points Per Amount.</param>
        public SimItem(
            Entities category,
            string name,
            string pluralForm,
            string delineatingUnit,
            int maxQuantity,
            float cost,
            int weight = 1,
            int minimumQuantity = 1,
            int startingQuantity = 0,
            int pointsAwarded = 1,
            int pointsPerAmount = 1)
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

            // Scoring information for points tabulation if player wins the game.
            PointsPerAmount = pointsPerAmount;
            PointsAwarded = pointsAwarded;

            // All items start off as being in full health.

            // Ensure the values for points per amount are not zero.
            if (PointsPerAmount <= 0)
                PointsPerAmount = 1;

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
        ///     Initializes a new instance of the <see cref="SimItem" /> class.
        ///     Creates a new SimItem from previous instance and with updated quantity.
        /// </summary>
        /// <param name="oldItem">Old SimItem that is going to be replaced.</param>
        /// <param name="newQuantity">Updated quantity the new SimItem will have.</param>
        public SimItem(SimItem oldItem, int newQuantity)
        {
            // Check that new quantity is greater than ceiling.
            if (newQuantity > oldItem.MaxQuantity)
                newQuantity = oldItem.MaxQuantity;

            // Check that new quantity is not less than floor.
            if (newQuantity < oldItem.MinQuantity)
                newQuantity = oldItem.MinQuantity;

            // Set updated quantity values, plus ceiling and floor.
            Quantity = newQuantity;
            MinQuantity = oldItem.MinQuantity;
            MaxQuantity = oldItem.MaxQuantity;
            StartingQuantity = oldItem.StartingQuantity;

            // Scoring information for points tabulation if player wins the game.
            PointsPerAmount = oldItem.PointsPerAmount;
            PointsAwarded = oldItem.PointsAwarded;

            // Ensure the values for points per amount are not zero.
            if (PointsPerAmount <= 0)
                PointsPerAmount = 1;

            // Display name and SimItem entity type.
            Name = oldItem.Name;
            Category = oldItem.Category;
            Cost = oldItem.Cost;
            DelineatingUnit = oldItem.DelineatingUnit;
            PluralForm = oldItem.PluralForm;
            Weight = oldItem.Weight;
        }

        /// <summary>Calculates the total points that should be given for inputted quantity of the object in question.</summary>
        /// <returns>Points to be awarded for the given quantity of the item according to scoring rules.</returns>
        public int Points
        {
            get
            {
                // Check quantity is above zero.
                if (Quantity <= 0)
                    return 0;

                // Check that quantity is above divisor for point calculation.
                if (Quantity < PointsPerAmount)
                    return 0;

                // Figure out how many points for this quantity.
                var points = Quantity/PointsPerAmount*PointsAwarded;

                // Return the result to the caller.
                return points;
            }
        }

        /// <summary>
        ///     Minimum number of this SimItem the player must purchase for it to be considered actually in his inventory.
        /// </summary>
        public int MinQuantity { get; }

        /// <summary>
        ///     Total number of the items the player is going to be taking.
        /// </summary>
        public int Quantity { get; private set; }

        /// <summary>
        ///     Cost of the SimItem in monies.
        /// </summary>
        public float Cost { get; }

        /// <summary>
        ///     Single unit of the items name, for example is there is an Oxen SimItem each one of those items is referred
        ///     to as an
        ///     'ox'.
        /// </summary>
        public string DelineatingUnit { get; }

        /// <summary>
        ///     When multiple of this SimItem exist in a stack or need to be referenced, such as "10 pounds of food" the 'pounds'
        ///     is very important to get correct in context. Another example of this property being used is for Oxen SimItem, a
        ///     single Ox is the delineating and the plural form would be "Oxen".
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
        private int StartingQuantity { get; }

        /// <summary>
        ///     Total points the player will get for this item being in their inventory multiplied by the quantity owned.
        /// </summary>
        public int PointsAwarded { get; }

        /// <summary>
        ///     Defines the quantity of the type of item that must be located in inventory for points awarded to be returned.
        /// </summary>
        public int PointsPerAmount { get; }

        /// <summary>
        ///     Total weight of all food items this represents multiplied by base minimum weight.
        /// </summary>
        public int TotalWeight => Weight*Quantity;

        /// <summary>
        ///     Returns the total value of the SimItem which is it's quantity multiplied by it's base cost value.
        /// </summary>
        public float TotalValue => Cost*Quantity;

        /// <summary>
        ///     Limit on the number of items that are possible to have of this particular type.
        /// </summary>
        public int MaxQuantity { get; }

        /// <summary>
        ///     Determines what type of SimItem this is, used by the simulation to help sort the items and quickly iterate
        ///     over
        ///     them
        ///     when looking for a particular piece of data in the vehicles inventory list.
        /// </summary>
        public Entities Category { get; }

        /// <summary>
        ///     Display name of the SimItem as it should be known to players.
        /// </summary>
        public string Name { get; }

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
        public int Compare(IEntity x, IEntity y)
        {
            var result = string.Compare(x?.Name, y?.Name, StringComparison.Ordinal);
            if (result != 0) return result;

            return result;
        }

        /// <summary>Compares the current object with another object of the same type.</summary>
        /// <returns>
        ///     A value that indicates the relative order of the objects being compared. The return value has the following
        ///     meanings: Value Meaning Less than zero This object is less than the <paramref name="other" /> parameter.Zero This
        ///     object is equal to <paramref name="other" />. Greater than zero This object is greater than
        ///     <paramref name="other" />.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public int CompareTo(IEntity other)
        {
            var result = string.Compare(other.Name, Name, StringComparison.Ordinal);
            if (result != 0) return result;

            return result;
        }

        /// <summary>Indicates whether the current object is equal to another object of the same type.</summary>
        /// <returns>true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.</returns>
        /// <param name="other">An object to compare with this object.</param>
        public bool Equals(IEntity other)
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

        /// <summary>Determines whether the specified objects are equal.</summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <returns>true if the specified objects are equal; otherwise, false.</returns>
        public bool Equals(IEntity x, IEntity y)
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
        public int GetHashCode(IEntity obj)
        {
            var hash = 23;
            hash = hash*31 + Name.GetHashCode();
            return hash;
        }

        /// <summary>
        ///     Called when the simulation is ticked by underlying operating system, game engine, or potato. Each of these system
        ///     ticks is called at unpredictable rates, however if not a system tick that means the simulation has processed enough
        ///     of them to fire off event for fixed interval that is set in the core simulation by constant in milliseconds.
        /// </summary>
        /// <remarks>Default is one second or 1000ms.</remarks>
        /// <param name="systemTick">
        ///     TRUE if ticked unpredictably by underlying operating system, game engine, or potato. FALSE if
        ///     pulsed by game simulation at fixed interval.
        /// </param>
        /// <param name="skipDay">
        ///     Determines if the simulation has force ticked without advancing time or down the trail. Used by
        ///     special events that want to simulate passage of time without actually any actual time moving by.
        /// </param>
        public void OnTick(bool systemTick, bool skipDay)
        {
            // Nothing to see here, move along...
        }

        /// <summary>
        ///     Forcefully resets the quantity to whatever the starting quantity was configured to be when the SimItem was
        ///     created.
        /// </summary>
        public void Reset()
        {
            Quantity = StartingQuantity;

            // TODO: Adjust cost of SimItem, create multiplier that can be used to make items more expensive with curve.
        }

        /// <summary>Shows off a representation of the SimItem as cost per delineating unit of the particular SimItem.</summary>
        /// <param name="storeMode">The store Mode.</param>
        /// <returns>The <see cref="string" />.</returns>
        public string ToString(bool storeMode)
        {
            return !storeMode
                ? $"{Cost:F2} per {DelineatingUnit}"
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

        /// <summary>
        ///     Adjusts the quantity of the item instance to be lower than current quantity. Will automatically check for quantity
        ///     minimum floor and maximum ceiling values and adjust accordingly.
        /// </summary>
        /// <param name="amount">Total amount the quantity will be reduced by.</param>
        public void ReduceQuantity(int amount)
        {
            // Subtract the amount from the quantity.
            var simulatedSubtraction = Quantity - amount;

            // Check that amount is not below minimum floor.
            if (simulatedSubtraction <= 0)
            {
                Quantity = 0;
                return;
            }

            // Check that amount is not above maximum ceiling.
            if (simulatedSubtraction > MaxQuantity)
            {
                Quantity = MaxQuantity;
                return;
            }

            // Set the quantity to desired amount.
            Quantity = simulatedSubtraction;
        }

        /// <summary>
        ///     Adjusts the quantity of the item instance to be higher than current quantity. Will automatically check for maximum
        ///     ceiling and minimum floor values specified in the item.
        /// </summary>
        /// <param name="amount">Amount the quantity should increase by.</param>
        public void AddQuantity(int amount)
        {
            // Add the amount from the quantity.
            var simulatedAddition = Quantity + amount;

            // Check that amount is not below minimum floor.
            if (simulatedAddition < 0)
            {
                Quantity = 0;
                return;
            }

            // Check that amount is not above maximum ceiling.
            if (simulatedAddition > MaxQuantity)
            {
                Quantity = MaxQuantity;
                return;
            }

            // Set the quantity to desired amount.
            Quantity = simulatedAddition;
        }

        /// <summary>
        ///     Repairs the item in full and restores it to working condition.
        /// </summary>
        public void Repair()
        {
        }
    }
}